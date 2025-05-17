using System.ComponentModel.DataAnnotations;

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para representar y gestionar datos de Categorías en la interfaz de usuario.
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class CategoriaViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el nombre de la categoría.
        /// Este campo es obligatorio y tiene una longitud máxima de 50 caracteres.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(50, ErrorMessage = "El nombre debe tener máximo {1} caracteres")]
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción opcional de la categoría.
        /// </summary>
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la categoría está activa.
        /// </summary>
        [Display(Name = "Activa")]
        public bool Activa { get; set; }
    }
}
