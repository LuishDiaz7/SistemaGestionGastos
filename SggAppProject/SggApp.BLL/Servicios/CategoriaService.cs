using SggApp.BLL.Interfaces;
using SggApp.DAL; 
using SggApp.DAL.Entidades; 
using SggApp.DAL.Repositorios; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Servicios
{
    /// <summary>
    /// Implementa la lógica de negocio para la gestión de categorías.
    /// Proporciona operaciones CRUD y validaciones específicas para categorías de usuario.
    /// </summary>
    public class CategoriaService : ICategoriaService
    {
        private readonly CategoriaRepository _categoriaRepository;
        private readonly GastoRepository _gastoRepository; // Inyectado para validar eliminación.
        private readonly UnitOfWork _unitOfWork; // Inyectado para gestionar transacciones.

        /// <summary>
        /// Inicializa una nueva instancia del servicio de categorías.
        /// </summary>
        /// <param name="categoriaRepository">Repositorio para acceder a los datos de categorías.</param>
        /// <param name="gastoRepository">Repositorio para acceder a los datos de gastos (usado para validaciones).</param>
        /// <param name="unitOfWork">Unidad de trabajo para gestionar transacciones de persistencia.</param>
        public CategoriaService(CategoriaRepository categoriaRepository, GastoRepository gastoRepository, UnitOfWork unitOfWork)
        {
            _categoriaRepository = categoriaRepository;
            _gastoRepository = gastoRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obtiene todas las categorías disponibles en el sistema.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Categoria.</returns>
        public async Task<IEnumerable<Categoria>> ObtenerTodasAsync()
        {
            // Delega la operación al repositorio de categorías.
            return await _categoriaRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtiene una categoría específica por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la categoría a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Categoria si se encuentra; de lo contrario, null.</returns>
        public async Task<Categoria> ObtenerPorIdAsync(int id)
        {
            // Delega la operación al repositorio de categorías.
            return await _categoriaRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene todas las categorías pertenecientes a un usuario específico.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario cuyas categorías se desean obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Categoria del usuario especificado.</returns>
        public async Task<IEnumerable<Categoria>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            // Delega la operación al repositorio, filtrando por el ID de usuario.
            return await _categoriaRepository.GetByConditionAsync(c => c.UsuarioId == usuarioId);
        }

        /// <summary>
        /// Agrega una nueva categoría al sistema.
        /// Incluye validación para asegurar que el nombre sea único para el usuario.
        /// </summary>
        /// <param name="categoria">La entidad Categoria a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="ArgumentException">Lanzada si ya existe una categoría con el mismo nombre para el usuario.</exception>
        public async Task AgregarAsync(Categoria categoria)
        {
            // Realiza una validación de negocio: verifica si ya existe una categoría con el mismo nombre para el usuario.
            if (await ExisteNombreParaUsuarioAsync(categoria.UsuarioId, categoria.Nombre))
            {
                // Lanza una excepción si se encuentra una duplicación.
                throw new ArgumentException("Ya existe una categoría con este nombre para el usuario.");
            }

            // Agrega la categoría a través del repositorio y guarda los cambios usando la Unidad de Trabajo.
            await _categoriaRepository.AddAsync(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Actualiza una categoría existente.
        /// Incluye validación para asegurar que el nuevo nombre sea único para el usuario (excluyendo la propia categoría).
        /// </summary>
        /// <param name="categoria">La entidad Categoria con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="ArgumentException">Lanzada si ya existe otra categoría con el mismo nombre para el usuario (excluyendo la que se está actualizando).</exception>
        public async Task ActualizarAsync(Categoria categoria)
        {
            // Realiza una validación de negocio: verifica si existe otra categoría con el mismo nombre para el usuario,
            // excluyendo la categoría que se está actualizando por su ID.
            var categoriasConMismoNombre = await _categoriaRepository.GetByConditionAsync(c =>
                c.UsuarioId == categoria.UsuarioId &&
                c.Nombre == categoria.Nombre &&
                c.Id != categoria.Id); // Excluye la categoría actual.

            if (categoriasConMismoNombre.Any())
            {
                // Lanza una excepción si se encuentra otra categoría con el mismo nombre.
                throw new ArgumentException("Ya existe una categoría con este nombre para el usuario.");
            }

            // Marca la categoría para ser actualizada a través del repositorio y guarda los cambios.
            _categoriaRepository.Update(categoria);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una categoría por su identificador.
        /// Incluye validación para prevenir la eliminación si tiene gastos asociados.
        /// </summary>
        /// <param name="id">El identificador de la categoría a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="InvalidOperationException">Lanzada si la categoría tiene gastos asociados.</exception>
        public async Task EliminarAsync(int id)
        {
            // Obtiene la categoría por su ID.
            var categoria = await _categoriaRepository.GetByIdAsync(id);
            if (categoria != null)
            {
                // Realiza una validación de negocio: verifica si hay gastos asociados a esta categoría.
                var gastosAsociados = await _gastoRepository.GetByConditionAsync(g => g.CategoriaId == id);
                if (gastosAsociados.Any())
                {
                    // Lanza una excepción si se encuentran gastos asociados.
                    throw new InvalidOperationException("No se puede eliminar la categoría porque tiene gastos asociados.");
                }

                // Elimina la categoría a través del repositorio y guarda los cambios.
                _categoriaRepository.Delete(categoria);
                await _unitOfWork.SaveChangesAsync();
            }
            // Si la categoría no existe, el método simplemente finaliza sin lanzar excepción.
        }

        /// <summary>
        /// Verifica si ya existe una categoría con un nombre específico para un usuario dado.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="nombre">El nombre de la categoría a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si ya existe una categoría con ese nombre para el usuario; de lo contrario, false.</returns>
        public async Task<bool> ExisteNombreParaUsuarioAsync(int usuarioId, string nombre)
        {
            // Busca categorías con el nombre y usuario especificados.
            var categorias = await _categoriaRepository.GetByConditionAsync(c =>
                c.UsuarioId == usuarioId &&
                c.Nombre == nombre);
            // Retorna true si se encuentra al menos una categoría, false en caso contrario.
            return categorias.Any();
        }
    }
}
