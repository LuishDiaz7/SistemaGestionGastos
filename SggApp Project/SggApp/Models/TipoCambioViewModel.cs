using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class TipoCambioViewModel : BaseViewModel
    {
        [Display(Name = "Moneda origen")]
        public int MonedaOrigenId { get; set; }

        [Display(Name = "Moneda origen")]
        public string MonedaOrigenCodigo { get; set; }

        [Display(Name = "Moneda destino")]
        public int MonedaDestinoId { get; set; }

        [Display(Name = "Moneda destino")]
        public string MonedaDestinoCodigo { get; set; }

        [Required(ErrorMessage = "La tasa es obligatoria")]
        [Display(Name = "Tasa")]
        [Range(0.000001, double.MaxValue, ErrorMessage = "La tasa debe ser mayor que cero")]
        public decimal Tasa { get; set; }

        [Display(Name = "Fecha de actualización")]
        public DateTime FechaActualizacion { get; set; }
    }
}
