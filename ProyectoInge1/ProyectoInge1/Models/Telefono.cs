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

    public partial class Telefono
    {
        public string usuario { get; set; }
        [StringLength(8)]
        [Display(Name = "Tel�fono:")]
        [RegularExpression(@"^[0-9]{8,8}$", ErrorMessage = "El n�mero s�lo puede contener 8 n�meros")]
        public string numero { get; set; }
    
        public virtual Usuario Usuario1 { get; set; }
    }
}
