using System;
using System.Collections.Generic; 
using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para presentar información general de un Usuario en la interfaz de usuario (ej. listados, detalles administrativos).
    /// Hereda de BaseViewModel para incluir la propiedad Id.
    /// </summary>
    public class UsuarioViewModel : BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el nombre del usuario.
        /// Este campo es obligatorio.
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el email del usuario.
        /// Este campo es obligatorio y se presenta con formato de email válido.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")] // Aunque es un ViewModel de visualización, la anotación está presente.
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que el usuario se registró en el sistema.
        /// Se presenta con un formato de fecha específico.
        /// </summary>
        [Display(Name = "Fecha de registro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")] // Formato para mostrar la fecha.
        public DateTime FechaRegistro { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda predeterminada seleccionada por el usuario.
        /// Permite valores nulos.
        /// </summary>
        [Display(Name = "Moneda predeterminada")]
        public int? MonedaPredeterminadaId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la moneda predeterminada del usuario para su visualización.
        /// </summary>
        [Display(Name = "Moneda predeterminada")]
        public string MonedaPredeterminadaNombre { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la cuenta del usuario está activa.
        /// </summary>
        [Display(Name = "Activo")]
        public bool Activo { get; set; }
    }
}