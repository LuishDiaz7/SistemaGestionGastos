using SggApp.DAL.Entidades; 
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios relacionados con la gestión de gastos.
    /// Establece las operaciones de negocio disponibles para interactuar con los gastos.
    /// </summary>
    public interface IGastoService
    {
        /// <summary>
        /// Obtiene una colección de todos los gastos disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto.</returns>
        Task<IEnumerable<Gasto>> ObtenerTodosAsync();

        /// <summary>
        /// Obtiene un gasto específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del gasto a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Gasto si se encuentra; de lo contrario, null.</returns>
        Task<Gasto> ObtenerPorIdAsync(int id); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Gasto?>.

        /// <summary>
        /// Obtiene una colección de gastos pertenecientes a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario cuyos gastos se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto del usuario especificado.</returns>
        Task<IEnumerable<Gasto>> ObtenerPorUsuarioAsync(int usuarioId);

        /// <summary>
        /// Obtiene una colección de gastos de un usuario dentro de un rango de fechas específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango (inclusiva).</param>
        /// <param name="fechaFin">La fecha de fin del rango (inclusiva).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto dentro del rango de fechas especificado para el usuario.</returns>
        Task<IEnumerable<Gasto>> ObtenerPorUsuarioYFechasAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene una colección de gastos asociados a una categoría específica.
        /// </summary>
        /// <param name="categoriaId">El identificador de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto asociadas a la categoría especificada.</returns>
        Task<IEnumerable<Gasto>> ObtenerPorCategoriaAsync(int categoriaId);

        /// <summary>
        /// Calcula el monto total de gastos de un usuario dentro de un rango de fechas, convirtiendo todos los montos a una moneda de destino específica.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="monedaId">El identificador de la moneda a la que se deben convertir los totales.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango de gastos (inclusiva).</param>
        /// <param name="fechaFin">La fecha de fin del rango de gastos (inclusiva).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el monto total de gastos convertidos a la moneda de destino.</returns>
        Task<decimal> ObtenerTotalGastosPorUsuarioAsync(int usuarioId, int monedaId, DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Agrega un nuevo gasto al sistema.
        /// </summary>
        /// <param name="gasto">La entidad Gasto a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AgregarAsync(Gasto gasto);

        /// <summary>
        /// Actualiza un gasto existente.
        /// </summary>
        /// <param name="gasto">La entidad Gasto con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarAsync(Gasto gasto);

        /// <summary>
        /// Elimina un gasto por su identificador.
        /// </summary>
        /// <param name="id">El identificador del gasto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EliminarAsync(int id);

        /// <summary>
        /// Obtiene los gastos de un usuario dentro de un rango de fechas, incluyendo la carga de entidades relacionadas (ej. para visualización detallada).
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango (inclusiva).</param>
        /// <param name="fechaFin">La fecha de fin del rango (inclusiva).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto con sus relaciones cargadas.</returns>
        Task<IEnumerable<Gasto>> ObtenerPorUsuarioYRangoFechaAsync(int userId, DateTime fechaInicio, DateTime fechaFin);

        /// <summary>
        /// Obtiene los gastos más recientes de un usuario.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <param name="cantidad">El número máximo de gastos recientes a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto ordenadas por fecha de registro descendente.</returns>
        Task<IEnumerable<Gasto>> ObtenerRecientesPorUsuarioAsync(int userId, int cantidad);
    }
}
