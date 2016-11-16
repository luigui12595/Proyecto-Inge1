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
        
        
    }
}
