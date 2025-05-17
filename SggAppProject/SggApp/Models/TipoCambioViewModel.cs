using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para presentar datos de Tipos de Cambio en la interfaz de usuario (ej. listados o detalles).
    /// Incluye la tasa de cambio y detalles básicos de las monedas de origen y destino.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class TipoCambioViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la moneda de origen para el tipo de cambio.
        /// </summary>
        [Display(Name = "Moneda origen")]
        public int MonedaOrigenId { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la moneda de origen para su visualización (ej. "USD").
        /// </summary>
        [Display(Name = "Moneda origen")]
        public string MonedaOrigenCodigo { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda de destino para el tipo de cambio.
        /// </summary>
        [Display(Name = "Moneda destino")]
        public int MonedaDestinoId { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la moneda de destino para su visualización (ej. "COP").
        /// </summary>
        [Display(Name = "Moneda destino")]
        public string MonedaDestinoCodigo { get; set; }

        /// <summary>
        /// Obtiene o establece la tasa de conversión del tipo de cambio (cuántas unidades de moneda destino por una de origen).
        /// </summary>
        [Required(ErrorMessage = "La tasa es obligatoria")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Tasa")]
        [Range(0.000001, double.MaxValue, ErrorMessage = "La tasa debe ser mayor que cero")] // Aunque es un ViewModel de visualización, la anotación está presente.
        public decimal Tasa { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora de la última actualización de la tasa de cambio.
        /// </summary>
        [Display(Name = "Fecha de actualización")]
        // [DataType(DataType.DateTime)] // Podría añadirse si el formato de fecha/hora en la UI necesitara especificación para visualización.
        public DateTime FechaActualizacion { get; set; }
    }
}
