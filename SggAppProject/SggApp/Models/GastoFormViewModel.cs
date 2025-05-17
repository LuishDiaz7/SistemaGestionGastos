using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar y validar los datos de un gasto en formularios de creación y edición.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class GastoFormViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador de la categoría asociada al gasto.
        /// Este campo es obligatorio.
        /// </summary>
        [Display(Name = "Categoría")]
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int CategoriaId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda en la que se registró el gasto.
        /// Este campo es obligatorio.
        /// </summary>
        [Display(Name = "Moneda")]
        [Required(ErrorMessage = "La moneda es obligatoria")]
        public int MonedaId { get; set; }

        /// <summary>
        /// Obtiene o establece el monto del gasto.
        /// Este campo es obligatorio y debe ser un valor positivo mayor que cero.
        /// </summary>
        [Required(ErrorMessage = "El monto es obligatorio")]
        [Display(Name = "Monto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que se realizó el gasto.
        /// Este campo es obligatorio y se presenta como una fecha.
        /// </summary>
        [Required(ErrorMessage = "La fecha es obligatoria")]
        [Display(Name = "Fecha")]
        [DataType(DataType.Date)] // Indica que este campo representa una fecha.
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece una descripción opcional del gasto.
        /// Tiene una longitud máxima de 255 caracteres.
        /// </summary>
        [Display(Name = "Descripción")]
        [StringLength(255, ErrorMessage = "La descripción debe tener máximo {1} caracteres")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el lugar opcional donde se realizó el gasto.
        /// Tiene una longitud máxima de 100 caracteres.
        /// </summary>
        [Display(Name = "Lugar")]
        [StringLength(100, ErrorMessage = "El lugar debe tener máximo {1} caracteres")]
        public string Lugar { get; set; }

        /// <summary>
        /// Obtiene o establece un valor booleano que indica si el gasto es recurrente.
        /// </summary>
        [Display(Name = "Es recurrente")]
        public bool EsRecurrente { get; set; }

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
