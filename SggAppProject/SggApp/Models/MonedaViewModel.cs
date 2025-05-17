using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para representar y gestionar datos de Monedas en la interfaz de usuario.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class MonedaViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el código ISO 4217 de 3 caracteres de la moneda (ej. "USD", "COP").
        /// Este campo es obligatorio y debe tener exactamente 3 caracteres.
        /// </summary>
        [Required(ErrorMessage = "El código es obligatorio")]
        [Display(Name = "Código")]
        [StringLength(3, MinimumLength = 3, ErrorMessage = "El código debe tener exactamente 3 caracteres")]
        public string Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre completo de la moneda (ej. "Dólar estadounidense", "Peso colombiano").
        /// Este campo es obligatorio y tiene una longitud máxima de 50 caracteres.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El nombre debe tener máximo {1} caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el símbolo de la moneda opcional (ej. "$", "€", "COP$").
        /// Tiene una longitud máxima de 5 caracteres.
        /// </summary>
        [Display(Name = "Símbolo")]
        [StringLength(5, ErrorMessage = "El símbolo debe tener máximo {1} caracteres")]
        public string Simbolo { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la moneda está activa en el sistema.
        /// </summary>
        [Display(Name = "Activa")]
        public bool Activa { get; set; }
    }
}