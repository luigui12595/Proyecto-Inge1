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
    }
}