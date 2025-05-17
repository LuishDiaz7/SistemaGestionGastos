using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para presentar datos de Gastos en la interfaz de usuario (ej. listados o detalles).
    /// Incluye información del gasto y detalles básicos de sus entidades relacionadas (categoría, moneda).
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class GastoViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la categoría asociada al gasto.
        /// </summary>
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la categoría asociada al gasto para su visualización.
        /// </summary>
        [Display(Name = "Categoría")]
        public string CategoriaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda en la que se registró el gasto.
        /// </summary>
        [Display(Name = "Moneda")]
        public int MonedaId { get; set; }

        /// <summary>
        /// Obtiene o establece el código de la moneda asociada al gasto para su visualización (ej. "USD", "COP").
        /// </summary>
        [Display(Name = "Moneda")]
        public string MonedaCodigo { get; set; }

        /// <summary>
        /// Obtiene o establece el monto del gasto.
        /// </summary>
        [Required(ErrorMessage = "El monto es obligatorio")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Monto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")] // Aunque es un ViewModel de visualización, la anotación está presente.
        public decimal Monto { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que se realizó el gasto.
        /// Se presenta como una fecha.
        /// </summary>
        [Required(ErrorMessage = "La fecha es obligatoria")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)] // Indica que este campo representa una fecha.
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción opcional del gasto.
        /// </summary>
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el lugar opcional donde se realizó el gasto.
        /// </summary>
        [Display(Name = "Lugar")]
        public string Lugar { get; set; }

        /// <summary>
        /// Obtiene o establece un valor booleano que indica si el gasto es recurrente.
        /// </summary>
        [Display(Name = "Es recurrente")]
        public bool EsRecurrente { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora en que se registró el gasto en el sistema.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; }
    }
}
