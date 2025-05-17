using System; 
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para presentar y gestionar la información del perfil de un usuario.
    /// Contiene campos para mostrar datos del usuario y permitir su edición.
    /// </summary>
    public class PerfilViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador único del usuario.
        /// </summary>
        public int Id { get; set; } // Este ViewModel no hereda de BaseViewModel, pero incluye Id directamente.

        /// <summary>
        /// Obtiene o establece el nombre del usuario.
        /// Este campo es obligatorio y tiene restricciones de longitud.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(100, ErrorMessage = "El nombre debe tener entre {2} y {1} caracteres", MinimumLength = 2)]
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el email del usuario.
        /// Este campo es de visualización en el perfil y no se espera su edición a través de este ViewModel.
        /// </summary>
        [Display(Name = "Email")]
        // [EmailAddress] // Podría añadirse si se necesitara validar formato en display, aunque no es común.
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que el usuario se registró en el sistema.
        /// Es un campo de visualización.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")] // Formato para mostrar la fecha.
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda predeterminada seleccionada por el usuario.
        /// Permite valores nulos si el usuario no ha seleccionado una.
        /// </summary>
        [Display(Name = "Moneda predeterminada")]
        public int? MonedaPredeterminadaId { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de monedas disponibles para seleccionar como predeterminada en un control de lista.
        /// Esta propiedad se utiliza para la UI y no es parte de los datos enviados por el formulario POST.
        /// </summary>
        public IEnumerable<SelectListItem> MonedasDisponibles { get; set; }
    }
}