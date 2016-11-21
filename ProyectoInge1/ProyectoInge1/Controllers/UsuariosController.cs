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
    public class UsuariosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        ApplicationDbContext context = new ApplicationDbContext();
        private bool revisarPermisos(string permiso)
        {
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var listaPermisos = BD.Permiso;
            var permisoID = 1;
            foreach (var perm in listaPermisos) {
                if (perm.descripcion == permiso) {
                    permisoID = perm.id;
                }
            }
            var listaRoles = BD.NetRolesPermiso.Where(m => m.idPermiso == permisoID).ToList().Select(n => n.idNetRoles);
            bool userRol = listaRoles.Contains(rol.RoleId);
            return userRol;
        }

        // GET: Usuarios
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!revisarPermisos("Index de usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Rol" ? "rol_desc" : "Rol";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var usuarios = from users in BD.Usuario
                           select users;
            if (!String.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(users => users.apellidos.Contains(searchString) || users.nombre.Contains(searchString));
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
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.listaUsuarios = usuarios.ToList();
            return View(usuarios.ToList().ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Eliminar(string id)
        {
            if (!revisarPermisos("Eliminar Usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Usuario");
            }
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(ModUsuarioInter modelo)
        {
            var usuario = BD.Usuario.Find(modelo.modeloUsuario.cedula);
            BD.Entry(usuario).State = EntityState.Deleted;
            BD.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Detalles(string id)
        {

            if (!revisarPermisos("Detalles de Usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Usuario");
            }
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();
            modelo.Role = "admin";
            if (1 <= modelo.listaTelefono.Count) { 
                modelo.modeloTelefono1 = modelo.listaTelefono.ElementAt(0);

            }
            if (1 < modelo.listaTelefono.Count)
            {
                modelo.modeloTelefono2 = modelo.listaTelefono.ElementAt(1);
           
            }
            return View(modelo);  
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Detalles(ModUsuarioInter modelo)
        {

            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
            BD.Entry(modelo.modeloUsuario).State = EntityState.Modified;
            var id = modelo.modeloUsuario.cedula;
            var roleId = modelo.Role;
            var role = await RoleManager.FindByIdAsync(roleId);
            await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, role.Name);
            modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();
            for(int i = 0; i <modelo.listaTelefono.Count; i++){ 
                BD.Entry(modelo.listaTelefono.ElementAt(i)).State = EntityState.Deleted;
            }
            if (modelo.modeloTelefono1.numero!= null)
            {
                modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
                BD.Telefono.Add(modelo.modeloTelefono1);
                BD.SaveChanges();

            }
            if (modelo.modeloTelefono2.numero != null)
            {
                modelo.modeloTelefono2.usuario = modelo.modeloUsuario.cedula;
                BD.Telefono.Add(modelo.modeloTelefono2);
                BD.SaveChanges();

            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModUsuarioInter modelo)
        {
            if (!revisarPermisos("Crear Usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Usuario");
            }

            if (ModelState.IsValid)
            {

                var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();


                var password = modelo.pass;
                var user = new ApplicationUser { UserName = modelo.modeloUsuario.correo, Email = modelo.modeloUsuario.correo };
                var result = await UserManager.CreateAsync(user, password);
    
              if (result.Succeeded)
                    {
                        modelo.modeloUsuario.id = user.Id;

                        BD.Usuario.Add(modelo.modeloUsuario);
                        BD.SaveChanges();

                        if (modelo.modeloTelefono1.numero != null)
                        {
                            modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
                            BD.Telefono.Add(modelo.modeloTelefono1);
                        }
                        if (modelo.modeloTelefono2.numero != null)
                        {
                            modelo.modeloTelefono2.usuario = modelo.modeloUsuario.cedula;
                            BD.Telefono.Add(modelo.modeloTelefono2);
                        }
                        BD.SaveChanges();

                        var roleId = modelo.Role;
                        var role = await RoleManager.FindByIdAsync(roleId);
                        var result2 = await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, role.Name);

                       
                    

                }

               return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return View(modelo);
            }
        }
    }
}