using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para la funcionalidad de cambio de contraseña.
    /// Contiene los campos requeridos para que un usuario actualice su contraseña.
    /// </summary>
    public class CambiarPasswordViewModel
    {
        /// <summary>
        /// Obtiene o establece la contraseña actual del usuario.
        /// Este campo es obligatorio.
        /// </summary>
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string PasswordActual { get; set; }

        /// <summary>
        /// Obtiene o establece la nueva contraseña deseada por el usuario.
        /// Este campo es obligatorio y tiene requisitos de longitud mínima y máxima.
        /// </summary>
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        public string NuevoPassword { get; set; }

        /// <summary>
        /// Obtiene o establece la confirmación de la nueva contraseña.
        /// Se utiliza para verificar que la nueva contraseña se ingresó correctamente.
        /// Debe coincidir con el valor de NuevoPassword.
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nueva contraseña")]
        [Compare("NuevoPassword", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarPassword { get; set; }
    }
}
