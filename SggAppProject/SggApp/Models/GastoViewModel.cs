using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class GastoViewModel : BaseViewModel
    {
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        [Display(Name = "Categoría")]
        public string CategoriaNombre { get; set; }

        [Display(Name = "Moneda")]
        public int MonedaId { get; set; }

        [Display(Name = "Moneda")]
        public string MonedaCodigo { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Display(Name = "Monto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Lugar")]
        public string Lugar { get; set; }

        [Display(Name = "Es recurrente")]
        public bool EsRecurrente { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
