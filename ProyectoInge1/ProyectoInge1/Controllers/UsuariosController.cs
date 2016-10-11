using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using ProyectoInge1.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProyectoInge1.Controllers
{
    public class UsuariosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        // GET: Usuarios
        public ActionResult Index( string sortOrder, string searchString )
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Proy" ? "proy_desc" : "Proy";
            var usuarios = from users in BD.Usuario
                           select users;
            var proyectos = from proy in BD.Proyecto
                            select proy;
            if (!String.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(users => users.apellidos.Contains(searchString)
                                       || users.nombre.Contains(searchString));
            }
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

        public string GetRoleToUsers(string userId)
        {
            var context = new ApplicationDbContext();
            var role = context.Roles.Where(x => x.Users.Select(y => y.UserId).Contains(userId)).ElementAt(0).Name;
            return role;
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Detalles(string id)
        {
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            //modelo.modeloTelefono1.numero = BD.Usuario.Find(id).cedula;
            //modelo.modeloTelefono2 = BD.Telefono.Find(id);
            return View(modelo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(ModUsuarioInter modelo)
        {
            BD.Entry(modelo.modeloUsuario).State = EntityState.Modified;
            BD.SaveChanges();
            return View(modelo);
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