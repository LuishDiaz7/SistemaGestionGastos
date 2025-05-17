using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar y validar los datos de un tipo de cambio en formularios de creación y edición.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class TipoCambioFormViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la moneda de origen para el tipo de cambio.
        /// Este campo es obligatorio.
        /// </summary>
        [Display(Name = "Moneda origen")]
        [Required(ErrorMessage = "La moneda origen es obligatoria")]
        public int MonedaOrigenId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda de destino para el tipo de cambio.
        /// Este campo es obligatorio.
        /// </summary>
        [Display(Name = "Moneda destino")]
        [Required(ErrorMessage = "La moneda destino es obligatoria")]
        public int MonedaDestinoId { get; set; }

        /// <summary>
        /// Obtiene o establece la tasa de conversión del tipo de cambio (cuántas unidades de moneda destino por una de origen).
        /// Este campo es obligatorio y debe ser un valor positivo mayor que cero.
        /// </summary>
        [Required(ErrorMessage = "La tasa es obligatoria")]
        [Display(Name = "Tasa")]
        [Range(0.000001, double.MaxValue, ErrorMessage = "La tasa debe ser mayor que cero")] // Permite valores muy pequeños.
        public decimal Tasa { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora de la última actualización de la tasa de cambio.
        /// </summary>
        [Display(Name = "Fecha de actualización")]
        // [DataType(DataType.DateTime)] // Podría añadirse si el formato de fecha/hora en la UI necesitara especificación.
        public DateTime FechaActualizacion { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de monedas disponibles para seleccionar como origen o destino en controles de lista.
        /// Esta propiedad se utiliza para la UI y no es parte de los datos enviados por el formulario POST.
        /// </summary>
        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}