using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ProyectoInge1.Models
{
    public class ModUsuarioInter
    {
        public Usuario modeloUsuario { get; set; }
        public Telefono modeloTelefono1 { get; set; }
        public Telefono modeloTelefono2 { get; set; }
        public Proyecto modeloProyecto { get; set; }
        public ApplicationUser appUser { get; set; }
        public List<ApplicationUser> appUserList { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
        public List<Telefono> listaTelefono { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
        public List<IdentityRole> listaRoles { get; set; }
        
    }
}
