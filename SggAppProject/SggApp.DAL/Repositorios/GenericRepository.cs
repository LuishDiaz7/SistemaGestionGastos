using Microsoft.EntityFrameworkCore; 
using System; 
using System.Collections.Generic; 
using System.Linq;
using System.Linq.Expressions; 
using System.Threading.Tasks; 
using SggApp.DAL.Data; 

namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Proporciona una implementación genérica del patrón repositorio para operaciones de acceso a datos comunes.
    /// Permite realizar operaciones CRUD básicas y consultas condicionales para cualquier entidad.
    /// </summary>
    /// <typeparam name="TEntity">El tipo de entidad sobre el que opera el repositorio. Debe ser una clase.</typeparam>
    public class GenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// El contexto de base de datos de la aplicación utilizado por el repositorio.
        /// </summary>
        protected readonly ApplicationDbContext _context;

        /// <summary>
        /// El DbSet de Entity Framework Core asociado al tipo de entidad TEntity.
        /// </summary>
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Inicializa una nueva instancia del repositorio genérico.
        /// Configura el contexto de base de datos y el DbSet para la entidad especificada.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>(); // Obtiene el DbSet correspondiente al tipo de entidad.
        }

        /// <summary>
        /// Obtiene todos los registros de la entidad de forma asíncrona.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de todas las entidades.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            // Ejecuta la consulta ToListAsync() en el DbSet.
            return await _dbSet.ToListAsync();
        }

        /// <summary>
        /// Obtiene un registro específico por su identificador (clave primaria) de forma asíncrona.
        /// Se espera que el identificador sea un entero.
        /// </summary>
        /// <param name="id">El identificador del registro.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad si se encuentra; de lo contrario, null.</returns>
        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            // Utiliza FindAsync para buscar por clave primaria.
            return await _dbSet.FindAsync(id);
        }

        /// <summary>
        /// Obtiene registros de la entidad que cumplen una condición específica de forma asíncrona.
        /// </summary>
        /// <param name="predicate">Una expresión lambda que define la condición a aplicar (cláusula Where).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades que cumplen la condición.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetByConditionAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Aplica el filtro y ejecuta ToListAsync().
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Agrega un nuevo registro a la base de datos de forma asíncrona.
        /// Guarda los cambios inmediatamente en la base de datos.
        /// </summary>
        /// <param name="entity">La entidad a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public virtual async Task AddAsync(TEntity entity)
        {
            // Agrega la entidad al DbSet.
            await _dbSet.AddAsync(entity);
            // Guarda los cambios de forma asíncrona en el contexto.
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza un registro existente en la base de datos.
        /// Marca la entidad como modificada y guarda los cambios inmediatamente (de forma síncrona).
        /// </summary>
        /// <param name="entity">La entidad con los datos actualizados.</param>
        public virtual void Update(TEntity entity)
        {
            // Marca la entidad como modificada.
            _dbSet.Update(entity);
            // Guarda los cambios de forma síncrona en el contexto.
            _context.SaveChanges();
        }

        /// <summary>
        /// Elimina un registro de la base de datos.
        /// Marca la entidad para eliminación y guarda los cambios inmediatamente (de forma síncrona).
        /// </summary>
        /// <param name="entity">La entidad a eliminar.</param>
        public virtual void Delete(TEntity entity)
        {
            // Marca la entidad para ser eliminada.
            _dbSet.Remove(entity);
            // Guarda los cambios de forma síncrona en el contexto.
            _context.SaveChanges();
        }

        /// <summary>
        /// Verifica de forma asíncrona si existe al menos un registro que cumpla una condición.
        /// </summary>
        /// <param name="predicate">Una expresión lambda que define la condición a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si existe al menos un registro que cumple la condición; de lo contrario, false.</returns>
        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            // Ejecuta la verificación AnyAsync() en el DbSet con la condición.
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Cuenta de forma asíncrona el número total de registros o aquellos que cumplen una condición.
        /// </summary>
        /// <param name="predicate">Una expresión lambda opcional que define la condición para contar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el número de registros.</returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null)
        {
            // Si no se proporciona predicado, cuenta todos los registros. Si se proporciona, cuenta solo los que cumplen la condición.
            return predicate == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(predicate);
        }
    }
}