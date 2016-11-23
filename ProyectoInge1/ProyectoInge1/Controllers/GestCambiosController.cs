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
    public class GestCambiosController : Controller
    {

        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        ApplicationDbContext context = new ApplicationDbContext();

        private bool revisarPermisos(string permiso)
        {
            
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var permisoID = BD.Permiso.Where(m => m.descripcion == permiso).First().id;
            var listaRoles = BD.NetRolesPermiso.Where(m => m.idPermiso == permisoID).ToList().Select(n => n.idNetRoles);
            bool userRol = listaRoles.Contains(rol.RoleId);
            return userRol;
        }

        public ActionResult Solicitudes(string sortOrder, string currentFilter, string searchString, int? page)
        {
            /*if (!revisarPermisos("Index de usuario"))
            {
                 this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }*/

            ViewBag.CurrentSort = sortOrder;
            ViewBag.ReqSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ProySortParm = sortOrder == "Proy" ? "proy_desc" : "Proy";
            ViewBag.VersSortParm = sortOrder == "Vers" ? "version_desc" : "Vers";
            ViewBag.RealSortParm = sortOrder == "Real" ? "real_desc" : "Real";
            ViewBag.EstSortParm = sortOrder == "Est" ? "est_desc" : "Est";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var solicitudes = from solicitud in BD.Solicitud
                              join req in BD.ReqFuncional on solicitud.idReqFunc equals req.id
                              select solicitud;
            var requerimientos = from requerimiento in BD.ReqFuncional
                                 select requerimiento;
            var usuarios = from usuario in BD.Usuario
                           select usuario;
            ModGestionCambios modelo = new ModGestionCambios();
            if (!String.IsNullOrEmpty(searchString))
            {
                solicitudes = solicitudes.Where(sol => sol.nombreRF.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.nombreRF); break;
                case "Proy": solicitudes = solicitudes.OrderBy(solicitud => solicitud.nomProyecto); break;
                case "proy_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.nomProyecto); break;
                case "Version": solicitudes = solicitudes.OrderBy(solicitud => solicitud.versionRF); break;
                case "version_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.versionRF); break;
                case "Real": solicitudes = from sol in BD.Solicitud
                                           join user in BD.Usuario on sol.realizadoPor equals user.cedula
                                           orderby user.nombre ascending
                                           select sol; break;
                case "real_desc": solicitudes = from sol in BD.Solicitud
                                                join user in BD.Usuario on sol.realizadoPor equals user.cedula
                                                orderby user.nombre descending
                                                select sol; break;
                case "Est": solicitudes = solicitudes.OrderBy(solicitud => solicitud.estado); break;
                case "est_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.estado); break;
                default: solicitudes = solicitudes.OrderBy(sol => sol.nombreRF); break;
            }    
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            //modelo.listaRequerimientos = requerimientos.ToList();
            //modelo.listaUsuarios = usuarios.ToList();
            ViewBag.reqFuncList = requerimientos.ToList();
            ViewBag.userList = usuarios.ToList();
            //ViewBag.reqList = new SelectList(modelo.listaRequerimientos, "id", "nombre");
            modelo.listaSolicitudes = solicitudes.ToList();
            return View(solicitudes.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Details(string id)
        {
            /*if (!revisarPermisos("Detalles de Usuario"))
            {
                return RedirectToAction("Index", "Usuario");
            }*/
            var usuarios = from usuario in BD.Usuario
                           orderby usuario.cedula
                           select usuario;
            ModGestionCambios modelo = new ModGestionCambios();
            string[] parameters = id.Split('~');
            short version = Convert.ToInt16(parameters[0]);
            int idRF = Convert.ToInt32(parameters[1]);
            string nomProy = parameters[2];
            string fecha = parameters[3].Replace('-', ':').Replace('_', '-');
            string currentUser = parameters[4];
            var userView = from user in BD.Usuario
                           where currentUser == user.id
                           select user;
            modelo.userInView = userView.ToList().First();
            bool? lider = modelo.userInView.lider;
            DateTime myDate = DateTime.ParseExact(fecha, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            modelo.solicitud = BD.Solicitud.Find(myDate, version, idRF, nomProy);
            modelo.versionReq = BD.HistVersiones.Find(version, idRF, nomProy);
            modelo.Requerimiento = BD.ReqFuncional.Find(idRF, nomProy);
            modelo.Proyecto = BD.Proyecto.Find(nomProy);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.versionReq.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.versionReq.responsable2RF);
            ViewBag.userList = usuarios.ToList();
            modelo.Proyecto = BD.Proyecto.Find(nomProy);
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyUsuarios = modelo.Proyecto.Usuario2.ToList();
            var D = modelo.listaUsuarios.Intersect(modelo.listaProyUsuarios);
            modelo.listaUsuarioView = D.ToList();
            ViewBag.Lista = modelo.listaUsuarioView;
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(ModGestionCambios modelo)
        {
            if (!revisarPermisos("Modificar Solicitud"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Usuario");
            }
            if (ModelState.IsValid)
            {
                if (modelo.solicitud.estado == "Pendiente")
                {
                    string s = modelo.solicitud.fecha.ToString();
                    BD.Entry(modelo.solicitud).State = EntityState.Modified;
                    BD.SaveChanges();
                }
            }
            return RedirectToAction("Solicitudes");
        }

        // GET: Gestion
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!revisarPermisos("Index de Version"))
            {

                return RedirectToAction("Index", "Home");
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Rol" ? "rol_desc" : "Rol";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var versiones = from verCam in BD.HistVersiones
                           select verCam;
            if (!String.IsNullOrEmpty(searchString))
            {
                versiones = versiones.Where(cambios => cambios.razon.Contains(searchString) || cambios.nomProyecto.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    versiones = versiones.OrderByDescending(users => users.versionRF);
                    break;
                default:
                    versiones = versiones.OrderBy(users => users.versionRF);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ModGestionCambios modelo = new ModGestionCambios();
             modelo.listaCambios = versiones.ToList();
            modelo.listaUsuarios = BD.Usuario.ToList();
            ViewBag.userList = modelo.listaUsuarios;
            //modelo.listaSolicitud = versiones.ToList();
            // modelo.listaModelos= versiones.ToList();
           // ViewBag.Desarrolladores = new SelectList(model.DesarrolladoresNoLider, "cedula", "names");
            return View(versiones.ToList().ToPagedList(pageNumber, pageSize));
        }



        public ActionResult Detalles(/*int id,int Ver*/)
        {
            int id = 3;
            int Ver = 1;
            DateTime Momento = DateTime.Parse("2016-11-18 13:00:53.000");
            string Proy = "Inge I";
            ModGestionCambios modelo = new ModGestionCambios();
            modelo.listaUsuarios= BD.Usuario.ToList();
            var VReq = BD.Solicitud.ToList();
            foreach(var VRF in VReq){
                if (DateTime.Compare(VRF.fecha, Momento) == 0){
                    if (VRF.idReqFunc==id) {
                         if (VRF.versionRF==Ver) {
                            modelo.solicitud = VRF;
                        }
                    }
                }
            }
            modelo.Proyecto = BD.Proyecto.Find(Proy);
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyUsuarios = modelo.Proyecto.Usuario2.ToList();
           
            var D = modelo.listaUsuarios.Intersect(modelo.listaProyUsuarios);
            modelo.listaUsuarioView = D.ToList();
            ViewBag.Lista = modelo.listaUsuarioView;
            return View(modelo);

        }

        /* public ActionResult Index()
         {
             ModGestionCambios GestionC = new ModGestionCambios();
             GestionC.listaProyectos = BD.Proyecto.ToList();
             return View(GestionC);

         }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(ModGestionCambios modelo ) {
            if (ModelState.IsValid) {
                // BD.Solicitud.Find(modelo.Solicitud.fecha,modelo.Solicitud.versionRF,modelo.Solicitud.nomProyecto,modelo.Solicitud.idReqFunc);
                if (modelo.solicitud.estado=="Pendiente") {
                    string s = modelo.solicitud.fecha.ToString();

                    // DateTime T = DateTime.ParseExact(s, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    BD.Entry(modelo.solicitud).State = EntityState.Modified;
                    //BD.Entry(modelo.Solicitud).Property(M => M.razon).IsModified = true;
                    // BD.Solicitud.Add(modelo.Solicitud);
                    BD.SaveChanges();
                }
            }

            int id = 3;
            int Ver = 1;
            DateTime Momento = DateTime.Parse("2016-11-18 13:00:53.000");
            string Proy = "Inge I";
            ModGestionCambios mod = new ModGestionCambios();
            mod.listaUsuarios = BD.Usuario.ToList();
            var VReq = BD.Solicitud.ToList();
            foreach (var VRF in VReq)
            {
                if (DateTime.Compare(VRF.fecha, Momento) == 0)
                {
                    if (VRF.idReqFunc == id)
                    {
                        if (VRF.versionRF == Ver)
                        {
                            mod.solicitud = VRF;
                        }
                    }
                }
            }
            mod.Proyecto = BD.Proyecto.Find(Proy);
            mod.listaUsuarios = BD.Usuario.ToList();
            mod.listaProyUsuarios = mod.Proyecto.Usuario2.ToList();
            return View(mod);
        }

        public ActionResult Create(int? versionRF,int? idReqFunc,string nomProyecto)
        {

            return View();
        }
        }

}

/*
@if(Request.IsAuthenticated && User.IsInRole("Desarrollador") && Model.userInView.cedula == Model.proyecto.lider)
{*/