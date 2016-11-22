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
            modelo.listaUsuarios = BD.Usuario.ToList();
            ViewBag.userList = modelo.listaUsuarios;
            //modelo.listaSolicitud = versiones.ToList();
            // modelo.listaModelos= versiones.ToList();
           // ViewBag.Desarrolladores = new SelectList(model.DesarrolladoresNoLider, "cedula", "names");
            return View(versiones.ToList().ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Detalles(/*int id,int Ver*/)
        {
            int id = 3;
            int Ver = 1;
            DateTime Momento = DateTime.Parse("2016-11-18 13:00:53.000");
            string Proy = "Inge I";
            ModGestionCambios modelo = new ModGestionCambios();
            modelo.listaUsuarios= BD.Usuario.ToList();
            var VReq = BD.Solicitud.ToList();
            foreach(var VRF in VReq){
                if (DateTime.Compare(VRF.fecha, Momento) == 0){
                    if (VRF.idReqFunc==id) {
                         if (VRF.versionRF==Ver) {
                            modelo.Solicitud = VRF;
                        }
                    }
                }
            }
            modelo.Proyecto = BD.Proyecto.Find(Proy);
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyUsuarios = modelo.Proyecto.Usuario2.ToList();
           
            var D = modelo.listaUsuarios.Intersect(modelo.listaProyUsuarios);
            modelo.listaUsuarioView = D.ToList();
            ViewBag.Lista = modelo.listaUsuarioView;
            return View(modelo);

        }
        /* public ActionResult Index()
         {
             ModGestionCambios GestionC = new ModGestionCambios();
             GestionC.listaProyectos = BD.Proyecto.ToList();
             return View(GestionC);

         }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(ModGestionCambios modelo ) {
            if (ModelState.IsValid) {
                // BD.Solicitud.Find(modelo.Solicitud.fecha,modelo.Solicitud.versionRF,modelo.Solicitud.nomProyecto,modelo.Solicitud.idReqFunc);
                if (modelo.Solicitud.estado=="Pendiente") {
                    string s = modelo.Solicitud.fecha.ToString();

                    // DateTime T = DateTime.ParseExact(s, "dd-MM-yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                    BD.Entry(modelo.Solicitud).State = EntityState.Modified;
                    //BD.Entry(modelo.Solicitud).Property(M => M.razon).IsModified = true;
                    // BD.Solicitud.Add(modelo.Solicitud);
                    BD.SaveChanges();
                }
            }

            int id = 3;
            int Ver = 1;
            DateTime Momento = DateTime.Parse("2016-11-18 13:00:53.000");
            string Proy = "Inge I";
            ModGestionCambios mod = new ModGestionCambios();
            mod.listaUsuarios = BD.Usuario.ToList();
            var VReq = BD.Solicitud.ToList();
            foreach (var VRF in VReq)
            {
                if (DateTime.Compare(VRF.fecha, Momento) == 0)
                {
                    if (VRF.idReqFunc == id)
                    {
                        if (VRF.versionRF == Ver)
                        {
                            mod.Solicitud = VRF;
                        }
                    }
                }
            }
            mod.Proyecto = BD.Proyecto.Find(Proy);
            mod.listaUsuarios = BD.Usuario.ToList();
            mod.listaProyUsuarios = mod.Proyecto.Usuario2.ToList();
            return View(mod);
        }

        public ActionResult Create(int? versionRF,int? idReqFunc,string nomProyecto)
        {

            return View();
        }
        }


}