using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades; 

namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Implementa el repositorio específico para la entidad Moneda.
    /// Extiende el repositorio genérico y añade métodos de consulta particulares para monedas.
    /// </summary>
    public class MonedaRepository : GenericRepository<Moneda> // Hereda funcionalidad CRUD básica del repositorio genérico.
    {
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de monedas.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public MonedaRepository(ApplicationDbContext context) : base(context) // Pasa el contexto al constructor de la clase base.
        {
            // El cuerpo del constructor está vacío ya que la inicialización la maneja la clase base.
        }

        /// <summary>
        /// Obtiene todas las monedas que están marcadas como activas.
        /// Las monedas se ordenan por nombre.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Moneda activas.</returns>
        public async Task<IEnumerable<Moneda>> GetActiveCurrenciesAsync()
        {
            // Consulta el DbSet de Monedas en el contexto:
            return await _context.Monedas
                // Filtra por monedas que están activas.
                .Where(m => m.Activa)
                // Ordena los resultados por el nombre de la moneda.
                .OrderBy(m => m.Nombre)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene una moneda específica por su código ISO 4217, realizando una comparación insensible a mayúsculas/minúsculas.
        /// </summary>
        /// <param name="codigo">El código de la moneda (ej. "USD", "COP").</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Moneda si se encuentra; de lo contrario, null.</returns>
        public async Task<Moneda?> GetByCodigo(string codigo)
        {
            // Consulta el DbSet de Monedas en el contexto:
            return await _context.Monedas
                // Busca la primera moneda cuyo código coincida (comparando en mayúsculas para insensibilidad).
                .FirstOrDefaultAsync(m => m.Codigo.ToUpper() == codigo.ToUpper());
            // Ejecuta la consulta de forma asíncrona y retorna la primera coincidencia o null.
        }
    }
}