using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using ProyectoInge1.Models;
using System.Web.Security;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;
using PagedList;
using System.Text;




namespace ProyectoInge1.Controllers
{
    public class ReqFuncionalController : Controller
    {

        /*// GET: ReqFuncional
        */

        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        ApplicationDbContext context = new ApplicationDbContext();
       

        private bool revisarPermisos(string permiso)
        {
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var listaPermisos = BD.Permiso;
            var permisoID = 1;
            foreach (var perm in listaPermisos)
            {
                if (perm.descripcion == permiso)
                {
                    permisoID = perm.id;
                }
            }
            var listaRoles = BD.NetRolesPermiso.Where(m => m.idPermiso == permisoID).ToList().Select(n => n.idNetRoles);
            bool userRol = listaRoles.Contains(rol.RoleId);
            return userRol;
        }
        // GET: Usuarios
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, string nombreProyecto)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Rol" ? "rol_desc" : "Rol";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            string param1 = nombreProyecto;

            var requerimientos = from rfunc in BD.ReqFuncional
                                 where rfunc.nomProyecto == param1  // aquí va el parámetro recibido:  where rfunc.nomProyecto == parámetro.
                                 select rfunc;
            if (!String.IsNullOrEmpty(searchString))
            {
                requerimientos = requerimientos.Where(rfunc => rfunc.nombre.Contains(searchString) || rfunc.nombre.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    requerimientos = requerimientos.OrderByDescending(rfunc => rfunc.nombre);
                    break;
                default:
                    requerimientos = requerimientos.OrderBy(rfunc => rfunc.nombre);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ModReqFuncionalInter modelo = new ModReqFuncionalInter();
            modelo.listaRequerimientos = requerimientos.ToList();
            return View(requerimientos.ToList().ToPagedList(pageNumber, pageSize));
        }
        
		/*
		    Inicializa los datos necesarios para la creacion de un requerimienro funcional en un proyecto
			@param NombProy: Consiste en una hilera de caracteres que corresponde al nombre de un proyecto existente en la
                             base de datos.
			@return: Un modelo de requerimiento funcional que contiene el nombre del proyecto al que pertenece y un ViewBag 
                     que contieneuna lista de usuarios relacionados a que participan de ese proyecto.
		*/
        public ActionResult Create(string NombProy)
        {
            if (!revisarPermisos("Crear RF"))
            {
                return RedirectToAction("Index", "ReqFuncional");
            }
            string id = NombProy;
            ModReqFuncionalInter RQ = new ModReqFuncionalInter();
            RQ.Requerimiento = new ReqFuncional();
            RQ.nProy = NombProy;
            RQ.Requerimiento.nomProyecto = NombProy;
            RQ.UsuariosSistema = BD.Usuario.ToList();
            RQ.proyecto = BD.Proyecto.Find(id);
            RQ.listaUsuario = RQ.proyecto.Usuario2.ToList();
            var Temp = RQ.UsuariosSistema.Intersect(RQ.listaUsuario);
            RQ.listaUsuarioView = Temp.ToList();
            ViewBag.lista = RQ.listaUsuarioView;
            return View(RQ);
        }

    

        public ActionResult Details(short id)
        {
            /*if (!revisarPermisos("Detalles de Usuario"))
            {
                return RedirectToAction("Index", "Usuario");
            }*/
            ModReqFuncionalInter modelo = new ModReqFuncionalInter();
            var requerimiento = from rfunc in BD.ReqFuncional
                                 where rfunc.id == id  // aquí va el parámetro recibido:  where rfunc.nomProyecto == parámetro.
                                 select rfunc;
            modelo.Requerimiento = BD.ReqFuncional.Find(id, requerimiento.First().nomProyecto);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.Requerimiento.responsable1);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.Requerimiento.responsable2);
            
