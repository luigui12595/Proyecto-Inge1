using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;


namespace ProyectoInge1.Models
{
    public class ModUsuarioInter
    {
        public Usuario modeloUsuario { get; set; }
        public Telefono modeloTelefono { get; set; }
        public Proyecto modeloProyecto { get; set; }
        public ApplicationUser appUser { get; set; }
        public List<ApplicationUser> appUserList { get; set; }
        public List<Usuario> listaUsuarios { get; set; }
        public List<Telefono> listaTelefono { get; set; }
        public List<Proyecto> listaProyectos { get; set; }
        public List<IdentityRole> listaRoles { get; set; }
        
    }
}

/*@for (int j = 0; j < Model.appUserList.Count; j++)
                    {
                        if (Model.listaUsuarios.ElementAt(i).id.Equals(Model.appUserList.ElementAt(j).Id))
                        {
                            if (Model.appUserList.ElementAt(j).Id.Equals(Model.appUserList.ElementAt(j).Roles.ElementAt(j).UserId))
                            {
                                if (Model.appUserList.ElementAt(j).Roles.ElementAt(j).RoleId.Equals(Model.listaRoles.ElementAt(j).Id))
                                {
                                    @Html.DisplayFor(modelItem => Model.listaRoles.ElementAt(j).Name)
                                    @Html.Raw(" ")
                                }
                            }
                        }
                    }
*/