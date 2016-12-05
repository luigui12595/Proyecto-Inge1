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
    using System.ComponentModel.DataAnnotations;

    public partial class HistVersiones
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HistVersiones()
        {
            this.Solicitud = new HashSet<Solicitud>();
        }
        [Display(Name = "Version")]
        public short versionRF { get; set; }
        [Display(Name = "Fecha")]
        public System.DateTime fecha { get; set; }
        [Display(Name = "Razon")]
        public string razon { get; set; }
        [Display(Name = "Realizado por")]
        public string realizadoPor { get; set; }
        [Display(Name = "ID")]
        public int idReqFunc { get; set; }
        [Display(Name = "Nombre Proyecto")]
        public string nomProyecto { get; set; }
        [Display(Name = "Nombre Requerimiento")]
        public string nombreRF { get; set; }
        [Display(Name = "Sprint")]
        public Nullable<byte> sprintRF { get; set; }
        [Display(Name = "Modulo")]
        public Nullable<byte> moduloRF { get; set; }
        [Display(Name = "Fecha Inicio")]
        public Nullable<System.DateTime> fechaInicialRF { get; set; }
        [Display(Name = "Fecha Final")]
        public Nullable<System.DateTime> fechaFinalRF { get; set; }
        [Display(Name = "Observaciones")]
        public string observacionesRF { get; set; }
        [Display(Name = "Descripcion")]
        public string descripcionRF { get; set; }
        [Display(Name = "Esfuerzo")]
        public Nullable<short> esfuerzoRF { get; set; }
        [Display(Name = "Prioridad")]
        public Nullable<short> prioridadRF { get; set; }
        [Display(Name = "Imagen")]
        public byte[] imagenRF { get; set; }
        [Display(Name = "Responsable")]
        public string responsable1RF { get; set; }
        [Display(Name = "Responsable")]
        public string responsable2RF { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Solicitud> Solicitud { get; set; }
        public virtual ReqFuncional ReqFuncional { get; set; }
        public virtual Usuario Usuario { get; set; }
        public virtual Usuario Usuario1 { get; set; }
        public virtual Usuario Usuario2 { get; set; }
        public virtual Usuario Usuario3 { get; set; }
    }
}
