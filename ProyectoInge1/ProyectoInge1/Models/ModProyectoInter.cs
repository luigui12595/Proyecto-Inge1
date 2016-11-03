using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoInge1.Models
{
    public class ModProyectoInter
    {
        public Usuario usuario { get; set; }
        public Proyecto proyecto { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
        public ReqFuncional requerimiento { get; set; }
        public List<ReqFuncional> listaRequerimientos { get; set; }
        public List<CriterioAceptacion> listaCriterio { get; set; }
        public CriterioAceptacion criterio { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<Usuario> listaUsuariosProyecto { get; set; }

    }
}