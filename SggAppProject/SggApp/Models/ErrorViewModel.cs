using System; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para mostrar información de errores en la interfaz de usuario.
    /// Contiene datos relevantes para depuración, como el identificador de la solicitud.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la solicitud web asociada al error.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Obtiene un valor que indica si se debe mostrar el identificador de la solicitud.
        /// Retorna true si la propiedad RequestId no es nula ni está vacía.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}