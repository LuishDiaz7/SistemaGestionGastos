using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class GastoFormViewModel : BaseViewModel
    {
        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "La moneda es obligatoria")]
        public int MonedaId { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Display(Name = "Monto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(255, ErrorMessage = "La descripción debe tener máximo {1} caracteres")]
        public string Descripcion { get; set; }

        [Display(Name = "Lugar")]
        [StringLength(100, ErrorMessage = "El lugar debe tener máximo {1} caracteres")]
        public string Lugar { get; set; }

        [Display(Name = "Es recurrente")]
        public bool EsRecurrente { get; set; }

        public IEnumerable<SelectListItem> CategoriasDisponibles { get; set; }
        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}
