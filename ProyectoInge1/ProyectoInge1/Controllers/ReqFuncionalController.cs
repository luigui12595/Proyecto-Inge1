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
            string param1 = this.Request.QueryString["Proyecto"];
            var requerimientos = from rfunc in BD.ReqFuncional
                                 where rfunc.nomProyecto == param1  // aquí va el parámetro recibido:  where rfunc.nomProyecto == parámetro.
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

        public ActionResult Create(string NombProy)
        {
            string id = "Aseguradora";

            ModReqFuncionalInter RQ = new ModReqFuncionalInter();
            var req= from usersP in BD.ReqFuncional
                     select usersP;
            req = req.Where(x => x.nombre == id);
            RQ.listaRequerimientos = req.ToList();
            /* if (RQ.listaRequerimientos.Count == 0)
             {
                 RQ.Requerimientos.id = 0;
             }
             else {
               Int32 d = RQ.listaRequerimientos.Count;
                Int16 d2 = (Int16) (d + 1);
                 d= (Int32)(d2);
                 RQ.Requerimientos.id = (Int16)(d);
             }*/

            /* var proy = from usersP in BD.Proyecto
                            select usersP;

             proy = proy.Where(x => x.nombre == id);

             RQ.proyecto = proy;

             for (int j=0; j<RQ.listaProyecto.Count;j++) {

             }*/
            RQ.UsuariosSistema = BD.Usuario.ToList();
            RQ.proyecto = BD.Proyecto.Find(id);
            RQ.listaUsuario = RQ.proyecto.Usuario2.ToList();
            // return View(usuarios.ToList() );*/
            return View(RQ);
        }

        public ActionResult Details(short id)
        {
            /*if (!revisarPermisos("Detalles de Usuario"))
            {
                return RedirectToAction("Index", "Usuario");
            }*/
            ModReqFuncionalInter modelo = new ModReqFuncionalInter();
            string nombre = "Aseguradora";
            modelo.Requerimiento = BD.ReqFuncional.Find(id, nombre);
            modelo.UsuarioFuente = BD.Usuario.Find(modelo.Requerimiento.fuente);
            modelo.UsuarioResponsable1 = BD.Usuario.Find(modelo.Requerimiento.responsable1);
            modelo.UsuarioResponsable2 = BD.Usuario.Find(modelo.Requerimiento.responsable2);
            return View(modelo);

        }


        /* [HttpPost]
         [ValidateAntiForgeryToken]
         public async Task<ActionResult> Detalles(ModReqFuncionalInter modelo)
         {
             BD.Entry(modelo.modeloUsuario).State = EntityState.Modified;
             var id = modelo.modeloUsuario.cedula;
             var roleId = modelo.Role;
             var role = await RoleManager.FindByIdAsync(roleId);
             // en ves de a;adir es modificar
             await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, role.Name);
             modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();
             for (int i = 0; i < modelo.listaTelefono.Count; i++)
             {
                 BD.Entry(modelo.listaTelefono.ElementAt(i)).State = EntityState.Deleted;
             }
             if (modelo.modeloTelefono1.numero != null)
             {
                 modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
                 BD.Telefono.Add(modelo.modeloTelefono1);
                 BD.SaveChanges();

             }
             if (modelo.modeloTelefono2.numero != null)
             {
                 modelo.modeloTelefono2.usuario = modelo.modeloUsuario.cedula;
                 BD.Telefono.Add(modelo.modeloTelefono2);
                 BD.SaveChanges();

             }
             return RedirectToAction("Index");
         }
         */

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModReqFuncionalInter modelo)
        {
            var NReqFun = from RF in BD.ReqFuncional select RF;
            var NombreP = modelo.Requerimientos.nomProyecto;         
            BD.ReqFuncional.Add(modelo.Requerimientos);
            BD.SaveChanges();
            List<ReqFuncional> LR;
            LR = BD.ReqFuncional.ToList();
            var idReq = LR.Last().id;
            if (modelo.values != null) {
                String[] substrings = modelo.values.Split('|');
                foreach (var substring in substrings)
                {
                   CriterioAceptacion mod= new CriterioAceptacion();
                    mod.idReqFunc = idReq;
                    mod.nomProyecto = NombreP;
                    mod.criterio = substring;
                    BD.CriterioAceptacion.Add(mod);
                    BD.SaveChanges();
                }
            }
                /* if (ModelState.IsValid)
                 {
                     //var idRF;
                     var NReqFun = from RF in BD.ReqFuncional select RF;
                     // NReqFun = NReqFun.Where(x => x.nombre == modelo.RequerimientosF.nomProyecto).Max(x => x.id);
                     BD.ReqFuncional.Add(modelo.RequerimientosF);
                     BD.SaveChanges();
                 }
                 else
                 {
                     ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                     return View(modelo);
                 }*/
                return View();
        }
        //return View();
        //return true;
    }
}