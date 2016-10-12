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
using static ProyectoInge1.Models.ModRolesInter;
using System.Web.UI.WebControls;

namespace ProyectoInge1.Controllers
{
    public class RolesController : Controller
    {

        ApplicationDbContext context;
        BD_IngeGrupo4Entities1 BD;  
        public RolesController()
        {
            context = new ApplicationDbContext();
            BD = new BD_IngeGrupo4Entities1();
        }


        private bool revisarPermisos(string permiso)
        {
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var permisoID = BD.Permiso.Where(m => m.descripcion == permiso).First().id;
            var listaRoles = BD.NetRolesPermiso.Where(m => m.idPermiso == permisoID).ToList().Select(n => n.idNetRoles);
            bool userRol = listaRoles.Contains(rol.RoleId);
            return userRol;
        }

        // GET: Roles
        public ActionResult Index()
        {
            /*if (!revisarPermisos("Gestionar Permisos"))
            {
                //this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }*/

           
            ModRolesInter modelo = new ModRolesInter();
            modelo.listaPermisos = BD.Permiso.ToList();
            modelo.listaRoles = context.Roles.ToList();
            modelo.rol_permisos = BD.NetRolesPermiso.ToList();
            modelo.rolPermisoId = new List<ModRolesInter.Relacion_Rol_Permiso>();

            foreach (var rol in modelo.listaRoles)
            {
                foreach (var permiso in modelo.listaPermisos)
                {
                    NetRolesPermiso rol_nuevo = new NetRolesPermiso();
                    rol_nuevo.idPermiso= permiso.id;
                    rol_nuevo.idNetRoles = rol.Id;
                    bool exists = false;
                    foreach (var rol_permiso in modelo.rol_permisos)
                    {
                        if (rol_permiso.idPermiso == permiso.id && rol.Id == rol_permiso.idNetRoles)
                        {
                            exists = true;
                        }
                    }
                    modelo.rolPermisoId.Add(new ModRolesInter.Relacion_Rol_Permiso(rol.Id, permiso.id, exists));
                }
            }

            return View(modelo);   
        }
        //POST: Roles_Permisos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ModRolesInter model)
        {
            var rol_permisos = BD.NetRolesPermiso.ToList();

            foreach (var rol_permiso in rol_permisos)
            {

                BD.Entry(rol_permiso).State = System.Data.Entity.EntityState.Deleted;
            }
            BD.SaveChanges();

            foreach (var relacion_rol_permiso in model.rolPermisoId)
            {
                if (relacion_rol_permiso.valor)
                {
                    var rolPermisosEntry = new NetRolesPermiso();
                    rolPermisosEntry.idPermiso = (short)relacion_rol_permiso.permiso;
                    rolPermisosEntry.idNetRoles = relacion_rol_permiso.rol;
                    BD.NetRolesPermiso.Add(rolPermisosEntry);
                }
            }
            BD.SaveChanges();

            //this.AddToastMessage("Permisos Guardados", "Se han logrado asignar los permisos a sus respectivos roles correctamente.", ToastType.Success);
            return RedirectToAction("Index", "Home");
        }
    }

}
