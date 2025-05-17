using SggApp.DAL.Entidades;
using System; 
using System.Collections.Generic; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Interfaces
{
    /// <summary>
    /// Define el contrato para los servicios relacionados con la gestión de presupuestos.
    /// Establece las operaciones de negocio disponibles para interactuar con los presupuestos, incluyendo consultas, cálculos de uso y alertas.
    /// </summary>
    public interface IPresupuestoService
    {
        /// <summary>
        /// Obtiene una colección de todos los presupuestos disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto.</returns>
        Task<IEnumerable<Presupuesto>> ObtenerTodosAsync();

        /// <summary>
        /// Obtiene un presupuesto específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del presupuesto a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Presupuesto si se encuentra; de lo contrario, null.</returns>
        Task<Presupuesto> ObtenerPorIdAsync(int id); // Nota: Si la implementación puede devolver null, el tipo de retorno en la interfaz podría ser Task<Presupuesto?>.

        /// <summary>
        /// Obtiene una colección de presupuestos pertenecientes a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario cuyos presupuestos se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto del usuario especificado.</returns>
        Task<IEnumerable<Presupuesto>> ObtenerPorUsuarioAsync(int usuarioId);

        /// <summary>
        /// Obtiene una colección de presupuestos de un usuario que están vigentes en la fecha actual.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto vigentes para el usuario especificado.</returns>
        Task<IEnumerable<Presupuesto>> ObtenerPresupuestosVigentesAsync(int usuarioId);

        /// <summary>
        /// Obtiene una colección de presupuestos asociados a una categoría específica.
        /// </summary>
        /// <param name="categoriaId">El identificador de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto asociadas a la categoría especificada.</returns>
        Task<IEnumerable<Presupuesto>> ObtenerPresupuestosPorCategoriaAsync(int categoriaId);

        /// <summary>
        /// Calcula el monto total de gastos asociados a un presupuesto específico dentro de su período y categoría (si aplica), convirtiendo montos a la moneda del presupuesto.
        /// La implementación puede lanzar excepciones si el presupuesto no se encuentra.
        /// </summary>
        /// <param name="presupuestoId">El identificador del presupuesto.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el monto total de gastos relevantes convertido a la moneda del presupuesto.</returns>
        Task<decimal> ObtenerGastoActualAsync(int presupuestoId);

        /// <summary>
        /// Calcula el porcentaje del límite de un presupuesto que ha sido consumido por los gastos relevantes.
        /// La implementación puede lanzar excepciones si el presupuesto no se encuentra.
        /// </summary>
        /// <param name="presupuestoId">El identificador del presupuesto.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el porcentaje de consumo del presupuesto.</returns>
        Task<decimal> ObtenerPorcentajeConsumidoAsync(int presupuestoId);

        /// <summary>
        /// Verifica si el gasto actual de un presupuesto ha superado el nivel de alerta configurado.
        /// </summary>
        /// <param name="presupuestoId">El identificador del presupuesto a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si el porcentaje consumido es mayor o igual al umbral de alerta; de lo contrario, false.</returns>
        Task<bool> VerificarAlertaPresupuestoAsync(int presupuestoId);

        /// <summary>
        /// Agrega un nuevo presupuesto al sistema.
        /// </summary>
        /// <param name="presupuesto">La entidad Presupuesto a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task AgregarAsync(Presupuesto presupuesto);

        /// <summary>
        /// Actualiza un presupuesto existente.
        /// </summary>
        /// <param name="presupuesto">La entidad Presupuesto con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task ActualizarAsync(Presupuesto presupuesto);

        /// <summary>
        /// Elimina un presupuesto por su identificador.
        /// </summary>
        /// <param name="id">El identificador del presupuesto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        Task EliminarAsync(int id);

        /// <summary>
        /// Obtiene los presupuestos activos de un usuario específico.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto activas para el usuario especificado.</returns>
        Task<IEnumerable<Presupuesto>> ObtenerActivosPorUsuarioAsync(int userId);
    }
}
