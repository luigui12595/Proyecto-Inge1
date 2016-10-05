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
        public ActionResult Index( string sortOrder )
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Proy" ? "proy_desc" : "Proy";
            var usuarios = from users in BD.Usuario
                           select users;
            var proyectos = from proy in BD.Proyecto
                            select proy;
            switch (sortOrder)
            {
                case "name_desc":
                    usuarios = usuarios.OrderByDescending(users => users.apellidos);
                    break;
                case "Proy":
                    proyectos = proyectos.OrderBy(proy => proy.nombre);
                    break;
                case "proy_desc":
                    proyectos = proyectos.OrderByDescending(proy => proy.nombre);
                    break;
                default:
                    usuarios = usuarios.OrderBy(users => users.apellidos);
                    break;
            }
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.listaUsuarios = usuarios.ToList();
            modelo.listaProyectos = proyectos.ToList();
            return View(modelo);
        }
    }
}