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
            ViewBag.VersSortParm = sortOrder == "Version" ? "version_desc" : "version";
            ViewBag.RealSortParm = sortOrder == "Real" ? "real_desc" : "real";
            ViewBag.EstSortParm = sortOrder == "Est" ? "est_desc" : "est";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var solicitudes = from solicitud in BD.Solicitud
                           select solicitud;
            var requerimientos = from requerimiento in BD.ReqFuncional
                                 select requerimiento;
            if (!String.IsNullOrEmpty(searchString))
            {
                solicitudes = solicitudes.Where(solicit => solicit.apellidos.Contains(searchString) || users.nombre.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    usuarios = usuarios.OrderByDescending(users => users.apellidos);
                    break;
                default:
                    usuarios = usuarios.OrderBy(users => users.apellidos);
                    break;
            }
            
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ModGestionCambios modelo = new ModGestionCambios();
            ViewBag.reqFuncList = modelo.listaRequerimientos;
            ViewBag.reqList = new SelectList(modelo.listaRequerimientos, "id", "nombre");
            modelo.listaUsuarios = usuarios.ToList();
            return View(usuarios.ToList().ToPagedList(pageNumber, pageSize));
        }
    }
}