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

//http://stackoverflow.com/questions/10042608/passing-javascript-array-to-asp-net-mvc-controller.
//http://stackoverflow.com/questions/15782417/post-javascript-array-with-ajax-to-asp-net-mvc-controller

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
            if ( modelo.proyecto.Usuario2.Count > 0 || !modelo.proyecto.Usuario2.Equals(null) ) {
                modelo.listaUsuariosProyecto = modelo.proyecto.Usuario2.ToList();
            }
            // bolsa de desarrolladores disponibles
            List<Usuario> listadesarrolladores = new List<Usuario>();
            string RolDesarrollador = context.Roles.Where(m => m.Name == "Desarrollador").First().Id;
            
            ViewBag.desarrolladores = new SelectList(listadesarrolladores, "nombre", "apellidos");
            ViewBag.desarDisponibles = listadesarrolladores;
            return View(modelo);

        }

        public ActionResult Eliminar(bool confirm, string Proyecto)
        {
          
            if (confirm == true)
            {

                var ProyectoB = BD.Proyecto.Find(Proyecto);
                //Condicion de estado
                if (ProyectoB.estado == "Finalizado" || ProyectoB.estado == "Cancelado"|| ProyectoB.estado == "Suspendido")
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
                return RedirectToAction("Index"); 
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Detalles(ModProyectoInter modelo)
        {
            BD.Entry(modelo.proyecto).State = EntityState.Modified;
            BD.SaveChanges();

            //Agregar participantes
            if (modelo.listaUsuarios != null) {
                foreach (var item in modelo.listaUsuarios) {
                    //pUsuario.usuario = item.cedula;
                    
                }
            }
            return RedirectToAction("Index");
        }

        /*public bool UserIsDeveloper( string id, IQueryable<ApplicationUser> desarrolladores )
        {
            for (int i = 0; i < desarrolladores.Count(); i++)
                if (id == desarrolladores.ElementAt(i).Id)
                    return true;
            return false;
        }*/

        public ActionResult Create()
        {
            var usuarios = from users in BD.Usuario
                           select users;
            var context = new ApplicationDbContext();
            var desarrolladores = from developer in context.Users
                                  where developer.Roles.Any(r => r.RoleId == "2")
                                  select developer; 
            ModProyectoInter model = new ModProyectoInter();
            var DesarrolladoresNoLider = new List<Usuario>();
            var proyecto = new Proyecto();
            var proyUsers = proyecto.Usuario2;
            var usersSelected = new List<Usuario>();
            var usersAvailable = new List<Usuario>();
            var userLider = new List<Usuario>();
            foreach ( var x in usuarios) {
                foreach ( var y in desarrolladores) {
                    if ( x.id == y.Id && ( x.lider == false || x.lider == null ) ) {
                        DesarrolladoresNoLider.Add(x);
                    }
                }
            }
            foreach ( var developer in DesarrolladoresNoLider )
            {
                if (proyUsers.Contains(developer)) { usersSelected.Add(developer); }
                else { usersAvailable.Add(developer); }
            }
            ViewBag.Usuarios = usuarios.ToList();
            ViewBag.Desarrolladores = DesarrolladoresNoLider.ToList();
            ViewBag.Leader = new SelectList(userLider.ToList(), "cedula", "names"); 
            ViewBag.SelectOpts = new MultiSelectList( usersSelected.ToList(), "cedula", "names" );
            ViewBag.AvailableOpts = new MultiSelectList( usersAvailable.ToList(), "cedula", "names" );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( ModProyectoInter modelo, string[] selectedOpts, string[] liderValue )
        {
            if (selectedOpts != null)
            {
                modelo.proyecto.Usuario2 = new List<Usuario>();
                foreach ( var developer in selectedOpts )
                {
                    var proyDeveloper = BD.Usuario.Find( developer );
                    modelo.proyecto.Usuario2.Add( proyDeveloper );
                }
            }
            modelo.proyecto.lider = liderValue[0];
            /*if ( ModelState.IsValid ) {*/
                modelo.liderProyecto = BD.Usuario.Find( modelo.proyecto.lider );
                BD.Entry(modelo.liderProyecto).State = EntityState.Modified;
                BD.Proyecto.Add( modelo.proyecto );
                BD.SaveChanges();
                return RedirectToAction("Index");
            //}
            /*else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return RedirectToAction("Create");
            }*/
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar1(/*bool confirm, string Proyecto*/)
        {
            bool confirm = true;
            string Proyecto = "pruebaB";
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

