using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class TipoCambioFormViewModel : BaseViewModel
    {
        [Display(Name = "Moneda origen")]
        [Required(ErrorMessage = "La moneda origen es obligatoria")]
        public int MonedaOrigenId { get; set; }

        [Display(Name = "Moneda destino")]
        [Required(ErrorMessage = "La moneda destino es obligatoria")]
        public int MonedaDestinoId { get; set; }

        [Required(ErrorMessage = "La tasa es obligatoria")]
        [Display(Name = "Tasa")]
        [Range(0.000001, double.MaxValue, ErrorMessage = "La tasa debe ser mayor que cero")]
        public decimal Tasa { get; set; }

        [Display(Name = "Fecha de actualización")]
        public DateTime FechaActualizacion { get; set; }

        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}
