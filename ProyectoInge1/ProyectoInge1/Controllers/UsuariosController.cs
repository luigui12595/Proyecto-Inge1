using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProyectoInge1.Models;

namespace ProyectoInge1.Controllers
{
    public class UsuariosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        // GET: Usuarios
        public ActionResult Index()
        {
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.listaUsuarios = BD.Usuario.ToList();
            modelo.listaProyectos = BD.Proyecto.ToList();
            return View(modelo);
        }

        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModUsuarioInter modelo)
        {
            if (ModelState.IsValid)
            {
                BD.Usuario.Add(modelo.modeloUsuario);
                BD.SaveChanges();

                if (modelo.modeloTelefono1.numero != null)
                {
                    modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
                    BD.Telefono.Add(modelo.modeloTelefono1);
                }
                if (modelo.modeloTelefono2.numero != null)
                {
                    modelo.modeloTelefono2.usuario = modelo.modeloUsuario.cedula;
                    BD.Telefono.Add(modelo.modeloTelefono2);
                }
                BD.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return View(modelo);
            }
        }
    }
}