using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para presentar datos de Presupuestos en la interfaz de usuario (ej. listados, detalles o dashboard).
    /// Incluye información del presupuesto, detalles básicos de sus entidades relacionadas, y métricas calculadas de uso.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class PresupuestoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la categoría asociada al presupuesto.
        /// Este campo es opcional y puede ser nulo.
        /// </summary>
        [Display(Name = "Categoría")]
        public int? CategoriaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la categoría asociada al presupuesto para su visualización.
        /// Será "General" si el presupuesto no tiene una categoría específica.
        /// </summary>
        [Display(Name = "Categoría")]
        public string CategoriaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda en la que se definió el límite del presupuesto.
        /// </summary>
        [Display(Name = "Moneda")]
        public int MonedaId { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la moneda asociada al presupuesto para su visualización (ej. "USD", "COP").
        /// </summary>
        [Display(Name = "Moneda")]
        public string MonedaCodigo { get; set; }

        /// <summary>
        /// Obtiene o establece el monto límite definido para el presupuesto.
        /// </summary>
        [Required(ErrorMessage = "El límite es obligatorio")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Límite")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El límite debe ser mayor que cero")] // Aunque es un ViewModel de visualización, la anotación está presente.
        public decimal Limite { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de inicio del período que cubre el presupuesto.
        /// Se presenta como una fecha.
        /// </summary>
        [Required(ErrorMessage = "La fecha de inicio es obligatoria")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Fecha de inicio")]
        [DataType(DataType.Date)] // Indica que este campo representa una fecha.
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de fin del período que cubre el presupuesto.
        /// Se presenta como una fecha.
        /// </summary>
        [Required(ErrorMessage = "La fecha de fin es obligatoria")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Fecha de fin")]
        [DataType(DataType.Date)] // Indica que este campo representa una fecha.
        public DateTime FechaFin { get; set; }

        /// <summary>
        /// Obtiene o establece el porcentaje opcional para notificar al usuario sobre el uso del presupuesto.
        /// Permite valores nulos.
        /// </summary>
        [Display(Name = "Notificar al")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")] // Aunque es un ViewModel de visualización, la anotación está presente.
        public int? NotificarAl { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que se creó el presupuesto.
        /// </summary>
        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Obtiene o establece el monto total gastado que se contabiliza contra este presupuesto.
        /// Este valor suele ser calculado en la lógica de negocio o controlador.
        /// </summary>
        [Display(Name = "Monto gastado")]
        public decimal MontoGastado { get; set; }

        /// <summary>
        /// Obtiene o establece el porcentaje del presupuesto límite que ha sido utilizado por el monto gastado.
        /// Este valor suele ser calculado en la lógica de negocio o controlador.
        /// </summary>
        [Display(Name = "Porcentaje utilizado")]
        public decimal PorcentajeUtilizado { get; set; }
    }
}
