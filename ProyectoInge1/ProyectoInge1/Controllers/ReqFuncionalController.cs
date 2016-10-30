using System;
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
    public class ReqFuncionalController : Controller
    {

        /*// GET: ReqFuncional
        public ActionResult Index()
        {
            return View();
        }*/

        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();

        // GET: Usuarios
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Rol" ? "rol_desc" : "Rol";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var requerimientos = from rfunc in BD.ReqFuncional
                                 where rfunc.nomProyecto == "Telecomunicaciones"  // aquí va el parámetro recibido:  where rfunc.nomProyecto == parámetro.
                                 select rfunc;
            if (!String.IsNullOrEmpty(searchString))
            {
                requerimientos = requerimientos.Where(rfunc => rfunc.nombre.Contains(searchString) || rfunc.nombre.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    requerimientos = requerimientos.OrderByDescending(rfunc => rfunc.nombre);
                    break;
                default:
                    requerimientos = requerimientos.OrderBy(rfunc => rfunc.nombre);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ModReqFuncionalInter modelo = new ModReqFuncionalInter();
            modelo.listaRequerimientos = requerimientos.ToList();
            return View(requerimientos.ToList().ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create(/*string NombProy*/)
        {
            string id = "Aseguradora";

            ModReqFuncionalInter RQ = new ModReqFuncionalInter();
            //RQ.ReqUsuario = BD.Usuario.Find(id);
           /* RQ.ReqFunUsu=
           */ var usuarios =
                          from usersP in BD.Proyecto
                          //where usersP.Proyecto = NombProy
                          select usersP;

            //usuarios = usuarios.Where(x => x.nombre == NombProy);
            usuarios = usuarios.Where(x => x.nombre == id);
            // return View(usuarios.ToList() );*/
            return View(/*RQusuarios.ToList()*/);
        }

    }
}