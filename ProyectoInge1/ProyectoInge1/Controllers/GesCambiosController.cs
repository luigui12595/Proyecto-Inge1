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
    public class GesCambiosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        // GET: Gestion
        public ActionResult Index()
        {
            /*ModGestionCambios GestionC = new ModGestionCambios();
            GestionC.listaProyectos = BD.Proyecto.ToList();
            return View(GestionC);*/
            return View();
        }
    }
}