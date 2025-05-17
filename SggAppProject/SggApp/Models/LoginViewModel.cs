using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar las credenciales de inicio de sesión del usuario.
    /// Contiene los campos de email, contraseña y la opción "Recordarme".
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Obtiene o establece el email del usuario para iniciar sesión.
        /// Este campo es obligatorio y debe tener un formato de email válido.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o establece la contraseña del usuario para iniciar sesión.
        /// Este campo es obligatorio y se presenta como un campo de tipo contraseña.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Obtiene o establece un valor booleano que indica si se debe recordar la sesión del usuario.
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
