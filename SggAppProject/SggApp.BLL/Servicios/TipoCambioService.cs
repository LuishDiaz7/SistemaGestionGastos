using SggApp.BLL.Interfaces; 
using SggApp.DAL.Entidades; 
using SggApp.DAL.Repositorios; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Servicios
{
    /// <summary>
    /// Implementa la lógica de negocio para la gestión de tipos de cambio.
    /// Proporciona operaciones CRUD para tipos de cambio, obtención de tasas y conversión de montos entre monedas.
    /// </summary>
    public class TipoCambioService : ITipoCambioService
    {
        private readonly TipoCambioRepository _tipoCambioRepository; // Repositorio para acceder a los datos de tipos de cambio.
        // private readonly ApplicationDbContext _context; // Campo removido: inyectado pero no utilizado en el código proporcionado.

        /// <summary>
        /// Inicializa una nueva instancia del servicio de tipos de cambio.
        /// </summary>
        /// <param name="tipoCambioRepository">Repositorio para acceder a los datos de tipos de cambio.</param>
        // <param name="context">Parámetro removido: inyectado pero no utilizado.</param>
        public TipoCambioService(TipoCambioRepository tipoCambioRepository /*, ApplicationDbContext context */)
        {
            _tipoCambioRepository = tipoCambioRepository;
            // _context = context; // Asignación removida.
        }

        /// <summary>
        /// Obtiene un tipo de cambio específico entre una moneda de origen y una moneda de destino.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio si se encuentra; de lo contrario, null.</returns>
        public async Task<TipoCambio?> ObtenerTipoCambioAsync(int monedaOrigen, int monedaDestino)
        {
            // Delega la operación al repositorio para buscar por par de monedas.
            return await _tipoCambioRepository.GetByMonedasAsync(monedaOrigen, monedaDestino);
        }

        /// <summary>
        /// Obtiene la tasa de cambio entre una moneda de origen y una moneda de destino.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la tasa de cambio.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentra un tipo de cambio definido para el par de monedas especificado.</exception>
        public async Task<decimal> ObtenerTasaCambioAsync(int monedaOrigen, int monedaDestino)
        {
            // Obtiene el tipo de cambio utilizando otro método del servicio.
            var tipoCambio = await ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);
            // Retorna la tasa si el tipo de cambio existe, o lanza una excepción si no se encuentra.
            return tipoCambio?.Tasa ?? throw new KeyNotFoundException("No se encontró el tipo de cambio especificado.");
        }

        /// <summary>
        /// Actualiza la tasa de cambio entre una moneda de origen y una moneda de destino, o crea un nuevo registro si no existe.
        /// Asigna la fecha de actualización al momento de la operación.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino.</param>
        /// <param name="tasa">La nueva tasa de cambio.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task ActualizarTipoCambioAsync(int monedaOrigen, int monedaDestino, decimal tasa)
        {
            // Intenta obtener el tipo de cambio existente por el par de monedas.
            var tipoCambio = await ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);

            // Si no se encuentra un tipo de cambio existente para este par:
            if (tipoCambio == null)
            {
                // Crea una nueva entidad TipoCambio con los datos proporcionados.
                tipoCambio = new TipoCambio
                {
                    MonedaOrigenId = monedaOrigen,
                    MonedaDestinoId = monedaDestino,
                    Tasa = tasa,
                    FechaActualizacion = DateTime.Now // Establece la fecha de actualización.
                };
                // Agrega la nueva entidad a través del repositorio.
                await _tipoCambioRepository.AddAsync(tipoCambio);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
            // Si se encuentra un tipo de cambio existente:
            else
            {
                // Actualiza la tasa y la fecha de actualización de la entidad existente.
                tipoCambio.Tasa = tasa;
                tipoCambio.FechaActualizacion = DateTime.Now; // Actualiza la fecha.
                // Marca la entidad para ser actualizada a través del repositorio.
                _tipoCambioRepository.Update(tipoCambio);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
        }

        /// <summary>
        /// Convierte un monto de una moneda a otra utilizando la tasa de cambio disponible.
        /// </summary>
        /// <param name="monto">El monto a convertir.</param>
        /// <param name="monedaOrigen">El identificador de la moneda original del monto.</param>
        /// <param name="monedaDestino">El identificador de la moneda a la que se desea convertir.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el monto convertido.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si no se encuentra un tipo de cambio definido para la conversión necesaria (propagada desde ObtenerTasaCambioAsync).</exception>
        public async Task<decimal> ConvertirMontoAsync(decimal monto, int monedaOrigen, int monedaDestino)
        {
            // Si las monedas de origen y destino son las mismas, retorna el monto original.
            if (monedaOrigen == monedaDestino)
                return monto;

            // Obtiene la tasa de cambio utilizando otro método del servicio (lanzará excepción si no la encuentra).
            var tasa = await ObtenerTasaCambioAsync(monedaOrigen, monedaDestino);
            // Retorna el monto multiplicado por la tasa de cambio.
            return monto * tasa;
        }

        // --- Implementaciones de métodos CRUD de ITipoCambioService ---

        /// <summary>
        /// Obtiene todos los tipos de cambio disponibles en el sistema.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades TipoCambio.</returns>
        public async Task<IEnumerable<TipoCambio>> ObtenerTodosAsync()
        {
            // Delega la operación al método correspondiente en el repositorio.
            return await _tipoCambioRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtiene un tipo de cambio específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del tipo de cambio a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio si se encuentra; de lo contrario, null.</returns>
        public async Task<TipoCambio> ObtenerPorIdAsync(int id)
        {
            // Delega la operación al método correspondiente en el repositorio.
            // Nota: La interfaz define el retorno como Task<TipoCambio>, pero la implementación del repositorio GetByIdAsync podría devolver null.
            // El consumidor debe manejar el caso en que el resultado sea null.
            return await _tipoCambioRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Agrega un nuevo tipo de cambio al sistema.
        /// Se recomienda realizar validaciones de negocio (ej. unicidad del par de monedas) antes de llamar a este método.
        /// Asigna la fecha de actualización si no se ha establecido.
        /// </summary>
        /// <param name="tipoCambio">La entidad TipoCambio a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        // <exception cref="ArgumentException">Puede ser lanzada aquí o por una capa superior si se implementa validación de unicidad del par de monedas.</exception>
        public async Task AgregarAsync(TipoCambio tipoCambio)
        {
            // Se recomienda implementar validación aquí si aún no se hace en otra capa (ej. controlador).
            // if (await ObtenerTipoCambioAsync(tipoCambio.MonedaOrigenId, tipoCambio.MonedaDestinoId) != null)
            // {
            //      throw new ArgumentException("Ya existe un tipo de cambio para este par de monedas.");
            // }

            // Asegura que la fecha de actualización se establezca si no tiene un valor por defecto (ej. DateTime.MinValue).
            if (tipoCambio.FechaActualizacion == default)
            {
                tipoCambio.FechaActualizacion = DateTime.Now;
            }

            // Delega la operación al método correspondiente en el repositorio.
            await _tipoCambioRepository.AddAsync(tipoCambio);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Actualiza un tipo de cambio existente utilizando los datos proporcionados en la entidad.
        /// Se asume que la entidad `tipoCambio` recibida ya contiene los datos actualizados.
        /// </summary>
        /// <param name="tipoCambio">La entidad TipoCambio con los datos actualizados. Se espera que el ID sea válido.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si el tipo de cambio con el ID especificado no se encuentra para actualizar.</exception>
        public async Task ActualizarAsync(TipoCambio tipoCambio)
        {
            // Opcional, pero buena práctica: verificar si la entidad existe antes de intentar actualizar.
            var existingTipoCambio = await _tipoCambioRepository.GetByIdAsync(tipoCambio.Id);
            if (existingTipoCambio == null)
            {
                // Lanza una excepción si la entidad no fue encontrada para actualizar.
                throw new KeyNotFoundException($"Tipo de cambio con ID {tipoCambio.Id} no encontrado para actualizar.");
            }

            // Delega la operación al método correspondiente en el repositorio.
            // Si el mapeo se hace en el controlador, `tipoCambio` ya es la entidad existente actualizada.
            // Si el mapeo no se hace antes, deberías mapear aquí: _mapper.Map(tipoCambio, existingTipoCambio);
            _tipoCambioRepository.Update(tipoCambio);

            // Puedes actualizar la fecha aquí si quieres que refleje la última modificación desde el servicio.
            // tipoCambio.FechaActualizacion = DateTime.Now; // o existingTipoCambio.FechaActualizacion = DateTime.Now;
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Elimina un tipo de cambio por su identificador.
        /// </summary>
        /// <param name="id">El identificador del tipo de cambio a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="KeyNotFoundException">Lanzada si el tipo de cambio con el ID especificado no se encuentra para eliminar.</exception>
        public async Task EliminarAsync(int id)
        {
            // Obtiene el tipo de cambio a eliminar por su ID.
            var tipoCambioToDelete = await _tipoCambioRepository.GetByIdAsync(id);
            // Si el tipo de cambio no se encuentra, lanza una excepción.
            if (tipoCambioToDelete == null)
            {
                throw new KeyNotFoundException($"Tipo de cambio con ID {id} no encontrado para eliminar.");
            }

            // Marca la entidad para ser eliminada a través del repositorio.
            _tipoCambioRepository.Delete(tipoCambioToDelete);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            // Basado en tu esquema DB, TipoCambio no tiene FKs apuntándole desde otras tablas,
            // por lo que no necesita verificaciones de registros asociados para eliminar.
        }
    }
}