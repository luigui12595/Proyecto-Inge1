using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ProyectoInge1.Models
{
    public class ModGestionCambios
    {

        public List<HistVersiones> listaCambios { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
        public List<ModGestionCambios> listaModelos { get; set; }

        public List<Usuario> listaUsuarios { get; set; }
        public List<Usuario> listaProyUsuarios { get; set; }
        public List<Usuario> listaUsuarioView { get; set; }
        public Proyecto Proyecto { get; set; }
        public List<Solicitud> listaSolicitud { get; set; }
        public ReqFuncional Requerimiento { get; set; }
        public Solicitud Solicitud { get; set; }

    }
}
