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
    public class UsuariosController : Controller
    {
        BD_IngeGrupo4Entities1 BD = new BD_IngeGrupo4Entities1();
        // PARA LA GENERACIÓN DE CONTRASEÑA
        static string alphaCaps = "QWERTYUIOPASDFGHJKLZXCVBNM";
        static string alphaLow = "qwertyuiopasdfghjklzxcvbnm";
        static string numerics = "1234567890";
        static string special = "@#$";
        //create another string which is a concatenation of all above
        string allChars = alphaCaps + alphaLow + numerics + special;
        Random r = new Random();

        public string GenerateStrongPassword(int length)
        {
            String generatedPassword = "";

            if (length < 4)
                throw new Exception("Number of characters should be greater than 4.");

            // Generate four repeating random numbers are postions of
            // lower, upper, numeric and special characters
            // By filling these positions with corresponding characters,
            // we can ensure the password has atleast one
            // character of those types
            int pLower, pUpper, pNumber, pSpecial;
            string posArray = "0123456789";
            if (length < posArray.Length)
                posArray = posArray.Substring(0, length);
            pLower = getRandomPosition(ref posArray);
            pUpper = getRandomPosition(ref posArray);
            pNumber = getRandomPosition(ref posArray);
            pSpecial = getRandomPosition(ref posArray);


            for (int i = 0; i < length; i++)
            {
                if (i == pLower)
                    generatedPassword += getRandomChar(alphaCaps);
                else if (i == pUpper)
                    generatedPassword += getRandomChar(alphaLow);
                else if (i == pNumber)
                    generatedPassword += getRandomChar(numerics);
                else if (i == pSpecial)
                    generatedPassword += getRandomChar(special);
                else
                    generatedPassword += getRandomChar(allChars);
            }
            return generatedPassword;
        }

        private string getRandomChar(string fullString)
        {
            return fullString.ToCharArray()[(int)Math.Floor(r.NextDouble() * fullString.Length)].ToString();
        }

        private int getRandomPosition(ref string posArray)
        {
            int pos;
            string randomChar = posArray.ToCharArray()[(int)Math.Floor(r.NextDouble()
                                           * posArray.Length)].ToString();
            pos = int.Parse(randomChar);
            posArray = posArray.Replace(randomChar, "");
            return pos;
        }
        // END OF "PARA LA GENERACIÓN DE CONTRASEÑA"

        ApplicationDbContext context = new ApplicationDbContext();
        private bool revisarPermisos(string permiso)
        {
            string userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var rol = context.Users.Find(userID).Roles.First();
            var permisoID = BD.Permiso.Where(m => m.descripcion == permiso).First().id;
            var listaRoles = BD.NetRolesPermiso.Where(m => m.idPermiso == permisoID).ToList().Select(n => n.idNetRoles);
            bool userRol = listaRoles.Contains(rol.RoleId);
            return userRol;
        }

        // GET: Usuarios
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (!revisarPermisos("Detalles de Usuario"))
            {
                // this.AddToastMessage("Acceso Denegado", "No tienes el permiso para gestionar Roles!", ToastType.Warning);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Rol" ? "rol_desc" : "Rol";
            if (searchString != null) { page = 1; }
            else { searchString = currentFilter; }
            ViewBag.CurrentFilter = searchString;
            var usuarios = from users in BD.Usuario
                           select users;
            if (!String.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(users => users.apellidos.Contains(searchString) || users.nombre.Contains(searchString));
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
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.listaUsuarios = usuarios.ToList();
            return View(usuarios.ToList().ToPagedList(pageNumber, pageSize));
        }


        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Eliminar(string id)
        {
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Eliminar(ModUsuarioInter modelo)
        {
            modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
            modelo.modeloTelefono1.usuario = modelo.modeloUsuario.cedula;
            BD.Entry(modelo.modeloTelefono1).State = EntityState.Deleted;
            BD.Entry(modelo.modeloTelefono2).State = EntityState.Deleted;
            BD.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Detalles(string id)
        {
            ModUsuarioInter modelo = new ModUsuarioInter();
            modelo.modeloUsuario = BD.Usuario.Find(id);
            modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();
            if (1 <= modelo.listaTelefono.Count) { 
                modelo.modeloTelefono1 = modelo.listaTelefono.ElementAt(0);

            }
            if (1 < modelo.listaTelefono.Count)
            {
                modelo.modeloTelefono2 = modelo.listaTelefono.ElementAt(1);

            }
            
            return View(modelo);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detalles(ModUsuarioInter modelo)
        {
            BD.Entry(modelo.modeloUsuario).State = EntityState.Modified;
            var id = modelo.modeloUsuario.cedula;
            modelo.listaTelefono = BD.Telefono.Where(x => x.usuario == id).ToList();
            for(int i = 0; i <modelo.listaTelefono.Count; i++){ 
                BD.Entry(modelo.listaTelefono.ElementAt(i)).State = EntityState.Deleted;
            }
            if (modelo.modeloTelefono1.numero!= null)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ModUsuarioInter modelo)
        {
            if (ModelState.IsValid)
            {

                var UserManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var RoleManager = Request.GetOwinContext().Get<ApplicationRoleManager>();


                // var password = Membership.GeneratePassword(6, 1);
                var password = GenerateStrongPassword(8);
                var user = new ApplicationUser { UserName = modelo.modeloUsuario.correo, Email = modelo.modeloUsuario.correo };
                var result = await UserManager.CreateAsync(user, password);
    
              if (result.Succeeded)
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
                ModelState.AddModelError("", "Debe completar toda la información necesaria.");
                return View(modelo);
            }
        }
    }
}