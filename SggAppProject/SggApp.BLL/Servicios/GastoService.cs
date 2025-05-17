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
    /// Implementa la lógica de negocio para la gestión de gastos.
    /// Proporciona operaciones de consulta y manipulación de gastos, incluyendo cálculos de totales con conversión de moneda.
    /// </summary>
    public class GastoService : IGastoService
    {
        private readonly GastoRepository _gastoRepository; // Repositorio para acceder a los datos de gastos.
        private readonly ITipoCambioService _tipoCambioService; // Servicio para obtener tasas de cambio.

        /// <summary>
        /// Inicializa una nueva instancia del servicio de gastos.
        /// </summary>
        /// <param name="gastoRepository">Repositorio para acceder a los datos de gastos.</param>
        /// <param name="tipoCambioService">Servicio para obtener tasas de cambio.</param>
        public GastoService(
            GastoRepository gastoRepository,
            ITipoCambioService tipoCambioService)
        {
            _gastoRepository = gastoRepository;
            _tipoCambioService = tipoCambioService;
        }

        /// <summary>
        /// Obtiene los gastos más recientes de un usuario específico.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <param name="cantidad">El número máximo de gastos recientes a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto ordenadas por fecha de registro descendente.</returns>
        public async Task<IEnumerable<Gasto>> ObtenerRecientesPorUsuarioAsync(int userId, int cantidad)
        {
            // Obtiene todos los gastos del usuario.
            var gastos = await _gastoRepository.GetByUsuarioIdAsync(userId);

            // Ordena los gastos por fecha de registro de forma descendente y toma la cantidad especificada.
            return gastos.OrderByDescending(g => g.FechaRegistro).Take(cantidad);
        }

        /// <summary>
        /// Obtiene todos los gastos disponibles en el sistema.
        /// (Nota: Este método podría ser restringido a administradores o eliminado en una aplicación real).
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto.</returns>
        public async Task<IEnumerable<Gasto>> ObtenerTodosAsync()
        {
            // Delega la operación al repositorio de gastos.
            return await _gastoRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtiene un gasto específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del gasto a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Gasto si se encuentra; de lo contrario, null.</returns>
        public async Task<Gasto> ObtenerPorIdAsync(int id)
        {
            // Delega la operación al repositorio de gastos.
            return await _gastoRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene todos los gastos pertenecientes a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario cuyos gastos se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto del usuario especificado.</returns>
        public async Task<IEnumerable<Gasto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            // Delega la operación al repositorio, filtrando por el ID de usuario.
            return await _gastoRepository.GetByConditionAsync(g => g.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Obtiene los gastos de un usuario dentro de un rango de fechas específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango (inclusiva).</param>
        /// <param name="fechaFin">La fecha de fin del rango (inclusiva).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto dentro del rango de fechas especificado para el usuario.</returns>
        public async Task<IEnumerable<Gasto>> ObtenerPorUsuarioYFechasAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Delega la operación al repositorio, filtrando por usuario y rango de fechas.
            return await _gastoRepository.GetByConditionAsync(g =>
                g.UsuarioId == usuarioId &&
                g.Fecha >= fechaInicio &&
                g.Fecha <= fechaFin);
        }

        /// <summary>
        /// Obtiene todos los gastos asociados a una categoría específica.
        /// </summary>
        /// <param name="categoriaId">El identificador de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto asociadas a la categoría especificada.</returns>
        public async Task<IEnumerable<Gasto>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            // Delega la operación al repositorio, filtrando por el ID de categoría.
            return await _gastoRepository.GetByConditionAsync(g => g.CategoriaId == categoriaId);
        }

        /// <summary>
        /// Calcula el monto total de gastos de un usuario dentro de un rango de fechas, convirtiendo todos los montos a una moneda de destino específica.
        /// Si no se encuentra una tasa de cambio para una conversión necesaria, ese gasto no se incluirá en el total.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="monedaDestinoId">El identificador de la moneda a la que se deben convertir los totales.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango de gastos (inclusiva).</param>
        /// <param name="fechaFin">La fecha de fin del rango de gastos (inclusiva).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el monto total de gastos convertidos a la moneda de destino.</returns>
        public async Task<decimal> ObtenerTotalGastosPorUsuarioAsync(int usuarioId, int monedaDestinoId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Obtiene los gastos del usuario dentro del rango de fechas.
            var gastos = await ObtenerPorUsuarioYFechasAsync(usuarioId, fechaInicio, fechaFin);
            decimal total = 0;

            // Itera sobre cada gasto para sumarlo, realizando conversiones si es necesario.
            foreach (var gasto in gastos)
            {
                // Si la moneda del gasto ya es la moneda de destino, suma el monto directamente.
                if (gasto.MonedaId == monedaDestinoId)
                {
                    total += gasto.Monto;
                }
                // Si la moneda es diferente, intenta obtener la tasa de cambio y convertir.
                else
                {
                    try
                    {
                        // Obtiene la tasa de cambio utilizando el servicio de tipos de cambio.
                        var tasa = await _tipoCambioService.ObtenerTasaCambioAsync(gasto.MonedaId, monedaDestinoId);
                        // Suma el monto convertido al total.
                        total += gasto.Monto * tasa;
                    }
                    // Captura cualquier excepción durante la obtención de la tasa de cambio (ej. si la tasa no existe).
                    catch (Exception)
                    {
                        // Si la tasa de cambio no está disponible, este gasto no se incluye en el total.
                        // En una aplicación de producción, sería importante loggear o notificar este error.
                    }
                }
            }

            // Retorna el monto total calculado.
            return total;
        }

        /// <summary>
        /// Agrega un nuevo gasto al sistema.
        /// Asigna automáticamente la fecha de registro al momento de la creación.
        /// </summary>
        /// <param name="gasto">La entidad Gasto a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AgregarAsync(Gasto gasto)
        {
            // Asigna la fecha y hora actual a la propiedad FechaRegistro.
            gasto.FechaRegistro = DateTime.Now;
            // Agrega el gasto a través del repositorio.
            await _gastoRepository.AddAsync(gasto);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Actualiza un gasto existente.
        /// </summary>
        /// <param name="gasto">La entidad Gasto con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task ActualizarAsync(Gasto gasto)
        {
            // Marca la entidad gasto para ser actualizada a través del repositorio.
            _gastoRepository.Update(gasto);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Elimina un gasto por su identificador.
        /// Verifica si el gasto existe antes de intentar eliminar.
        /// </summary>
        /// <param name="id">El identificador del gasto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task EliminarAsync(int id)
        {
            // Obtiene el gasto por su ID.
            var gasto = await _gastoRepository.GetByIdAsync(id);
            // Si el gasto existe, lo marca para eliminación.
            if (gasto != null)
            {
                _gastoRepository.Delete(gasto);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
            // Si el gasto no existe, el método finaliza sin realizar ninguna operación.
        }

        /// <summary>
        /// Obtiene los gastos de un usuario dentro de un rango de fechas, incluyendo la carga de entidades relacionadas.
        /// Este método es específico para la carga de datos para el dashboard, incluyendo información de Categoría y Moneda.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango (inclusiva).</param>
        /// <param name="fechaFin">La fecha de fin del rango (inclusiva).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto con sus relaciones cargadas.</returns>
        public async Task<IEnumerable<Gasto>> ObtenerPorUsuarioYRangoFechaAsync(int userId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Llama al método específico en el repositorio para obtener los gastos con relaciones cargadas.
            return await _gastoRepository.GetByUsuarioYRangoFechaAsync(userId, fechaInicio, fechaFin);
        }
    }
}
