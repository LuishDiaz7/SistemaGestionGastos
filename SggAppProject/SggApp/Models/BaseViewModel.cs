using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace SggApp.Models
{
    /// <summary>
    /// Clase base para ViewModels que representan entidades con un identificador único.
    /// Proporciona una propiedad 'Id' común.
    /// </summary>
    public class BaseViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador único del ViewModel.
        /// Corresponde al Id de la entidad subyacente.
        /// </summary>
        public int Id { get; set; }
    }
}
