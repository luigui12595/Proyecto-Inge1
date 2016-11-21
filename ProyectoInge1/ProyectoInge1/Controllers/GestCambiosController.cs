using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ProyectoInge1.Models;
using Microsoft.AspNet.Identity;

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

        // GET: GestCambios
        public ActionResult Index()
        {
            return View();
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
            ViewBag.VersSortParm = sortOrder == "Proy" ? "proy_desc" : "proy";
            ViewBag.VersSortParm = sortOrder == "Version" ? "version_desc" : "version";
            ViewBag.RealSortParm = sortOrder == "Real" ? "real_desc" : "real";
            ViewBag.EstSortParm = sortOrder == "Est" ? "est_desc" : "est";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var solicitudes = from solicitud in BD.Solicitud
                              join req in BD.ReqFuncional on solicitud.idReqFunc equals req.id
                              orderby req.nombre
                              select solicitud;
            var requerimientos = from requerimiento in BD.ReqFuncional
                                 select requerimiento;
            var usuarios = from usuario in BD.Usuario
                           orderby usuario.cedula
                           select usuario;
            ModGestionCambios modelo = new ModGestionCambios();
            if (!String.IsNullOrEmpty(searchString))
            {
                solicitudes = from solicitud in BD.Solicitud
                              join req in BD.ReqFuncional on solicitud.idReqFunc equals req.id
                              where req.nombre.Contains(searchString)
                              orderby req.nombre
                              select solicitud;
            }
            switch (sortOrder)
            {
                case "name_desc": solicitudes = solicitudes.Reverse(); break;
                case "Version": solicitudes = solicitudes.OrderBy(solicitud => solicitud.versionRF); break;
                case "version_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.versionRF); break;
                case "Proy": solicitudes = solicitudes.OrderBy(solicitud => solicitud.nomProyecto); break;
                case "proy_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.nomProyecto); break;
                case "Real": solicitudes = solicitudes.OrderBy(solicitud => solicitud.realizadoPor); break;
                case "real_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.realizadoPor); break;
                case "Est": solicitudes = solicitudes.OrderBy(solicitud => solicitud.estado); break;
                case "est_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.estado); break;
                default: solicitudes = solicitudes; break;
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
            DateTime myDate = DateTime.ParseExact(fecha, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            modelo.solicitud = BD.Solicitud.Find(myDate, version, idRF, nomProy);
            modelo.versionReq = BD.HistVersiones.Find(version, idRF, nomProy);
            modelo.Requerimiento = BD.ReqFuncional.Find(idRF, nomProy);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.versionReq.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.versionReq.responsable2RF);
            ViewBag.userList = usuarios.ToList();
            return View(modelo);
        }
    }
}