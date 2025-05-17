using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Data;
using SggApp.DAL.Entidades; 

namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Implementa el repositorio específico para la entidad Gasto.
    /// Extiende el repositorio genérico y añade métodos de consulta particulares para gastos, incluyendo filtrado, ordenación y carga de relaciones.
    /// </summary>
    public class GastoRepository : GenericRepository<Gasto> // Hereda funcionalidad CRUD básica del repositorio genérico.
    {
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de gastos.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public GastoRepository(ApplicationDbContext context) : base(context) // Pasa el contexto al constructor de la clase base.
        {
            // El cuerpo del constructor está vacío ya que la inicialización la maneja la clase base.
        }

        /// <summary>
        /// Obtiene los gastos de un usuario específico dentro de un rango de fechas (inclusivo).
        /// Este método realiza una consulta directa sin incluir relaciones.
        /// </summary>
        /// <param name="userId">El identificador del usuario.</param>
        /// <param name="startDate">La fecha de inicio del rango.</param>
        /// <param name="endDate">La fecha de fin del rango.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto.</returns>
        public async Task<IEnumerable<Gasto>> GetByUsuarioYRangoFechaAsync(int userId, DateTime startDate, DateTime endDate)
        {
            // Consulta el DbSet de Gastos en el contexto:
            return await _context.Gastos
                // Filtra los gastos por el ID de usuario y dentro del rango de fechas especificado (incluyendo los límites).
                .Where(g => g.UsuarioId == userId && g.Fecha >= startDate && g.Fecha <= endDate)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los gastos pertenecientes a un usuario específico.
        /// Este método realiza una consulta directa sin incluir relaciones.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto del usuario especificado.</returns>
        public async Task<IEnumerable<Gasto>> GetByUsuarioIdAsync(int usuarioId)
        {
            // Consulta el DbSet de Gastos en el contexto:
            return await _context.Gastos
                // Filtra los gastos por el ID de usuario.
                .Where(g => g.UsuarioId == usuarioId)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene los gastos de un usuario específico dentro de un rango de fechas (inclusivo), ordenados descendentemente por fecha.
        /// Este método realiza una consulta directa sin incluir relaciones.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango.</param>
        /// <param name="fechaFin">La fecha de fin del rango.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto.</returns>
        public async Task<IEnumerable<Gasto>> GetByDateRangeAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Consulta el DbSet de Gastos en el contexto:
            return await _context.Gastos
                // Filtra los gastos por el ID de usuario y dentro del rango de fechas especificado.
                .Where(g => g.UsuarioId == usuarioId && g.Fecha >= fechaInicio && g.Fecha <= fechaFin)
                // Ordena los resultados por fecha de forma descendente.
                .OrderByDescending(g => g.Fecha)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene todos los gastos pertenecientes a un usuario específico, incluyendo la carga de sus entidades relacionadas (Categoría y Moneda).
        /// Los gastos se ordenan descendentemente por fecha.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Gasto con sus propiedades de navegación Categoria y Moneda cargadas.</returns>
        public async Task<IEnumerable<Gasto>> GetWithDetailsAsync(int usuarioId)
        {
            // Consulta el DbSet de Gastos en el contexto:
            return await _context.Gastos
                // Filtra los gastos por el ID de usuario.
                .Where(g => g.UsuarioId == usuarioId)
                // Incluye la entidad Categoria relacionada.
                .Include(g => g.Categoria)
                // Incluye la entidad Moneda relacionada.
                .Include(g => g.Moneda)
                // Ordena los resultados por fecha de forma descendente.
                .OrderByDescending(g => g.Fecha)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene el monto total de gastos de un usuario dentro de un rango de fechas, agrupados por el nombre de la categoría.
        /// Incluye la carga de la entidad Categoria para agrupar por nombre.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="fechaInicio">La fecha de inicio del rango.</param>
        /// <param name="fechaFin">La fecha de fin del rango.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es un diccionario donde la clave es el nombre de la categoría y el valor es el monto total de gastos en esa categoría.</returns>
        public async Task<Dictionary<string, decimal>> GetTotalByCategoriaAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Consulta el DbSet de Gastos en el contexto:
            var result = await _context.Gastos
                // Filtra los gastos por usuario y rango de fechas.
                .Where(g => g.UsuarioId == usuarioId && g.Fecha >= fechaInicio && g.Fecha <= fechaFin)
                // Incluye la entidad Categoria para poder acceder a su nombre.
                .Include(g => g.Categoria)
                // Agrupa los gastos por el nombre de la categoría asociada.
                .GroupBy(g => g.Categoria.Nombre)
                // Selecciona para cada grupo un objeto anónimo con el nombre de la Categoría (la clave del grupo) y la suma de los montos de los gastos en ese grupo.
                .Select(g => new { Categoria = g.Key, Total = g.Sum(x => x.Monto) })
                // Convierte el resultado agrupado y seleccionado a un diccionario asíncronamente.
                .ToDictionaryAsync(k => k.Categoria, v => v.Total);

            return result;
        }
    }
}
