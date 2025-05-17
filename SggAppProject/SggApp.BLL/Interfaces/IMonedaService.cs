using SggApp.DAL.Entidades; 
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios relacionados con la gestión de monedas.
    /// Establece las operaciones de negocio disponibles para interactuar con las monedas, incluyendo verificación de uso para eliminación.
    /// </summary>
    public interface IMonedaService
    {
        /// <summary>
        /// Obtiene una colección de todas las monedas disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Moneda.</returns>
        Task<IEnumerable<Moneda>> ObtenerTodasAsync();

        /// <summary>
        /// Obtiene una moneda específica por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la moneda a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Moneda si se encuentra; de lo contrario, null.</returns>
        Task<Moneda> ObtenerPorIdAsync(int id); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Moneda?>.

        /// <summary>
        /// Obtiene una moneda específica por su código ISO 4217.
        /// </summary>
        /// <param name="codigo">El código de la moneda (ej. "USD", "COP").</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Moneda si se encuentra; de lo contrario, null.</returns>
        Task<Moneda> ObtenerPorCodigoAsync(string codigo); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Moneda?>.

        /// <summary>
        /// Obtiene una colección de todas las monedas que están marcadas como activas.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Moneda activas.</returns>
        Task<IEnumerable<Moneda>> ObtenerActivasAsync();

        /// <summary>
        /// Agrega una nueva moneda al sistema.
        /// </summary>
        /// <param name="moneda">La entidad Moneda a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AgregarAsync(Moneda moneda);

        /// <summary>
        /// Actualiza una moneda existente.
        /// </summary>
        /// <param name="moneda">La entidad Moneda con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarAsync(Moneda moneda);

        /// <summary>
        /// Desactiva una moneda marcando su propiedad 'Activa' como false.
        /// </summary>
        /// <param name="id">El identificador de la moneda a desactivar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task DesactivarAsync(int id);

        /// <summary>
        /// Verifica si una moneda está siendo utilizada en otros registros (gastos, presupuestos, usuarios).
        /// </summary>
        /// <param name="monedaId">El identificador de la moneda a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si la moneda tiene registros asociados; de lo contrario, false.</returns>
        Task<bool> MonedaTieneRegistrosAsociadosAsync(int monedaId);

        /// <summary>
        /// Cuenta el número total de registros (gastos, presupuestos, usuarios) que utilizan una moneda específica.
        /// </summary>
        /// <param name="monedaId">El identificador de la moneda a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el número total de registros asociados.</returns>
        Task<int> ContarRegistrosAsociadosAsync(int monedaId);

        /// <summary>
        /// Elimina una moneda por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la moneda a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si la moneda fue eliminada exitosamente; retorna false si tiene registros asociados o no fue encontrada.</returns>
        Task<bool> EliminarAsync(int id);
    }
}
