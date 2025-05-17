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
    /// Implementa la lógica de negocio para la gestión de presupuestos.
    /// Proporciona operaciones de consulta, manipulación y cálculo de uso para presupuestos de usuario.
    /// </summary>
    public class PresupuestoService : IPresupuestoService
    {
        private readonly PresupuestoRepository _presupuestoRepository; // Repositorio para acceder a los datos de presupuestos.
        private readonly GastoRepository _gastoRepository; // Repositorio para acceder a los datos de gastos (usado para cálculos de uso).
        private readonly ITipoCambioService _tipoCambioService; // Servicio para obtener tasas de cambio (usado en cálculos de uso).
        // private readonly ApplicationDbContext _context; // Removido: inyectado pero no utilizado en el código proporcionado.

        /// <summary>
        /// Inicializa una nueva instancia del servicio de presupuestos.
        /// </summary>
        /// <param name="presupuestoRepository">Repositorio para acceder a los datos de presupuestos.</param>
        /// <param name="gastoRepository">Repositorio para acceder a los datos de gastos.</param>
        /// <param name="tipoCambioService">Servicio para obtener tasas de cambio.</param>
        public PresupuestoService(
            PresupuestoRepository presupuestoRepository,
            GastoRepository gastoRepository,
            ITipoCambioService tipoCambioService)
        {
            _presupuestoRepository = presupuestoRepository;
            _gastoRepository = gastoRepository;
            _tipoCambioService = tipoCambioService;
            // _context = context; // Removido: inyectado pero no utilizado.
        }

        /// <summary>
        /// Obtiene todos los presupuestos disponibles en el sistema.
        /// (Nota: Este método podría ser restringido a administradores o eliminado en una aplicación real).
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto.</returns>
        public async Task<IEnumerable<Presupuesto>> ObtenerTodosAsync()
        {
            // Delega la operación al repositorio de presupuestos.
            return await _presupuestoRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtiene un presupuesto específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del presupuesto a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Presupuesto si se encuentra; de lo contrario, null.</returns>
        public async Task<Presupuesto> ObtenerPorIdAsync(int id)
        {
            // Delega la operación al repositorio de presupuestos.
            return await _presupuestoRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene todos los presupuestos pertenecientes a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario cuyos presupuestos se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto del usuario especificado.</returns>
        public async Task<IEnumerable<Presupuesto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            // Delega la operación al repositorio, filtrando por el ID de usuario.
            return await _presupuestoRepository.GetByUsuarioIdWithDetailsAsync(usuarioId);
        }

        /// <summary>
        /// Obtiene los presupuestos de un usuario que están vigentes en la fecha actual.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto vigentes para el usuario especificado.</returns>
        public async Task<IEnumerable<Presupuesto>> ObtenerPresupuestosVigentesAsync(int usuarioId)
        {
            DateTime hoy = DateTime.Today;
            // Delega la operación al repositorio, filtrando por usuario y rango de fechas que incluya el día de hoy.
            return await _presupuestoRepository.GetByConditionAsync(p =>
                p.UsuarioId == usuarioId &&
                p.FechaInicio <= hoy &&
                p.FechaFin >= hoy);
        }

        /// <summary>
        /// Obtiene los presupuestos asociados a una categoría específica.
        /// (Nota: Esto puede incluir presupuestos generales si CategoriaId es nulo en el criterio de búsqueda).
        /// </summary>
        /// <param name="categoriaId">El identificador de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto asociadas a la categoría especificada (o aquellos donde CategoriaId coincide).</returns>
        public async Task<IEnumerable<Presupuesto>> ObtenerPresupuestosPorCategoriaAsync(int categoriaId)
        {
            // Delega la operación al repositorio, filtrando por el ID de categoría.
            // Esto solo encontrará presupuestos *asignados a esa categoría específica*.
            return await _presupuestoRepository.GetByConditionAsync(p => p.CategoriaId == categoriaId);
        }

        /// <summary>
        /// Calcula el monto total de gastos asociados a un presupuesto específico dentro de su período y categoría (si aplica),
        /// convirtiendo todos los montos a la moneda del presupuesto.
        /// Si no se encuentra una tasa de cambio para una conversión necesaria, ese gasto no se incluirá en el total.
        /// </summary>
        /// <param name="presupuestoId">El identificador del presupuesto.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el monto total de gastos relevantes convertidos a la moneda del presupuesto.</returns>
        /// <exception cref="Exception">Lanzada si el presupuesto con el ID especificado no se encuentra.</exception>
        public async Task<decimal> ObtenerGastoActualAsync(int presupuestoId)
        {
            // Obtiene el presupuesto por su ID.
            var presupuesto = await _presupuestoRepository.GetByIdAsync(presupuestoId);
            if (presupuesto == null)
            {
                // Lanza una excepción si el presupuesto no existe.
                throw new Exception($"No existe presupuesto con ID {presupuestoId}");
            }

            IEnumerable<Gasto> gastos;

            // Determina qué gastos son relevantes para este presupuesto (por usuario, rango de fechas y opcionalmente por categoría).
            if (presupuesto.CategoriaId.HasValue)
            {
                // Si el presupuesto es para una categoría específica, obtiene gastos que coincidan con esa categoría.
                gastos = await _gastoRepository.GetByConditionAsync(g =>
                    g.UsuarioId == presupuesto.UsuarioId &&
                    g.CategoriaId == presupuesto.CategoriaId &&
                    g.Fecha >= presupuesto.FechaInicio &&
                    g.Fecha <= presupuesto.FechaFin);
            }
            else
            {
                // Si el presupuesto es general (no tiene categoría), obtiene todos los gastos del usuario en el rango de fechas.
                gastos = await _gastoRepository.GetByConditionAsync(g =>
                    g.UsuarioId == presupuesto.UsuarioId &&
                    g.Fecha >= presupuesto.FechaInicio &&
                    g.Fecha <= presupuesto.FechaFin);
            }

            decimal total = 0;
            // Itera sobre cada gasto relevante para sumarlo, realizando conversiones si es necesario.
            foreach (var gasto in gastos)
            {
                // Si la moneda del gasto ya es la moneda del presupuesto, suma el monto directamente.
                if (gasto.MonedaId == presupuesto.MonedaId)
                {
                    total += gasto.Monto;
                }
                // Si la moneda es diferente, intenta obtener la tasa de cambio y convertir.
                else
                {
                    try
                    {
                        // Obtiene la tasa de cambio utilizando el servicio de tipos de cambio.
                        var tasa = await _tipoCambioService.ObtenerTasaCambioAsync(gasto.MonedaId, presupuesto.MonedaId);
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

            // Retorna el monto total de gastos relevantes calculado.
            return total;
        }

        /// <summary>
        /// Calcula el porcentaje del límite de un presupuesto que ha sido consumido por los gastos relevantes.
        /// </summary>
        /// <param name="presupuestoId">El identificador del presupuesto.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el porcentaje de consumo del presupuesto.</returns>
        /// <exception cref="Exception">Lanzada si el presupuesto con el ID especificado no se encuentra (propagada desde ObtenerGastoActualAsync).</exception>
        public async Task<decimal> ObtenerPorcentajeConsumidoAsync(int presupuestoId)
        {
            // Obtiene el presupuesto para verificar su límite (esto también valida su existencia).
            var presupuesto = await _presupuestoRepository.GetByIdAsync(presupuestoId);
            // Propagará la excepción si el presupuesto no existe.
            if (presupuesto == null)
            {
                throw new Exception($"No existe presupuesto con ID {presupuestoId}");
            }

            // Obtiene el monto de gasto actual asociado al presupuesto utilizando otro método del servicio.
            decimal gastoActual = await ObtenerGastoActualAsync(presupuestoId);

            // Calcula el porcentaje consumido.
            if (presupuesto.Limite <= 0)
            {
                // Si el límite es cero o negativo, el porcentaje consumido es 0 para evitar división por cero.
                return 0;
            }

            // Calcula y redondea el porcentaje al 2 decimales.
            return Math.Round((gastoActual / presupuesto.Limite) * 100, 2);
        }

        /// <summary>
        /// Verifica si el gasto actual de un presupuesto ha superado el nivel de alerta configurado.
        /// </summary>
        /// <param name="presupuestoId">El identificador del presupuesto a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si el porcentaje consumido es mayor o igual al umbral de alerta configurado; de lo contrario, false.</returns>
        public async Task<bool> VerificarAlertaPresupuestoAsync(int presupuestoId)
        {
            // Obtiene el presupuesto y verifica si tiene un nivel de notificación configurado.
            var presupuesto = await _presupuestoRepository.GetByIdAsync(presupuestoId);
            if (presupuesto == null || !presupuesto.NotificarAl.HasValue)
            {
                // Retorna false si el presupuesto no existe o no tiene umbral de notificación configurado.
                return false;
            }

            // Obtiene el porcentaje de consumo actual del presupuesto.
            decimal porcentajeConsumido = await ObtenerPorcentajeConsumidoAsync(presupuestoId);

            // Compara el porcentaje consumido con el nivel de alerta configurado.
            return porcentajeConsumido >= presupuesto.NotificarAl.Value;
        }


        /// <summary>
        /// Agrega un nuevo presupuesto al sistema.
        /// Asigna automáticamente la fecha de creación al momento de agregarlo.
        /// </summary>
        /// <param name="presupuesto">La entidad Presupuesto a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AgregarAsync(Presupuesto presupuesto)
        {
            // Asigna la fecha y hora actual a la propiedad FechaCreacion.
            presupuesto.FechaCreacion = DateTime.Now;
            // Agrega el presupuesto a través del repositorio.
            await _presupuestoRepository.AddAsync(presupuesto);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Actualiza un presupuesto existente.
        /// </summary>
        /// <param name="presupuesto">La entidad Presupuesto con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task ActualizarAsync(Presupuesto presupuesto)
        {
            // Marca la entidad presupuesto para ser actualizada a través del repositorio.
            _presupuestoRepository.Update(presupuesto);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Elimina un presupuesto por su identificador.
        /// Verifica si el presupuesto existe antes de intentar eliminar.
        /// </summary>
        /// <param name="id">El identificador del presupuesto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task EliminarAsync(int id)
        {
            // Obtiene el presupuesto por su ID.
            var presupuesto = await _presupuestoRepository.GetByIdAsync(id);
            // Si el presupuesto existe, lo marca para eliminación.
            if (presupuesto != null)
            {
                _presupuestoRepository.Delete(presupuesto);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
            // Si el presupuesto no existe, el método finaliza sin realizar ninguna operación.
        }

        /// <summary>
        /// Obtiene los presupuestos activos de un usuario específico.
        /// Un presupuesto se considera activo si su propiedad 'Activo' es true.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto activas para el usuario especificado.</returns>
        public async Task<IEnumerable<Presupuesto>> ObtenerActivosPorUsuarioAsync(int userId)
        {
            // Delega la operación al repositorio, filtrando por usuario y la propiedad Activo.
            return await _presupuestoRepository.GetByConditionAsync(p => p.UsuarioId == userId && p.Activo);
        }
    }
}