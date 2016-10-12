using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoInge1.Models
{
    public class ModRolesInter
    {
        public class Relacion_Rol_Permiso
        {
            public Relacion_Rol_Permiso()
            {
            }

            public Relacion_Rol_Permiso(string id, int codigo, bool exists)
            {
                this.rol = id;
                this.permiso = codigo;
                this.valor = exists;
            }

            public string rol { get; set; }
            public int permiso { get; set; }
            public bool valor { get; set; }
        }

        public Permiso modeloPermiso { get; set; }
        public IdentityRole Rol { get; set; }
        public NetRolesPermiso rol_permiso { get; set; }
        public List<Permiso> listaPermisos { get; set; }
        public List<IdentityRole> listaRoles { get; set; }
        public List<NetRolesPermiso> rol_permisos { get; set; }
        public List<Relacion_Rol_Permiso> rolPermisoId { get; set; }

    }

    
}