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
        public ActionResult Index()
        {
            return View();
        }*/

        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        String proy;

      /*  public recibeNomProyecto(string nomProyecto)
        {


        }*/

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

        public ActionResult Create(string NombProy)
        {
            string id = NombProy;

            ModReqFuncionalInter RQ = new ModReqFuncionalInter();
            RQ.Requerimientos = new ReqFuncional();
            RQ.nProy = NombProy;
            RQ.Requerimientos.nomProyecto = NombProy;
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
            foreach (var item in modelo.listaCriterios) {
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
            {
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModReqFuncionalInter modelo, HttpPostedFileBase imagen1,String NProyecto)
        {
            var NReqFun = from RF in BD.ReqFuncional select RF;
            
            //modelo.Requerimientos.nomProyecto = modelo.nProy;
           var NombreP = modelo.Requerimientos.nomProyecto;
            modelo.Requerimientos.estado = "Iniciado";
            /*Funcion para poder guardar una imagen*/
            if (imagen1 != null) { 
                modelo.Requerimientos.imagen = new byte[imagen1.ContentLength];
                imagen1.InputStream.Read(modelo.Requerimientos.imagen, 0, imagen1.ContentLength);
            }

            /*Funcion para poder guardar una imagen*/
            BD.ReqFuncional.Add(modelo.Requerimientos);
            BD.SaveChanges();
            List<ReqFuncional> LR;
            LR = BD.ReqFuncional.ToList();
            var idReq = LR.Last().id;
            if (modelo.values != null) {
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
            /* if (ModelState.IsValid)
             {
                 //var idRF;
                 var NReqFun = from RF in BD.ReqFuncional select RF;
                 // NReqFun = NReqFun.Where(x => x.nombre == modelo.RequerimientosF.nomProyecto).Max(x => x.id);
                 BD.ReqFuncional.Add(modelo.RequerimientosF);
                 BD.SaveChanges();
             }
             else
             {
                 ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                 return View(modelo);
             }*/
            string id = modelo.Requerimientos.nomProyecto;
            ModReqFuncionalInter RQ = new ModReqFuncionalInter();
            var req = from usersP in BD.ReqFuncional
                      select usersP;
            req = req.Where(x => x.nombre == id);
            RQ.listaRequerimientos = req.ToList();
            /* if (RQ.listaRequerimientos.Count == 0)
             {
                 RQ.Requerimientos.id = 0;
             }
             else {
               Int32 d = RQ.listaRequerimientos.Count;
                Int16 d2 = (Int16) (d + 1);
                 d= (Int32)(d2);
                 RQ.Requerimientos.id = (Int16)(d);
             }*/

            /* var proy = from usersP in BD.Proyecto
                            select usersP;

             proy = proy.Where(x => x.nombre == id);

             RQ.proyecto = proy;

             for (int j=0; j<RQ.listaProyecto.Count;j++) {

             }*/
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

