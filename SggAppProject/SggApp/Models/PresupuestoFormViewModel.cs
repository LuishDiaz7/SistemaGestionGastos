using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar y validar los datos de un presupuesto en formularios de creación y edición.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class PresupuestoFormViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la categoría asociada al presupuesto.
        /// Este campo es opcional (permite valor nulo).
        /// </summary>
        [Display(Name = "Categoría (opcional)")]
        public int? CategoriaId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda en la que se define el límite del presupuesto.
        /// Este campo es obligatorio.
        /// </summary>
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "La moneda es obligatoria")]
        public int MonedaId { get; set; }

        /// <summary>
        /// Obtiene o establece el monto límite definido para el presupuesto.
        /// Este campo es obligatorio y debe ser mayor que cero.
        /// </summary>
        [Required(ErrorMessage = "El límite es obligatorio")]
        [Display(Name = "Límite")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El límite debe ser mayor que cero")]
        public decimal Limite { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de inicio del período que cubre el presupuesto.
        /// Este campo es obligatorio y se presenta como una fecha.
        /// </summary>
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)] // Indica que este campo representa una fecha.
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de fin del período que cubre el presupuesto.
        /// Este campo es obligatorio y se presenta como una fecha.
        /// </summary>
        [Required(ErrorMessage = "La fecha de fin es obligatoria")]
        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)] // Indica que este campo representa una fecha.
        public DateTime FechaFin { get; set; }

        /// <summary>
        /// Obtiene o establece un porcentaje opcional para notificar al usuario cuando alcance un cierto umbral del presupuesto.
        /// Permite valores nulos y debe estar entre 0 y 100.
        /// </summary>
        [Display(Name = "Notificar al (%)")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public int? NotificarAl { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de categorías disponibles para seleccionar en un control de lista (Dropdown).
        /// Esta propiedad se utiliza para la UI y no es parte de los datos enviados por el formulario POST.
        /// </summary>
        public IEnumerable<SelectListItem> CategoriasDisponibles { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de monedas disponibles para seleccionar en un control de lista (Dropdown).
        /// Esta propiedad se utiliza para la UI y no es parte de los datos enviados por el formulario POST.
        /// </summary>
        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}
