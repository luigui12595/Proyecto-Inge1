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
        static string detailLink = "";
        static string alert = ""; 
        
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

        /*
           Inicializa la informacion necesaria para presentar detalles, modificar o eliminar una solicitud de cambio
           @param id: hilera de caracteres compuesta por la fecha de una solicitud, el requerimiento y el proyecto
                      al que perteneceria y una version de este requerimiento.               
           @return: Un modelo de solicitud de cambio que contiene la informacion correspondiente a este y un ViewBag 
                    que contiene una lista de usuarios relacionados a que participan de ese proyecto.
       */
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
            string fecha = parameters[3].Replace('-', ':').Replace('_', '-') + "." + parameters[5];
            string currentUser = parameters[4];
            var userView = from user in BD.Usuario
                           where currentUser == user.id
                           select user;
            modelo.userInView = userView.ToList().First();
            bool? lider = modelo.userInView.lider;
            DateTime myDate = DateTime.ParseExact(fecha, "dd-MM-yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            modelo.Solicitud = BD.Solicitud.Find(myDate, version, idRF, nomProy);
            modelo.versionReq = BD.HistVersiones.Find(version, idRF, nomProy);
            modelo.Requerimiento = BD.ReqFuncional.Find(idRF, nomProy);
            modelo.Proyecto = BD.Proyecto.Find(nomProy);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            /*modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.versionReq.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.versionReq.responsable2RF);
            */
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.Solicitud.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.Solicitud.responsable2RF);
            ViewBag.userList = usuarios.ToList();
            detailLink = id;
            modelo.Proyecto = BD.Proyecto.Find(nomProy);
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyUsuarios = modelo.Proyecto.Usuario2.ToList();
            var D = modelo.listaUsuarios.Intersect(modelo.listaProyUsuarios);
            modelo.listaUsuarioView = D.ToList();
            ViewBag.Lista = modelo.listaUsuarioView;
            return View(modelo);
        }

        /*
		    Contiene la informacion referente a la informacion necesaria para realizar algun cambio en la informacion de una 
            solicitud de cambio.
			@param modelo: Consiste en los datos ingresados por un usuario que corresponde a la informacion referente para la 
                          modificacion de una solicitud de cambio de una version de un requerimiento. 
            @param imagen1: Consiste en una imagen para almacenar en base.              
			@return: ningun valor pero devuelve al usuario al index de solicitudes
		*/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(ModGestionCambios modelo, HttpPostedFileBase imagen1)
        {
            if (!revisarPermisos("Modificar Solicitud"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Usuario");
            }

            if (modelo.Solicitud.estado == "Pendiente" || modelo.Solicitud.estado == "En Revision")
            {
                if (imagen1 != null)
                {
                    modelo.Solicitud.imagenRF = new byte[imagen1.ContentLength];
                    imagen1.InputStream.Read(modelo.Solicitud.imagenRF, 0, imagen1.ContentLength);
                }
                DateTime Prueba = new DateTime();
                if (modelo.Final.CompareTo(Prueba)!=0) {
                    modelo.Solicitud.fechaFinalRF = modelo.Final;
                }
                if (modelo.Inicio.CompareTo(Prueba) != 0)
                {
                    modelo.Solicitud.fechaInicialRF = modelo.Inicio;
                }
                BD.Entry(modelo.Solicitud).State = EntityState.Modified;
                    BD.SaveChanges();
                }
            
           
            if (modelo.Solicitud.estado == "Aprobada")
            {
                BD.Entry(modelo.Solicitud).State = EntityState.Modified;
                BD.SaveChanges();
                modelo.versionReq.versionRF += 1;
                BD.HistVersiones.Add(modelo.versionReq);
                BD.SaveChanges();
            }
                return RedirectToAction("Solicitudes");
        }

        /*
            Contiene la informacion referente a una versión.
            @param id: un string que contiene tres datos: el nombre del proyecto, el id del requerimiento funcional y el número de versión (concatenados); 
                        que corresponde a la información básica necesaria para desplegar la información sobre una versión específica.
            @return: Un modelo de gestión de cambios con toda la información de una versión de un requerimiento funcional.
        */
        public ActionResult Details_Hist(string id)
        {
            if (!revisarPermisos("Detalles de Version"))
            {
                return RedirectToAction("Index", "GestCambios");
            }
            var usuarios = from usuario in BD.Usuario
                           orderby usuario.cedula
                           select usuario;
            ModGestionCambios modelo = new ModGestionCambios();
            string[] parameters = id.Split('~');
            short version = Convert.ToInt16(parameters[0]);
            int idRF = Convert.ToInt32(parameters[1]);
            string nomProy = parameters[2];
            modelo.versionReq = BD.HistVersiones.Find(version, idRF, nomProy);
            modelo.Requerimiento = BD.ReqFuncional.Find(idRF, nomProy);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.versionReq.realizadoPor);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.versionReq.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.versionReq.responsable2RF);
            return View(modelo);
        }



        /*
            Crea un listado de todas las versiones que existen de un requerimiento funcional.
			@param sortOrder: Consiste en una hilera de caracteres que indica el orden en el que se realizara el ordenamiento de 
                              de las hileras
            @param currentFilter: Consiste en una hilera de caracteres que determina cual es el actual el actual estado de busqueda 
            @param searchString: Consiste en una hilera de caracteres para realizar una busqueda en el index 
            @param page: Consiste en un entero que determina el numero de paginas que se presentara               
			@return: Un modelo gestion de cambios con informacion de todas las versiones existentes.
		*/
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
            int pageSize = 10;
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


        public ActionResult Create(int versionRF, int idReqFunc, string nomProyecto)
        {
            ModGestionCambios modelo = new ModGestionCambios();
            modelo.Requerimiento = new ReqFuncional();
            modelo.lista = new List<Usuario>();
            modelo.listadesarrolladores = BD.Usuario.ToList();
            modelo.Solicitud = new Solicitud();
            modelo.Solicitud.estado = "Pendiente";
            modelo.Solicitud.fecha = DateTime.Now;
            modelo.Solicitud.versionRF = Convert.ToInt16(versionRF); // tiene que ser un small int no se si funcionara Nixson del futuro recuerdese revisar.
            modelo.Solicitud.idReqFunc = idReqFunc;
            modelo.Solicitud.nomProyecto = nomProyecto;
            modelo.Requerimiento = BD.ReqFuncional.Find(idReqFunc, modelo.Solicitud.nomProyecto);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.Requerimiento.responsable1);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.Requerimiento.responsable2);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            modelo.Solicitud.aprobadoPor = modelo.Requerimiento.fuente;
            foreach (var item in modelo.listadesarrolladores)
            {
                string nombre = item.nombre + " " + item.apellidos;
                item.apellidos = nombre;
                modelo.lista.Add(item);
            }
            ViewBag.desarrolladores = new SelectList(modelo.lista, "cedula", "apellidos");
            return View(modelo);
        }


        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModGestionCambios modelo)
        {
            BD.Solicitud.Add(modelo.Solicitud);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Eliminar(bool confirm, DateTime fecha, short version, int idReq, string nomPro)
        {

            if (confirm == true)
            {
                var Solicitud = BD.Solicitud.Find(fecha, version, idReq, nomPro);
                if (Solicitud.estado == "Pendiente")
                {
                    BD.Entry(Solicitud).State = EntityState.Deleted;
                    BD.SaveChanges();
                    return RedirectToAction("Solicitudes");
                }
                else {
                    alert = "El estado de la Solicitud debe de ser Pendiente para poder eliminarla";
                    var link = detailLink;
                    return RedirectToAction("Details",new {id = link});
                }
            }
            else
            {
                return RedirectToAction("Solicitudes");
            }
        }
    }
}



