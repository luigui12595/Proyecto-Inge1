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

        /*Método que posibilita el despliegue de un listado de solicitudes de cambio para requerimientos funcionales
          @param sortOrder: Orden de despliegue de las solicitudes en el listado  
          @param currentFilter: Filtro de busqueda
          @param searchString: Nuevo filtro de búsqueda
          @param page: numero de página actual del listado*/
        public ActionResult Solicitudes(string sortOrder, string currentFilter, string searchString, int? page)
        {
            /*if (!revisarPermisos("Index de usuario") )
            {
                 this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }*/

            ViewBag.CurrentSort = sortOrder; 
            ViewBag.ReqSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; // ordenamiento según la columna seleccionada en el listado
            ViewBag.ProySortParm = sortOrder == "Proy" ? "proy_desc" : "Proy";
            ViewBag.VersSortParm = sortOrder == "Vers" ? "version_desc" : "Vers";
            ViewBag.RazonSortParm = sortOrder == "Razon" ? "razon_desc" : "Razon";
            ViewBag.RealSortParm = sortOrder == "Real" ? "real_desc" : "Real";
            ViewBag.EstSortParm = sortOrder == "Est" ? "est_desc" : "Est";
            if (searchString != null) { page = 1; } //Primera pagina de despliegue de los resultados de la búsqueda
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var solicitudes = from solicitud in BD.Solicitud //Seleccion de las solicitudes de todos los requerimientos en la base
                              join req in BD.ReqFuncional on solicitud.idReqFunc equals req.id
                              select solicitud;
            var requerimientos = from requerimiento in BD.ReqFuncional //Carga de los requerimientos
                                 select requerimiento;
            var usuarios = from usuario in BD.Usuario //Carga de usuarios
                           select usuario;
            ModGestionCambios modelo = new ModGestionCambios();
            if (!String.IsNullOrEmpty(searchString)) //Carga de las solicitudes de acuerdo a resultados de la búsqueda
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
                case "Razon": solicitudes = solicitudes.OrderBy(solicitud => solicitud.versionRF); break;
                case "razon_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.versionRF); break;
                case "Est": solicitudes = solicitudes.OrderBy(solicitud => solicitud.estado); break;
                case "est_desc": solicitudes = solicitudes.OrderByDescending(solicitud => solicitud.estado); break;
                default: solicitudes = solicitudes.OrderBy(sol => sol.nombreRF); break;
            }    
            int pageSize = 10; //Numero de elementos por página
            int pageNumber = (page ?? 1);  //Número de página
            //modelo.listaRequerimientos = requerimientos.ToList();
            //modelo.listaUsuarios = usuarios.ToList();
            ViewBag.reqFuncList = requerimientos.ToList();
            ViewBag.userList = usuarios.ToList();
            //ViewBag.reqList = new SelectList(modelo.listaRequerimientos, "id", "nombre");
            modelo.listaSolicitudes = solicitudes.ToList();
            return View(solicitudes.ToList().ToPagedList(pageNumber, pageSize)); //Retorna la lista de solicitudes de acuerdo a la paginación y criterios de búsqueda selleccionados
        }

        /*Método que despliega los detalles de una varsión del requerimiento así como los detalles de una solicitud hecha para cambios en esa versión del requerimiento
         @param id: Contiene los valores necesarioso llaves primarias para realizar la búsqueda de la solicitud a detallar en la base de datos así como el usuario que está 
         observando consultandol los detalles de dicha solicitud*/ 
        public ActionResult Details(string id)
        {
            /*if (!revisarPermisos("Detalles de Usuario"))
            {
                return RedirectToAction("Index", "Usuario");
            }*/
            var usuarios = from usuario in BD.Usuario //Lista de usuarios ordenados por cédula
                           orderby usuario.cedula
                           select usuario;
            ModGestionCambios modelo = new ModGestionCambios();
            string[] parameters = id.Split('~'); //Separación de los parámetros necesarios provistos
            short version = Convert.ToInt16(parameters[0]); //Versión del requerimiento
            int idRF = Convert.ToInt32(parameters[1]); //Id del requerimiento
            string nomProy = parameters[2]; //Nombre del proyecto al cual pertenece el requerimiento
            string fecha = parameters[3].Replace('-', ':').Replace('_', '-') + "." + parameters[5]; //Fecha de realización de la solicitud de cambio
            string currentUser = parameters[4]; //Usuario consultando los detalles
            var userView = from user in BD.Usuario //Búsqueda del usuario en la base
                           where currentUser == user.id
                           select user;
            modelo.userInView = userView.ToList().First();
            bool? lider = modelo.userInView.lider; //Verifica si quien consulta los detalles del usuariuo es el líder del proyecto correspondiente
            DateTime myDate = DateTime.ParseExact(fecha, "dd-MM-yyyy HH:mm:ss.fff", System.Globalization.CultureInfo.InvariantCulture);
            modelo.Solicitud = BD.Solicitud.Find(myDate, version, idRF, nomProy);
            modelo.versionReq = BD.HistVersiones.Find(version, idRF, nomProy);
            modelo.Requerimiento = BD.ReqFuncional.Find(idRF, nomProy);
            modelo.Proyecto = BD.Proyecto.Find(nomProy);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.Solicitud.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.Solicitud.responsable2RF);
            modelo.ancientState = modelo.Solicitud.estado;
            ViewBag.userList = usuarios.ToList();
            modelo.versionReq.versionRF = modelo.Solicitud.versionRF;
            modelo.versionReq.fecha = modelo.Solicitud.fecha;
            modelo.versionReq.versionRF = modelo.Solicitud.versionRF;
            modelo.versionReq.razon = modelo.Solicitud.razon;
            modelo.versionReq.realizadoPor = modelo.Solicitud.realizadoPor;
            modelo.versionReq.idReqFunc = modelo.Solicitud.idReqFunc;
            modelo.versionReq.nomProyecto = modelo.Solicitud.nomProyecto;
            modelo.versionReq.nombreRF = modelo.Solicitud.nombreRF;
            modelo.versionReq.sprintRF = modelo.Solicitud.sprintRF;
            modelo.versionReq.moduloRF = modelo.Solicitud.moduloRF;
            modelo.versionReq.fechaInicialRF = modelo.Solicitud.fechaInicialRF;
            modelo.versionReq.fechaFinalRF = modelo.Solicitud.fechaFinalRF;
            modelo.versionReq.observacionesRF = modelo.Solicitud.observacionesRF;
            modelo.versionReq.descripcionRF = modelo.Solicitud.descripcionRF;
            modelo.versionReq.esfuerzoRF = modelo.Solicitud.esfuerzoRF;
            modelo.versionReq.prioridadRF = modelo.Solicitud.prioridadRF;
            modelo.versionReq.imagenRF = modelo.Solicitud.imagenRF;
            modelo.versionReq.responsable1RF = modelo.Solicitud.responsable1RF;
            modelo.versionReq.responsable2RF = modelo.Solicitud.responsable2RF;
            modelo.Proyecto = BD.Proyecto.Find(nomProy);
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyUsuarios = modelo.Proyecto.Usuario2.ToList();
            var D = modelo.listaUsuarios.Intersect(modelo.listaProyUsuarios);
            modelo.listaUsuarioView = D.ToList();
            ViewBag.Lista = modelo.listaUsuarioView;
            return View(modelo);
        }

        /*Método que realiza cambios en la base de datos dependiendo de el estado asignado a la solicitud por el líder del proyecto al que se realizó la solicitud,
         Si la olicitud fue aprobada entonces se crea una nueva versión del requerimiento funcional en cualquier otro estado seleccionado para la solicitud sól cambia 
         esta propiedad en la misma
         @param modelo: recibe el modelo con 
         los datos necesarios para cambiar el estado de una solicitud ys de ser necesario crear una nueva versión de requerimiento funcional*/ 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(ModGestionCambios modelo)
        {
            /*if (!revisarPermisos("Crear Usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Usuario");
            }*/
            BD.Entry(modelo.Solicitud).State = EntityState.Modified; //Modificación del estado de la soliocitud en la base
            BD.SaveChanges();
            if (modelo.Solicitud.estado == "Aprobada" && modelo.ancientState != "Aprobada") //Si el estado de la solicitud es cambiado a aceptada se crea una nueva versión
            {
                int count = (from version in BD.HistVersiones //Conteo de el número de versiones de un requerimiento funcional para asignar el número de versión a la nueva
                            where version.idReqFunc == modelo.Solicitud.idReqFunc
                            select version).ToList().Count;
                modelo.versionReq.versionRF += Convert.ToInt16(count+1); //# identificador de la nueva versión
                modelo.versionReq.fecha = DateTime.Now; //Fecha de creación de la nueva versión
                BD.HistVersiones.Add(modelo.versionReq); //Insertado de la nueva versión en la base
                BD.SaveChanges();
            }
            return RedirectToAction("Solicitudes"); //Redireccionamiento al listado de solicitudes
        }

        public ActionResult Details_Hist(string id)
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
            modelo.versionReq = BD.HistVersiones.Find(version, idRF, nomProy);
            modelo.Requerimiento = BD.ReqFuncional.Find(idRF, nomProy);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.versionReq.realizadoPor);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.versionReq.responsable1RF);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.versionReq.responsable2RF);
            return View(modelo);
        }

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
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



