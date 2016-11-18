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
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();

        // GET: Gestion
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
            //modelo.listaSolicitud = versiones.ToList();
           // modelo.listaModelos= versiones.ToList();
            return View(versiones.ToList().ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Detalles(/*int id,int Ver*/)
        {
            int id = 3;
            int Ver = 1;
            ModGestionCambios modelo = new ModGestionCambios();
           /* modelo.Requerimiento = BD.ReqFuncional.Find(id);
            modelo.listaSolicitud = BD.Solicitud.ToList();*/
          /*  if ( ) {

            }*/
           /* var solicitudes = from SolCam in BD.Solicitud
                                 where SolCam.idReqFunc == id && SolCam.versionRF==Ver  // aquí va el parámetro recibido:  where rfunc.nomProyecto == parámetro.
                                 select SolCam;
            
            modelo.Solicitud = solicitudes*/
            /*modelo.listaSolicitud = BD.Solicitud.Find(id);
            modelo.listaUsuarios = BD.Usuario.ToList();
            if (modelo.proyecto.Usuario2.Count > 0 || !modelo.proyecto.Usuario2.Equals(null))
            {
                modelo.listaUsuariosProyecto = modelo.proyecto.Usuario2.ToList();
            }*/
            return View(modelo);

        }

        public ActionResult Create(int versionRF,int idReqFun,string nombProyecto)
        {
            ModGestionCambios modelo = new ModGestionCambios();
            modelo.requerimiento = new ReqFuncional();
            modelo.lista = new List<Usuario>();
            modelo.listadesarrolladores = BD.Usuario.ToList();
            modelo.solicitud = new Solicitud();
            modelo.solicitud.estado = "Pendiente";
            modelo.solicitud.fecha = DateTime.Now;
            modelo.solicitud.versionRF = Convert.ToInt16(versionRF); // tiene que ser un small int no se si funcionara Nixson del futuro recuerdese revisar.
            modelo.solicitud.idReqFunc = idReqFun;
            modelo.solicitud.nomProyecto = nombProyecto;
            modelo.requerimiento = BD.ReqFuncional.Find(idReqFun,modelo.solicitud.nomProyecto);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.requerimiento.responsable1);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.requerimiento.responsable2);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.requerimiento.fuente);
            modelo.solicitud.aprobadoPor = modelo.requerimiento.fuente;
            foreach (var item in modelo.listadesarrolladores) {
               string nombre= item.nombre + " " +item.apellidos;
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
            BD.Solicitud.Add(modelo.solicitud);
            BD.SaveChanges();
            return RedirectToAction("Index");
        }

    }


}