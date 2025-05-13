using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class MonedaViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "El código es obligatorio")]
        [Display(Name = "Código")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código debe tener exactamente 3 caracteres")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El nombre debe tener máximo {1} caracteres")]
        public string Nombre { get; set; }

        [Display(Name = "Símbolo")]
        [StringLength(5, ErrorMessage = "El símbolo debe tener máximo {1} caracteres")]
        public string Simbolo { get; set; }

        [Display(Name = "Activa")]
        public bool Activa { get; set; }
    }
}
