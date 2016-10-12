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
using PagedList;

namespace ProyectoInge1.Controllers
{
    public class UsuariosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();

        // GET: Usuarios
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Proy" ? "proy_desc" : "Proy";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var usuarios = from users in BD.Usuario
                           select users;
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
                default:
                    usuarios = usuarios.OrderBy(users => users.apellidos);
                    break;
            }
            int pageSize = 1;
            int pageNumber = (page ?? 1);
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.listaUsuarios = usuarios.ToList();
            return View(usuarios.ToList().ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Detalles(string id)
        {
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            //modelo.modeloTelefono1 = BD.Telefono.Find(id);
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
        public async Task<ActionResult> Create(ModUsuarioInter modelo)
        {
            if (ModelState.IsValid)
            {

                // AGREGADO
          //      var SignInManager = Request.GetOwinContext().Get<ApplicationSignInManager>();
                var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();
                //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                var password = Membership.GeneratePassword(6, 1);
                var user = new ApplicationUser { UserName = modelo.modeloUsuario.correo, Email = modelo.modeloUsuario.correo };
                var result = await UserManager.CreateAsync(user, password);
                BD.SaveChanges();
                modelo.modeloUsuario.id = user.Id;

                /*        if (result.Succeeded)
                          {
                              BD.SaveChanges();
                              modelo.modeloUsuario.id = user.Id;

                              // 
                              // INFORMACIÓN IMPORTANTE PARA ENVIO DE CONTRASEÑA POR CORREO
                              // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                              // Send an email with this link
                              // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                              // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                              // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                          }
               */

                BD.Usuario.Add(modelo.modeloUsuario);
                BD.SaveChanges();   // Verificar caracteristicas de constraint FK_NetUsers_usuario -> si afecta cambiar a NO y NO.

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

 
                var roleId = "1"; // Cambiar esto por el retorno del combobox - Si del combobox se recibe un nombre omitir siguiente línea y dar la variable en lugar de role.Name
                var role = await RoleManager.FindByIdAsync(roleId);
                var result2 = await UserManager.AddToRoleAsync(modelo.modeloUsuario.id, role.Name);

                if (result2.Succeeded) {
                    BD.SaveChanges();
                }


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