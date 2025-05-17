using SggApp.DAL.Entidades; 
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios relacionados con la gestión de datos de la entidad Usuario.
    /// Establece las operaciones de negocio disponibles para interactuar con los datos del usuario, complementando ASP.NET Core Identity.
    /// </summary>
    public interface IUsuarioService
    {
        /// <summary>
        /// Obtiene una colección de todos los usuarios registrados.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Usuario.</returns>
        Task<IEnumerable<Usuario>> ObtenerTodosAsync();

        /// <summary>
        /// Obtiene un usuario específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del usuario a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Usuario si se encuentra; de lo contrario, null.</returns>
        Task<Usuario> ObtenerPorIdAsync(int id); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Usuario?>.

        /// <summary>
        /// Obtiene un usuario específico por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Usuario si se encuentra; de lo contrario, null.</returns>
        Task<Usuario> ObtenerPorEmailAsync(string email); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Usuario?>.

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos.
        /// </summary>
        /// <param name="usuario">La entidad Usuario a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AgregarAsync(Usuario usuario);

        /// <summary>
        /// Actualiza los datos de un usuario existente.
        /// </summary>
        /// <param name="usuario">La entidad Usuario con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarAsync(Usuario usuario);

        /// <summary>
        /// Elimina un usuario por su identificador.
        /// Si el usuario no se encuentra, la operación se completa sin realizar cambios.
        /// </summary>
        /// <param name="id">El identificador del usuario a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EliminarAsync(int id);

        /// <summary>
        /// Verifica si existe un usuario con una dirección de correo electrónico específica.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si existe un usuario con ese email; de lo contrario, false.</returns>
        Task<bool> ExisteEmailAsync(string email);

        /// <summary>
        /// Cambia la moneda predeterminada para un usuario específico.
        /// Si el usuario no se encuentra, la operación se completa sin realizar cambios.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="monedaId">El identificador de la nueva moneda predeterminada.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task CambiarMonedaPredeterminadaAsync(int usuarioId, int monedaId);
    }
}
