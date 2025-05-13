using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace SggApp.Models
{
    public class PresupuestoFormViewModel : BaseViewModel
    {
        [Display(Name = "Categoría (opcional)")]
        public int? CategoriaId { get; set; }

        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "La moneda es obligatoria")]
        public int MonedaId { get; set; }

        [Required(ErrorMessage = "El límite es obligatorio")]
        [Display(Name = "Límite")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El límite debe ser mayor que cero")]
        public decimal Limite { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Notificar al (%)")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public int? NotificarAl { get; set; }

        public IEnumerable<SelectListItem> CategoriasDisponibles { get; set; }
        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}
