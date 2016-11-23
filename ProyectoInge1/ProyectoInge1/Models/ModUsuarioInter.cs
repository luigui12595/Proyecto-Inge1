using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace ProyectoInge1.Models
{
    public class ModUsuarioInter
    {
        public class AplicationUser
        {
            public AplicationUser()
            {
            }

            public AplicationUser(string id, string rol)
            {
                this.userId = id;
                this.RolId = rol;
            }

            public string userId { get; set; }
            public string RolId { get; set; }
        }

        public Usuario modeloUsuario { get; set; }
        public Telefono modeloTelefono1 { get; set; }
        public Telefono modeloTelefono2 { get; set; }
        public Proyecto modeloProyecto { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
        public List<Telefono> listaTelefono { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public string Role { get; set; }
        public List<IdentityRole> listaRoles { get; set; }
        public List<ApplicationUser> listaUserRoles { get; set; }
        public DateTime Date { get; internal set; }
        public String pass { get; set; }
    } 
}
