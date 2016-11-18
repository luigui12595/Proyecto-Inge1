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
        public Solicitud solicitud { get; set; }
        public string NombreReq { get; set; }
        public ReqFuncional requerimiento{ get; set; }
        public Usuario UsuarioResponsable1 { get; set; } //Específico para obtener responsables de requerimiento al desoplegar los detalles  NO BORRAR
        public Usuario UsuarioResponsable2 { get; set; } //Específico para obtener responsables de requerimiento al desoplegar los detalles  NO BORRAR
        public Usuario UsuarioFuente { get; set; } //Específico para obtener la fuente el requerimiento al desplegar los detalles  NO BORRAR
        public List<Usuario> listadesarrolladores { get; set; }
        public List<Usuario> lista { get; set; }
        public List<ModGestionCambios> listaModelos { get; set; }
        public Proyecto Proyecto { get; set; }
        public List<Solicitud> listaSolicitud { get; set; }
        public ReqFuncional Requerimiento { get; set; }
        public Solicitud Solicitud { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
    }
}
