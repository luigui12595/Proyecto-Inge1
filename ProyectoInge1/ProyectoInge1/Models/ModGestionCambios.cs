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
        public List<HistVersiones> listaCambios { get; set; }//Específico para obtener el listado de versiones de cambios
        public List<Proyecto> listaProyectos { get; set; }

        //public Solicitud solicitud { get; set; }
        public string NombreReq { get; set; }
        //public ReqFuncional requerimiento{ get; set; }
        public Usuario UsuarioResponsable1 { get; set; } //Específico para obtener responsables de requerimiento al desoplegar los detalles  NO BORRAR
        public Usuario UsuarioResponsable2 { get; set; } //Específico para obtener responsables de requerimiento al desoplegar los detalles  NO BORRAR
        public Usuario UsuarioFuente { get; set; } //Específico para obtener la fuente el requerimiento al desplegar los detalles  NO BORRAR
        public List<Usuario> listadesarrolladores { get; set; }
        public List<Usuario> lista { get; set; }
        public List<Solicitud> listaSolicitudes { get; set; }
        public List<ReqFuncional> listaRequerimientos { get; set; }
        public List<Usuario> listaUsuarios { get; set; }//Especifico para obtener los usuarios que son parte del sistema 
        //public Proyecto proyecto { get; set; }
        public HistVersiones ModeloHistVersion { get; set; }
        // public Proyecto proyecto { get; set; }

        public DateTime Inicio { get; set; }//Especifico para almacenar temporalmente una nueva fecha de inicio de requerimiento
        public DateTime Final { get; set; }//Especifico para almacenar temporalmente una nueva fecha de final de requerimiento
        public ReqFuncional Requerimiento { get; set; }
        public HistVersiones versionReq { get; set; }
        public List<ModGestionCambios> listaModelos { get; set; }
        public List<Usuario> listaProyUsuarios { get; set; }//Especifico para obtener los id de los usuarios que son parte de un proyecto especifico
        public List<Usuario> listaUsuarioView { get; set; }//Especifico para obtener la infomacion usuarios que son parte de un proyecto especifico
        public Proyecto Proyecto { get; set; }
        public List<Solicitud> listaSolicitud { get; set; }

        // public Solicitud Solicitud { get; set; }
        public Solicitud Solicitud { get; set; }
        public Usuario userInView { get; set; }
        public string ancientState { get; set; }
        public string alerta { get; set; }
    }
}