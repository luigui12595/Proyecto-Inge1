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

    public partial class Usuario
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuario()
        {
            this.HistVersiones = new HashSet<HistVersiones>();
            this.HistVersiones1 = new HashSet<HistVersiones>();
            this.HistVersiones2 = new HashSet<HistVersiones>();
            this.HistVersiones3 = new HashSet<HistVersiones>();
            this.Proyecto = new HashSet<Proyecto>();
            this.Proyecto1 = new HashSet<Proyecto>();
            this.ReqFuncional = new HashSet<ReqFuncional>();
            this.ReqFuncional1 = new HashSet<ReqFuncional>();
            this.ReqFuncional2 = new HashSet<ReqFuncional>();
            this.Solicitud = new HashSet<Solicitud>();
            this.Solicitud1 = new HashSet<Solicitud>();
            this.Solicitud2 = new HashSet<Solicitud>();
            this.Telefono = new HashSet<Telefono>();
            this.Proyecto2 = new HashSet<Proyecto>();
        }
        public string names { get { return nombre + " " + apellidos; } } //Para desplegar nombre completo en listas

        [StringLength(9)]
        [Required(ErrorMessage = "La c�dula es un campo requerido.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "La c�dula solo puede estar compuesta por n�meros")]
        public string cedula { get; set; }

        [StringLength(15)]
        [Required(ErrorMessage = "El nombre es un campo requerido.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "El nombre solo puede estar compuesto por letras")]
        public string nombre { get; set; }

        [StringLength(40)]
        [Required(ErrorMessage = "Apellidos es un campo requerido.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]+$", ErrorMessage = "Apellidos solo pueden estar compuesto por letras")]
        public string apellidos { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string correo { get; set; }
        public string id { get; set; }
        public Nullable<bool> lider { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistVersiones> HistVersiones { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistVersiones> HistVersiones1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistVersiones> HistVersiones2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HistVersiones> HistVersiones3 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proyecto> Proyecto { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proyecto> Proyecto1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReqFuncional> ReqFuncional { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReqFuncional> ReqFuncional1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReqFuncional> ReqFuncional2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Solicitud> Solicitud { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Solicitud> Solicitud1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Solicitud> Solicitud2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Telefono> Telefono { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Proyecto> Proyecto2 { get; set; }
    }
}