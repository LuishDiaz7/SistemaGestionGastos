using System; 
using System.Linq; 
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades; 

namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Implementa el repositorio específico para la entidad Presupuesto.
    /// Extiende el repositorio genérico y añade métodos de consulta particulares para presupuestos, incluyendo filtrado por fechas y carga de relaciones.
    /// </summary>
    public class PresupuestoRepository : GenericRepository<Presupuesto> // Hereda funcionalidad CRUD básica del repositorio genérico.
    {
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de presupuestos.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public PresupuestoRepository(ApplicationDbContext context) : base(context) // Pasa el contexto al constructor de la clase base.
        {
            // El cuerpo del constructor está vacío ya que la inicialización la maneja la clase base.
        }

        /// <summary>
        /// Obtiene los presupuestos de un usuario específico que no han finalizado (FechaFin es hoy o posterior).
        /// Incluye las entidades Categoria y Moneda relacionadas y ordena por FechaFin.
        /// Nota: El nombre del método sugiere "activo", pero la condición se basa en la fecha de fin, no en la propiedad 'Activo' de la entidad.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Presupuesto vigentes por fecha con sus relaciones cargadas.</returns>
        public async Task<IEnumerable<Presupuesto>> GetActiveByUsuarioIdAsync(int usuarioId)
        {
            var today = DateTime.Today;
            // Consulta el DbSet de Presupuestos en el contexto:
            return await _context.Presupuestos
                // Filtra por UsuarioId y donde la FechaFin sea hoy o en el futuro.
                .Where(p => p.UsuarioId == usuarioId && p.FechaFin >= today)
                // Incluye la entidad Categoria relacionada.
                .Include(p => p.Categoria)
                // Incluye la entidad Moneda relacionada.
                .Include(p => p.Moneda)
                // Ordena los resultados por la FechaFin de forma ascendente.
                .OrderBy(p => p.FechaFin)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un presupuesto específico por su identificador, incluyendo la carga de sus entidades relacionadas (Categoria y Moneda).
        /// </summary>
        /// <param name="id">El identificador del presupuesto.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Presupuesto con sus relaciones cargadas si se encuentra; de lo contrario, null.</returns>
        public async Task<Presupuesto?> GetByIdWithDetailsAsync(int id)
        {
            // Consulta el DbSet de Presupuestos en el contexto:
            return await _context.Presupuestos
                // Incluye la entidad Categoria relacionada.
                .Include(p => p.Categoria)
                // Incluye la entidad Moneda relacionada.
                .Include(p => p.Moneda)
                // Busca el primer presupuesto que coincida con el ID.
                .FirstOrDefaultAsync(p => p.Id == id);
            // Ejecuta la consulta de forma asíncrona y retorna el resultado o null.
        }

        /// <summary>
        /// Verifica si ya existe un presupuesto asociado a una categoría específica para un usuario dado,
        /// cuyo rango de fechas (FechaInicio a FechaFin) se superponga con un período dado.
        /// Nota: Esta verificación se aplica solo a presupuestos *específicos de categoría* (donde CategoriaId no es nulo).
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="categoriaId">El identificador de la categoría.</param>
        /// <param name="fechaInicio">La fecha de inicio del período de superposición a verificar.</param>
        /// <param name="fechaFin">La fecha de fin del período de superposición a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si se encuentra un presupuesto superpuesto; de lo contrario, false.</returns>
        public async Task<bool> ExistsForCategoryInPeriodAsync(int usuarioId, int categoriaId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Consulta el DbSet de Presupuestos en el contexto:
            return await _context.Presupuestos
                // Verifica si existe *algún* presupuesto que cumpla todas las condiciones:
                .AnyAsync(p => p.UsuarioId == usuarioId && // Pertenecer al usuario especificado.
                             p.CategoriaId == categoriaId && // Estar asociado a la categoría específica.
                                                             // Verificar superposición de rangos de fechas:
                                                             // Opción 1: El inicio del presupuesto está dentro del período dado O el fin del presupuesto está dentro del período dado.
                             ((p.FechaInicio <= fechaInicio && p.FechaFin >= fechaInicio) ||
                             (p.FechaInicio <= fechaFin && p.FechaFin >= fechaFin) ||
                             // Opción 2: El período dado está completamente contenido dentro del presupuesto.
                             (p.FechaInicio >= fechaInicio && p.FechaFin <= fechaFin)));
            // Ejecuta la verificación de existencia de forma asíncrona.
        }

        public async Task<IEnumerable<Presupuesto>> GetByUsuarioIdWithDetailsAsync(int usuarioId)
        {
            // Consulta el DbSet de Presupuestos en el contexto
            return await _context.Presupuestos
                // Filtra por UsuarioId
                .Where(p => p.UsuarioId == usuarioId)
                // Incluye la entidad Categoria relacionada
                .Include(p => p.Categoria)
                // Incluye la entidad Moneda relacionada
                .Include(p => p.Moneda)
                // Ordena los resultados (opcional)
                .OrderByDescending(p => p.FechaCreacion)
                // Ejecuta la consulta de forma asíncrona y retorna una lista
                .ToListAsync();
        }
    }
}
