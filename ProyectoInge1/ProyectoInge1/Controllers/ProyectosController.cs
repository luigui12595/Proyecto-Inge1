﻿using System;
using System.Collections.Generic;
using ProyectoInge1.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace ProyectoInge1.Controllers
{
    public class ProyectosController : Controller
    {
        // GET: Proyectos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModUsuarioInter modelo)
        {
            /*if (ModelState.IsValid)
            {
                
                var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
                var user = new ApplicationUser { UserName = modelo.modeloUsuario.correo, Email = modelo.modeloUsuario.correo };
                var result = await UserManager.CreateAsync(user, password);
                */
                /*if (result.Succeeded)
                {
                    modelo.modeloUsuario.id = user.Id;

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

                    var roleId = modelo.Role;
                    var role = await RoleManager.FindByIdAsync(roleId);
                    var result2 = await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, role.Name);

                    if (result2.Succeeded)
                    {
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(modelo.modeloUsuario.id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = modelo.modeloUsuario.id, code = code }, protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(modelo.modeloUsuario.id, "Ingreso al sistema", "Su contraseña temporal asignada es " + password + "\n" + "Por favor confirme su cuenta pulsando click <a href=\"" + callbackUrl + "\">aquí</a>");

                    }

                }
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");*/
                return View(modelo);
            //}
        }
    }
}