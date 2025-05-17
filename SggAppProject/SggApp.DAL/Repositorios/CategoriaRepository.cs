using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades; 

namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Implementa el repositorio específico para la entidad Categoria.
    /// Extiende el repositorio genérico y añade métodos de consulta particulares para categorías.
    /// </summary>
    public class CategoriaRepository : GenericRepository<Categoria> // Hereda funcionalidad CRUD básica del repositorio genérico.
    {
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de categorías.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public CategoriaRepository(ApplicationDbContext context) : base(context) // Pasa el contexto al constructor de la clase base.
        {
            // El cuerpo del constructor está vacío ya que la inicialización la maneja la clase base.
        }

        /// <summary>
        /// Obtiene todas las categorías activas pertenecientes a un usuario específico.
        /// Las categorías se ordenan por nombre.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Categoria.</returns>
        public async Task<IEnumerable<Categoria>> GetByUsuarioIdAsync(int usuarioId)
        {
            // Consulta el DbSet de Categorias en el contexto:
            return await _context.Categorias
                // Filtra por UsuarioId y por categorías que estén activas.
                .Where(c => c.UsuarioId == usuarioId && c.Activa)
                // Ordena los resultados por el nombre de la categoría.
                .OrderBy(c => c.Nombre)
                // Ejecuta la consulta de forma asíncrona y retorna una lista.
                .ToListAsync();
        }

        /// <summary>
        /// Verifica si ya existe una categoría con un nombre específico (ignorando mayúsculas/minúsculas) para un usuario dado.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="nombre">El nombre de la categoría a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si existe una categoría con ese nombre para el usuario; de lo contrario, false.</returns>
        public async Task<bool> ExistsByNameForUserAsync(int usuarioId, string nombre)
        {
            // Consulta el DbSet de Categorias en el contexto:
            return await _context.Categorias
                // Verifica si existe *alguna* categoría que cumpla la condición:
                // - Pertenecer al usuario especificado.
                // - Tener el mismo nombre (comparando en minúsculas para hacer la comparación insensible a mayúsculas).
                .AnyAsync(c => c.UsuarioId == usuarioId && c.Nombre.ToLower() == nombre.ToLower());
            // Ejecuta la verificación de existencia de forma asíncrona.
        }
    }
}