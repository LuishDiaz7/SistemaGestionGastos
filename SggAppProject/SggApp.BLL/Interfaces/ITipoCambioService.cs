using SggApp.DAL.Entidades; 
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios relacionados con la gestión de tipos de cambio.
    /// Establece las operaciones de negocio disponibles para consultar, manipular y convertir montos entre monedas utilizando tasas de cambio.
    /// </summary>
    public interface ITipoCambioService
    {
        /// <summary>
        /// Obtiene un tipo de cambio específico entre una moneda de origen y una moneda de destino.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio si se encuentra; de lo contrario, null.</returns>
        Task<TipoCambio> ObtenerTipoCambioAsync(int monedaOrigen, int monedaDestino); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<TipoCambio?>.

        /// <summary>
        /// Obtiene la tasa de cambio entre una moneda de origen y una moneda de destino.
        /// La implementación puede lanzar una excepción si no se encuentra la tasa de cambio.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la tasa de cambio.</returns>
        Task<decimal> ObtenerTasaCambioAsync(int monedaOrigen, int monedaDestino);

        /// <summary>
        /// Actualiza la tasa de cambio entre una moneda de origen y una moneda de destino, o crea un nuevo registro si no existe.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino.</param>
        /// <param name="tasa">La nueva tasa de cambio.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarTipoCambioAsync(int monedaOrigen, int monedaDestino, decimal tasa);

        /// <summary>
        /// Convierte un monto de una moneda a otra utilizando la tasa de cambio disponible.
        /// La implementación puede lanzar una excepción si no se encuentra la tasa de cambio necesaria.
        /// </summary>
        /// <param name="monto">El monto a convertir.</param>
        /// <param name="monedaOrigen">El identificador de la moneda original del monto.</param>
        /// <param name="monedaDestino">El identificador de la moneda a la que se desea convertir.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el monto convertido.</returns>
        Task<decimal> ConvertirMontoAsync(decimal monto, int monedaOrigen, int monedaDestino);

        /// <summary>
        /// Obtiene una colección de todos los tipos de cambio disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades TipoCambio.</returns>
        Task<IEnumerable<TipoCambio>> ObtenerTodosAsync();

        /// <summary>
        /// Obtiene un tipo de cambio específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del tipo de cambio a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio si se encuentra; de lo contrario, null.</returns>
        Task<TipoCambio> ObtenerPorIdAsync(int id); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<TipoCambio?>.

        /// <summary>
        /// Agrega un nuevo tipo de cambio al sistema.
        /// </summary>
        /// <param name="tipoCambio">La entidad TipoCambio a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AgregarAsync(TipoCambio tipoCambio);

        /// <summary>
        /// Actualiza un tipo de cambio existente.
        /// </summary>
        /// <param name="tipoCambio">La entidad TipoCambio con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarAsync(TipoCambio tipoCambio);

        /// <summary>
        /// Elimina un tipo de cambio por su identificador.
        /// </summary>
        /// <param name="id">El identificador del tipo de cambio a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EliminarAsync(int id);
    }
}
