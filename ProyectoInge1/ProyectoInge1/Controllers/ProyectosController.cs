using System;
using System.Collections.Generic;
using ProyectoInge1.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Diagnostics;
using PagedList;
using System.Text;

namespace ProyectoInge1.Controllers
{
    public class ProyectosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();

        ApplicationDbContext context = new ApplicationDbContext();
        /* private bool revisarPermisos(string permiso)
        {
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var permisoID = BD.Permiso.Where(m => m.descripcion == permiso).First().id;
            var listaRoles = BD.NetRolesPermiso.Where(m => m.idPermiso == permisoID).ToList().Select(n => n.idNetRoles);
            bool userRol = listaRoles.Contains(rol.RoleId);
            return userRol;
        }
        */

        // GET: Proyectos
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            // if (!revisarPermisos("Listado de Permisos"))
            //{
            // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
            // return RedirectToAction("Index", "Home");
            //}
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.InitDateSortParm = sortOrder == "InitDate" ? "initDate_desc" : "InitDate";
            ViewBag.FinishDateSortParm = sortOrder == "FinishDate" ? "finishDate_desc" : "FinishDate";
            ViewBag.LeaderSortParm = sortOrder == "Leader" ? "leader_desc" : "Leader";
            ViewBag.StateSortParm = sortOrder == "State" ? "state_desc" : "StateDate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            var proyectos = from project in BD.Proyecto
                            select project;

            if (!String.IsNullOrEmpty(searchString))
            {
                proyectos = proyectos.Where(projects => projects.nombre.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    proyectos = proyectos.OrderByDescending(projects => projects.nombre);
                    break;
                case "InitDate":
                    proyectos = proyectos.OrderBy(projects => projects.fechaInicio);
                    break;
                case "initDate_desc":
                    proyectos = proyectos.OrderByDescending(projects => projects.fechaInicio);
                    break;
                case "FinishDate":
                    proyectos = proyectos.OrderBy(projects => projects.fechaFinal);
                    break;
                case "finishDate_desc":
                    proyectos = proyectos.OrderByDescending(projects => projects.fechaFinal);
                    break;
                case "Leader":
                    proyectos = proyectos.OrderBy(projects => projects.lider);
                    break;
                case "leader_desc":
                    proyectos = proyectos.OrderByDescending(projects => projects.lider);
                    break;
                case "State":
                    proyectos = proyectos.OrderBy(projects => projects.estado);
                    break;
                case "state_desc":
                    proyectos = proyectos.OrderByDescending(projects => projects.estado);
                    break;
                default:
                    proyectos = proyectos.OrderBy(projects => projects.nombre);
                    break;
            }
            int pageSize = 2;
            int pageNumber = (page ?? 1);
            ModProyectoInter modelo = new ModProyectoInter();
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyectos = BD.Proyecto.ToList();
            ViewBag.userList = modelo.listaUsuarios;
            return View(proyectos.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Detalles(string id)
        {
            
            ModProyectoInter modelo = new ModProyectoInter();
            modelo.proyecto = BD.Proyecto.Find(id);
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaUsuariosProyecto = modelo.proyecto.Usuario2.ToList();
            return View(modelo);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Detalles(ModProyectoInter modelo)
        {

            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            var usuarios = from users in BD.Usuario
                           select users;
            ModProyectoInter model = new ModProyectoInter();
            model.listaUsuarios = usuarios.ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModProyectoInter modelo/*, string id*/)
        {
            if (ModelState.IsValid)
            {
                //var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                //var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
                //var user = new ApplicationUser { UserName = modelo.modeloUsuario.correo, Email = modelo.modeloUsuario.correo };
                //var result = await UserManager.CreateAsync(user, password);
                
            //if (result.Succeeded)
            //{
                //modelo.modeloUsuario.id = user.Id;

                BD.Proyecto.Add(modelo.proyecto);
                BD.SaveChanges();

                /*if (modelo.modeloTelefono1.numero != null)
                {
                    modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
                    BD.Telefono.Add(modelo.modeloTelefono1);
                }
                if (modelo.modeloTelefono2.numero != null)
                {
                    modelo.modeloTelefono2.usuario = modelo.modeloUsuario.cedula;
                    BD.Telefono.Add(modelo.modeloTelefono2);
                }*/
                BD.SaveChanges();

                //var roleId = modelo.Role;
                //var role = await RoleManager.FindByIdAsync(roleId);
                //var result2 = await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, role.Name);

                /*if (result2.Succeeded)
                {
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(modelo.modeloUsuario.id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = modelo.modeloUsuario.id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(modelo.modeloUsuario.id, "Ingreso al sistema", "Su contraseña temporal asignada es " + password + "\n" + "Por favor confirme su cuenta pulsando click <a href=\"" + callbackUrl + "\">aquí</a>");

                }*/

            //}
            return RedirectToAction("Index");
        }
        else
        {
            ModelState.AddModelError("", "Debe completar toda la información necesaria.");
            return View(modelo);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(bool confirm, string Proyecto)
        {
            if (confirm == true)
            {

                var ProyectoB = BD.Proyecto.Find(Proyecto/*modelo.proyecto.nombre*//*id*/);
                //Condicion de estado
                if (ProyectoB.estado == "Terminado" || ProyectoB.estado == "Cancelado")
                {
                    //solicitar una confirmacion para eliminar proyecto 
                    BD.Entry(ProyectoB).State = EntityState.Deleted;
                    BD.SaveChanges();
                }
                else
                {
                    //Desplegar mensaje de imposible eliminar el proyecto
                    return View();
                }

            }


            //Fin condicion estado
            //  if (ProyectoB. ) { }
            /*var usuario = BD.Usuario.Find(modelo. );
            BD.Entry(usuario).State = EntityState.Deleted;
            BD.SaveChanges();*/
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar2(/**/bool confirm, string Proyecto/**//* string id*/)
        {
            //id ="pp";
            if (confirm == true)
            {

                var ProyectoB = BD.Proyecto.Find(Proyecto/*modelo.proyecto.nombre*//*id*/);
                //Condicion de estado
                if (ProyectoB.estado == "Terminado" || ProyectoB.estado == "Cancelado")
                {
                    //solicitar una confirmacion para eliminar proyecto 
                    BD.Entry(ProyectoB).State = EntityState.Deleted;
                    BD.SaveChanges();
                }
                else
                {
                    //Desplegar mensaje de imposible eliminar el proyecto
                    return View();
                }

            }


            //Fin condicion estado
            //  if (ProyectoB. ) { }
            /*var usuario = BD.Usuario.Find(modelo. );
            BD.Entry(usuario).State = EntityState.Deleted;
            BD.SaveChanges();*/
            return RedirectToAction("Index");
        }
    }
}

