//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProyectoInge1.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class HistVersiones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HistVersiones()
        {
            this.Solicitud = new HashSet<Solicitud>();
        }
    
        public short versionRF { get; set; }
        public System.DateTime fecha { get; set; }
        public string razon { get; set; }
        public string realizadoPor { get; set; }
        public int idReqFunc { get; set; }
        public string nomProyecto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Solicitud> Solicitud { get; set; }
        public virtual ReqFuncional ReqFuncional { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
