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
        public List<ReqFuncional> ModeloReqFun { get; set; }
        public List<Usuario> ReqUsuario { get; set; }
        public ReqFuncional Requerimiento { get; set; }
        public Usuario ReqFunUsu { get; set; }
        public ReqFuncional RequerimientosF { get; set; }
          

        }
    }