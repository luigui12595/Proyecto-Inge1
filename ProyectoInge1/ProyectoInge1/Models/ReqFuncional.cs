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
    
    public partial class ReqFuncional
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReqFuncional()
        {
            this.CriterioAceptacion = new HashSet<CriterioAceptacion>();
            this.GestionCambios = new HashSet<GestionCambios>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public Nullable<byte> sprint { get; set; }
        public Nullable<byte> modulo { get; set; }
        public string estado { get; set; }
        public Nullable<System.DateTime> fechaInicial { get; set; }
        public Nullable<System.DateTime> fechaFinal { get; set; }
        public string observaciones { get; set; }
        public string descripcion { get; set; }
        public Nullable<short> esfuerzo { get; set; }
        public Nullable<short> prioridad { get; set; }
        public byte[] imagen { get; set; }
        public string fuente { get; set; }
        public string responsable1 { get; set; }
        public string responsable2 { get; set; }
        public string nomProyecto { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CriterioAceptacion> CriterioAceptacion { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GestionCambios> GestionCambios { get; set; }
        public virtual Proyecto Proyecto { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Usuario1 { get; set; }
        public virtual Usuario Usuario2 { get; set; }
    }
}
