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
        public Usuario modeloUsuario { get; set; }
        public Telefono modeloTelefono1 { get; set; }
        public Telefono modeloTelefono2 { get; set; }
    //    public AspUsers modeloRol { get; set; }
        public Proyecto modeloProyecto { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
        public List<Telefono> listaTelefono { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un rol")]
        public string Role { get; set; }
        public DateTime Date { get; internal set; }
    } 
}