            modelo.listaCriterios = BD.CriterioAceptacion.ToList();
            modelo.values = "";
            foreach (var item in modelo.listaCriterios) {               //elimina los criterios de aceptacion para ser sustituidos por lños nuevos (pueden ser los mismos)
                modelo.values += item.criterio + "|"; 
                BD.Entry(item).State = EntityState.Deleted;
                BD.SaveChanges();
            }
            modelo.listaRequerimientos = requerimiento.ToList();
            modelo.UsuariosSistema = BD.Usuario.ToList();
            modelo.listaUsuario = modelo.Requerimiento.Proyecto.Usuario2.ToList();
            var temp = modelo.UsuariosSistema.Intersect(modelo.listaUsuario);
            modelo.listaUsuarioView = temp.ToList();
            ViewBag.Lista = modelo.listaUsuarioView;
            return View(modelo);

        }


         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Details(ModReqFuncionalInter modelo, HttpPostedFileBase imagen1)
         {

            /*Funcion para poder guardar una imagen*/
           if (imagen1 != null)
           {
                         modelo.Requerimiento.imagen = new byte[imagen1.ContentLength];
                         imagen1.InputStream.Read(modelo.Requerimiento.imagen, 0, imagen1.ContentLength);
           }
       
           BD.Entry(modelo.Requerimiento).State = EntityState.Modified;
           BD.SaveChanges();

            if (modelo.values != null)
            {   // Separa los criterios de aceptacion y los guarda en la base de datos.
                String[] substrings = modelo.values.Split('|');
                foreach (var substring in substrings)
                {
                    CriterioAceptacion mod = new CriterioAceptacion();
                    mod.idReqFunc = modelo.Requerimiento.id;
                    mod.nomProyecto = modelo.Requerimiento.nomProyecto;
                    mod.criterio = substring;
                    BD.CriterioAceptacion.Add(mod);
                    BD.SaveChanges();
                }
            }

            return RedirectToAction("Index", new { nombreProyecto = modelo.Requerimiento.nomProyecto }); 
         }
        

        public ActionResult Eliminar(bool confirm, string Requerimiento, string nomProy)
        {

            if (confirm == true)
            {
                int idReq = Int32.Parse(Requerimiento);
                var RequerimientoFun = BD.ReqFuncional.Find(idReq, nomProy);
                BD.Entry(RequerimientoFun).State = EntityState.Deleted;
                BD.SaveChanges();
                return RedirectToAction("Index", new { nombreProyecto = nomProy });
            }
            else {                 
                return RedirectToAction("Details", new { id = Requerimiento });
            }
        }


        /*
		    Almacena los datos ingresados por un usuario para la creacion de un requerimienro funcional en un proyecto
			@param modelo: Consiste en los datos ingresados por un usuario que corresponde a la informacion referente para la 
                           creacion de un requerimiento funcional para un proyecto.
            @param imagen1: Consiste en los datos referentes a una imagen almacenada por un usuario, puede estar vacia.                 
			@return: Un modelo de requerimiento funcional que contiene el nombre del proyecto al que pertenece y un ViewBag 
                     que contieneuna lista de usuarios relacionados a que participan de ese proyecto.
		*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModReqFuncionalInter modelo, HttpPostedFileBase imagen1,String NProyecto)
        {
            var NReqFun = from RF in BD.ReqFuncional select RF;
            var NombreP = modelo.Requerimiento.nomProyecto;
            modelo.Requerimiento.estado = "Iniciado";
            /*Inicio Funcion para poder guardar una imagen*/
            if (imagen1 != null) { 
                modelo.Requerimiento.imagen = new byte[imagen1.ContentLength];
                imagen1.InputStream.Read(modelo.Requerimiento.imagen, 0, imagen1.ContentLength);
            }
            /*Final Funcion para poder guardar una imagen*/
            BD.ReqFuncional.Add(modelo.Requerimiento);
            BD.SaveChanges();
            List<ReqFuncional> LR;
            LR = BD.ReqFuncional.ToList();
            var idReq = LR.Last().id;
            if (modelo.values != null)
            {   //Guarda los criterios de aceptacion
                String[] substrings = modelo.values.Split('|');
                foreach (var substring in substrings)
                {
                   CriterioAceptacion mod= new CriterioAceptacion();
                    mod.idReqFunc = idReq;
                    mod.nomProyecto = NombreP;
                    mod.criterio = substring;
                    BD.CriterioAceptacion.Add(mod);
                    BD.SaveChanges();
                }
            }
            string id = modelo.Requerimiento.nomProyecto;
            ModReqFuncionalInter RQ = new ModReqFuncionalInter();
            var req = from usersP in BD.ReqFuncional
                      select usersP;
            req = req.Where(x => x.nombre == id);
            RQ.listaRequerimientos = req.ToList();
            RQ.nProy = id;
            RQ.UsuariosSistema = BD.Usuario.ToList();
            RQ.proyecto = BD.Proyecto.Find(id);
            RQ.listaUsuario = RQ.proyecto.Usuario2.ToList();
            var Temp = RQ.UsuariosSistema.Intersect(RQ.listaUsuario);
            RQ.listaUsuarioView = Temp.ToList();
            ViewBag.lista = RQ.listaUsuarioView;
            return View(RQ);
        }
      
    }
}

