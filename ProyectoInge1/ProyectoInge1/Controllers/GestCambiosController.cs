using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
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
        // GET: GestCambios
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detalles()
        {
            ModGestionCambios modelo = new ModGestionCambios();
            modelo.ModeloHistVersion = new HistVersiones();
            modelo.ModeloHistVersion.nomProyecto = "Inge I";
            modelo.ModeloHistVersion.idReqFunc = 3;
            modelo.ModeloHistVersion.versionRF = 1;
          //  modelo.ModeloHistVersion.fecha = BD.HistVersiones.Find(modelo.ModeloHistVersion.versionRF);
            return View(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(ModGestionCambios modelo)
        {
            return View();
        }
    }
}