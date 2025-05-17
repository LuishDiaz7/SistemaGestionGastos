using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 
using Microsoft.AspNetCore.Mvc.Rendering; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar y validar los datos necesarios para el registro de un nuevo usuario.
    /// </summary>
    public class RegistroViewModel
    {
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
        /// Este campo es obligatorio, debe tener un formato de email válido y se utiliza como identificador de inicio de sesión.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o establece la contraseña para la nueva cuenta de usuario.
        /// Este campo es obligatorio y tiene requisitos de longitud mínima y máxima.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)] // Indica que este campo representa una contraseña.
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Obtiene o establece la confirmación de la contraseña.
        /// Se utiliza para verificar que la contraseña se ingresó correctamente.
        /// Debe coincidir con el valor de Password.
        /// </summary>
        [DataType(DataType.Password)] // Indica que este campo representa una contraseña.
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador opcional de la moneda predeterminada para el nuevo usuario.
        /// Permite valor nulo.
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