using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ProyectoInge1.Models
{

    public class ModReqFuncionalInter
    {
        public List<ReqFuncional> listaRequerimientos { get; set; }
       // public List<ReqFuncional> ModeloReqFun { get; set; }
        public List<Usuario> listaUsuario { get; set; }
        public Usuario Usuarios { get; set; }
        public Proyecto proyecto { get; set; }
        public ReqFuncional Requerimientos { get; set; }
        public List<CriterioAceptacion> listaCriterios { get; set; }   
        public CriterioAceptacion criterios { get; set; }
        public List<Proyecto> listaProyecto { get; set; }
        public List<Usuario> UsuariosSistema { get; set; }

    }
    }