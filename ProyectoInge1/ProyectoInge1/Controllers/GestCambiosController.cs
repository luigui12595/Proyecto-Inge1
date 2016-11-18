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