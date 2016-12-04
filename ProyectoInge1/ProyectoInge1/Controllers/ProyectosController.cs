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
        /*Metodo para la pantalla principal del modulo de proyectos, donde mostrará el grid para el listado de proyectos
          @param sortOrder: Consiste en una hilera de caracteres que indica el orden en el que se realizara el ordenamiento de las hileras.
          @param currentFilter: Consiste en una hilera de caracteres que determina cual es el actual el actual estado de busqueda.
          @param searchString: Consiste en una hilera de caracteres para realizar una busqueda en el index.
          @param page: Consiste en un entero que determina el numero de pagina que se presentara.
          @return: retorna al mismo listado de proyectos*/
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
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ModProyectoInter modelo = new ModProyectoInter();
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyectos = BD.Proyecto.ToList();
            ViewBag.userList = modelo.listaUsuarios;
            return View(proyectos.ToList().ToPagedList(pageNumber, pageSize));
        }
        /*Metodo para la mostrar los detalles de un proyecto seleccionado
          @param id: Consiste en un string que determinará el proyecto del cual se quiere mostrar el detalle.
          @return: retorna la vista de detalle lista para mostrar*/
        public ActionResult Detalles(string id)
        {

            ModProyectoInter modelo = new ModProyectoInter();
            modelo.proyecto = BD.Proyecto.Find(id);
            modelo.listaUsuarios = BD.Usuario.ToList();
            if (modelo.proyecto.Usuario2.Count > 0 || !modelo.proyecto.Usuario2.Equals(null))
            {
                modelo.listaUsuariosProyecto = modelo.proyecto.Usuario2.ToList();
            }
            // bolsa de desarrolladores disponibles
            modelo.liderViejo = BD.Usuario.Find(modelo.proyecto.lider).cedula;
            var usuarios = from users in BD.Usuario
                           select users;
            var context = new ApplicationDbContext();
            var desarrolladores = from developer in context.Users
                                  where developer.Roles.Any(r => r.RoleId == "2")
                                  select developer;
            var DesarrolladoresNoLider = new List<Usuario>();
            var Developers = new List<Usuario>();
            var usersSelected = new List<Usuario>();
            var usersAvailable = new List<Usuario>();
            DesarrolladoresNoLider.Add(BD.Usuario.Find(modelo.proyecto.lider));
            foreach (var x in usuarios)
            { //Carga de desarrolladores no líderes
                foreach (var y in desarrolladores) {
                    if (x.id == y.Id) {
                        Developers.Add(x);
                        if (x.lider == false || x.lider == null) {
                            DesarrolladoresNoLider.Add(x);
                        }
                    }
                }
            }
            foreach (var developer in Developers) {//seleccionar los desarrolladores disponibles para guardarlos en la viewBag 
                if ( modelo.proyecto.lider != developer.cedula) {
                    if (modelo.listaUsuariosProyecto.Contains(developer)) {
                        usersSelected.Add(developer);
                    } else { usersAvailable.Add(developer); }
                }
            }
            ViewBag.Usuarios = usuarios.ToList();
            ViewBag.DesarrolladoresNL = DesarrolladoresNoLider.ToList();
            ViewBag.SelectOpts = new MultiSelectList(usersSelected.ToList(), "cedula", "names");
            ViewBag.AvailableOpts = new MultiSelectList(usersAvailable.ToList(), "cedula", "names");
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
        public async Task<ActionResult> Detalles(ModProyectoInter modelo, string[] selectedOpts)
        {
            if (ModelState.IsValid)
            {
                var proyect = BD.Proyecto.Include(u => u.Usuario2)
                    .Single(u => u.nombre == modelo.proyecto.nombre);
                List<Usuario> usersSelected = new List<Usuario>();
                if (selectedOpts != null) {
                    foreach (var selected in selectedOpts)
                        usersSelected.Add(BD.Usuario.Find(selected));
                    foreach (var user in proyect.Usuario2.ToList())
                        if (!usersSelected.Any(u => u.cedula == user.cedula))
                            proyect.Usuario2.Remove(user);
                    foreach (var selectedUser in usersSelected)
                        if (!proyect.Usuario2.Any(u => u.cedula == selectedUser.cedula))
                            proyect.Usuario2.Add(selectedUser);
                } else {
                    foreach (var user in proyect.Usuario2.ToList())
                        proyect.Usuario2.Remove(user);
                }
                proyect.Usuario2.Add(BD.Usuario.Find(modelo.proyecto.lider));
                modelo.liderProyecto = BD.Usuario.Find(modelo.proyecto.lider);
                var oldLeader = BD.Usuario.Find(modelo.liderViejo); 
                oldLeader.lider = false;
                modelo.liderProyecto.lider = true; //Cambio del estatus del desarrollador a líder
                BD.Entry(modelo.liderProyecto).State = EntityState.Modified;
                BD.Entry(oldLeader).State = EntityState.Modified;
                BD.Entry(proyect).CurrentValues.SetValues(modelo.proyecto);
                BD.SaveChanges();
                return RedirectToAction("Index");  //Redireccionamiento al listado de proyectos
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return RedirectToAction("Create");
            }
        }

        /*Método que carga los recursos necesarios para crear un proyecto, desarrolladores dispoonibles para el proyecto, clientes...
          @return: retorna la vista con los elementos para crear un proyecto*/
        public ActionResult Create()
        {
            var usuarios = from users in BD.Usuario //Carga de usuarios de la base
                           select users;
            var context = new ApplicationDbContext();
            var desarrolladores = from developer in context.Users //Carga de desarrolladores
                                  where developer.Roles.Any(r => r.RoleId == "2")
                                  select developer; 
            ModProyectoInter model = new ModProyectoInter(); 
            var DesarrolladoresNoLider = new List<Usuario>(); //Lista de desarrolladores que no son lideres
            var proyecto = new Proyecto();
            var proyUsers = proyecto.Usuario2; //Desarrolladores del nuevo proyecto
            var usersSelected = new List<Usuario>(); 
            var usersAvailable = new List<Usuario>(); // Desarrolladores disponibles para el nuevo proyecto
            var userLider = new List<Usuario>(); //Lider del proyecto
            foreach ( var x in usuarios) { //Carga de desarrolladores no líderes
                foreach ( var y in desarrolladores) {
                    if ( x.id == y.Id ) {                    
                        usersAvailable.Add(x);
                        if ( x.lider == false || x.lider == null ) {
                            DesarrolladoresNoLider.Add(x);
                        }                          
                    }
                }
            }
            /*foreach ( var developer in DesarrolladoresNoLider ) //Carga de desarrolladores disponibles
            {
                if (proyUsers.Contains(developer)) { usersSelected.Add(developer); }
                else { usersAvailable.Add(developer); }
            }*/
            ViewBag.Usuarios = usuarios.ToList();
            ViewBag.DesarrolladoresNL = DesarrolladoresNoLider.ToList();
            //ViewBag.Leader = new SelectList(userLider.ToList(), "cedula", "names"); 
            ViewBag.SelectOpts = new MultiSelectList( usersSelected.ToList(), "cedula", "names" );
            ViewBag.AvailableOpts = new MultiSelectList( usersAvailable.ToList(), "cedula", "names" ); 
            return View();
        }

        /*Recibe los datos del proyecto asignados en la vista para agregarlo a la base de datos incluyendo
          los desarrolladores asignados, modifica la base de datos al crear proyectos
          @param modelo: Modelo de proyecto que contiene toda la información requerida para crear proyecto
          @param selectedOpts: recibe los numeros de cédula de los desarrollladores seleccionados para asignarlos 
          al proyecto
          @liderValue: recibe el número de cédula del líder asignado al proyecto
          @return: retorna al listado de proyectos*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( ModProyectoInter modelo, string[] selectedOpts )
        {           
            if ( ModelState.IsValid ) {
                modelo.proyecto.Usuario2 = new List<Usuario>();
                modelo.proyecto.Usuario2.Add(BD.Usuario.Find(modelo.proyecto.lider));
                if (selectedOpts != null) {
                    foreach (var developer in selectedOpts) { //asignacion de desarrolladores al proyecto
                        var proyDeveloper = BD.Usuario.Find(developer);
                        modelo.proyecto.Usuario2.Add(proyDeveloper);
                    }
                }
                modelo.liderProyecto = BD.Usuario.Find( modelo.proyecto.lider );
                modelo.liderProyecto.lider = true; //Cambio del estatus del desarrollador a líder
                BD.Entry(modelo.liderProyecto).State = EntityState.Modified; 
                BD.Proyecto.Add( modelo.proyecto );
                BD.SaveChanges();
                return RedirectToAction("Index");  //Redireccionamiento al listado de proyectos
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return RedirectToAction("Create");
            }
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

