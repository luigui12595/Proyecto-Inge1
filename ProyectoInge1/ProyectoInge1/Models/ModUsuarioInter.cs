using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoInge1.Models
{
    public class ModUsuarioInter
    {
        public Usuario modeloUsuario { get; set; }
        public Telefono modeloTelefono { get; set; }
        public Proyecto modeloProyecto { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
        public List<Telefono> listaTelefono { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
     }
}
