using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class PerfilViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres", MinimumLength = 2)]
        public string Nombre { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Fecha de registro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaRegistro { get; set; }

        [Display(Name = "Moneda predeterminada")]
        public int? MonedaPredeterminadaId { get; set; }

        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}
