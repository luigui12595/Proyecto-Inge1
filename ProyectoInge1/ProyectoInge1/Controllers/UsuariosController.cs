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

        /*Despliegue de los usuarios del sistema
         Parametros Recibidos: Ordenamiento, Filtro de búsqueda actual, Nuevo filtro, Página actual
         No modifica nada en el sistema*/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
           /* if (!revisarPermisos("Index de usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            } */

            ViewBag.CurrentSort = sortOrder; //Orden de usuarios en el index
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";  //Parámetro de ordenamiento por nombre
            ViewBag.DateSortParm = sortOrder == "Rol" ? "rol_desc" : "Rol";             //Por Rol
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;     //Filtro de búsqueda
            var usuarios = from users in BD.Usuario  //Carga de usuarios de la base
                           select users;
            if (!String.IsNullOrEmpty(searchString))
            {
                //Búsqueda de usuarios por nombre o apellidos
                usuarios = usuarios.Where(users => users.apellidos.Contains(searchString) || users.nombre.Contains(searchString));
            }
            switch (sortOrder) //Ordena de acuerdo al atributo de usuario seleccionado en la tabla de usuarios(columna seleccionada)
            {
                case "name_desc":  //Ordenamiento por apellidos
                    usuarios = usuarios.OrderByDescending(users => users.apellidos);
                    break;
                default:
                    usuarios = usuarios.OrderBy(users => users.apellidos);
                    break;
            }
            int pageSize = 10; //Número de usuarios dexplegados por página en el Index
            int pageNumber = (page ?? 1); //Número de página actual
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.listaUsuarios = usuarios.ToList(); //Asignación de usuarios al modelo
            return View(usuarios.ToList().ToPagedList(pageNumber, pageSize)); //Despliegue de usuarios según parámetros recibidos
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

        //Permite ver los detalles de los usuarios, y modificarlos
        public async Task<ActionResult> Detalles(string id)
        {
            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
            // verificar Permisos de usuario
            if (!revisarPermisos("Detalles de Usuario"))
            {
                return RedirectToAction("Index", "Usuario");
            }
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();


            modelo.Role = "Administrador";//await UserManager.FindByIdAsync(modelo.modeloUsuario.id);
            if (1 <= modelo.listaTelefono.Count) { 
                modelo.modeloTelefono1 = modelo.listaTelefono.ElementAt(0);

            }
            if (1 < modelo.listaTelefono.Count)
            {
                modelo.modeloTelefono2 = modelo.listaTelefono.ElementAt(1);
           
            }

            return View(modelo);  
        }
        // post detalles, cambia los datos de la base de datos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Detalles(ModUsuarioInter modelo)
        {
            var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
            BD.Entry(modelo.modeloUsuario).State = EntityState.Modified;
            BD.SaveChanges();
            var id = modelo.modeloUsuario.cedula;
            var roleId = modelo.Role;
            var role = await RoleManager.FindByIdAsync(roleId);
            //await UserManager.RemoveFromRoleAsync(modelo.modeloUsuario.id, role.Name);
            await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, modelo.Role);
            modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();
            for(int i = 0; i <modelo.listaTelefono.Count; i++){ 
                BD.Entry(modelo.listaTelefono.ElementAt(i)).State = EntityState.Deleted;
                BD.SaveChanges();
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