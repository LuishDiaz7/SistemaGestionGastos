using SggApp.DAL.Entidades; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios relacionados con la gestión de categorías.
    /// Establece las operaciones de negocio disponibles para interactuar con las categorías.
    /// </summary>
    public interface ICategoriaService
    {
        /// <summary>
        /// Obtiene una colección de todas las categorías disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Categoria.</returns>
        Task<IEnumerable<Categoria>> ObtenerTodasAsync();

        /// <summary>
        /// Obtiene una categoría específica por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la categoría a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Categoria si se encuentra; de lo contrario, null.</returns>
        Task<Categoria> ObtenerPorIdAsync(int id); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Categoria?>.

        /// <summary>
        /// Obtiene una colección de categorías pertenecientes a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario cuyas categorías se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Categoria del usuario especificado.</returns>
        Task<IEnumerable<Categoria>> ObtenerPorUsuarioAsync(int usuarioId);

        /// <summary>
        /// Agrega una nueva categoría al sistema.
        /// </summary>
        /// <param name="categoria">La entidad Categoria a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AgregarAsync(Categoria categoria);

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="categoria">La entidad Categoria con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarAsync(Categoria categoria);

        /// <summary>
        /// Elimina una categoría por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la categoría a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EliminarAsync(int id);

        /// <summary>
        /// Verifica si ya existe una categoría con un nombre específico para un usuario dado.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="nombre">El nombre de la categoría a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si ya existe una categoría con ese nombre para el usuario; de lo contrario, false.</returns>
        Task<bool> ExisteNombreParaUsuarioAsync(int usuarioId, string nombre);
    }
}