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
    /// Implementa la lógica de negocio para la gestión de datos de la entidad Usuario.
    /// Complementa las funcionalidades principales de gestión de cuentas proporcionadas por ASP.NET Core Identity,
    /// centrándose en la persistencia y recuperación de datos del usuario.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly UsuarioRepository _usuarioRepository; // Repositorio para acceder a los datos de usuarios.

        /// <summary>
        /// Inicializa una nueva instancia del servicio de usuarios.
        /// </summary>
        /// <param name="usuarioRepository">Repositorio para acceder a los datos de usuarios.</param>
        public UsuarioService(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Obtiene todos los usuarios registrados en el sistema.
        /// (Nota: Este método podría ser restringido a administradores en una aplicación real).
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Usuario.</returns>
        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            // Delega la operación al repositorio de usuarios.
            return await _usuarioRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador.
        /// </summary>
        /// <param name="id">El identificador del usuario a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Usuario si se encuentra; de lo contrario, null.</returns>
        public async Task<Usuario?> ObtenerPorIdAsync(int id)
        {
            // Delega la operación al repositorio de usuarios.
            return await _usuarioRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene un usuario específico por su dirección de correo electrónico.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Usuario si se encuentra; de lo contrario, null.</returns>
        public async Task<Usuario?> ObtenerPorEmailAsync(string email)
        {
            // Delega la operación al repositorio, filtrando por el email.
            var usuarios = await _usuarioRepository.GetByConditionAsync(u => u.Email == email);
            // Retorna el primer resultado encontrado o null si la colección está vacía.
            return usuarios.FirstOrDefault();
        }

        /// <summary>
        /// Agrega un nuevo usuario a la base de datos a través del repositorio.
        /// (Nota: La creación completa de un usuario con ASP.NET Core Identity generalmente se realiza a través de UserManager en la capa de presentación/controlador).
        /// </summary>
        /// <param name="usuario">La entidad Usuario a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task AgregarAsync(Usuario usuario)
        {
            // Delega la operación al repositorio.
            await _usuarioRepository.AddAsync(usuario);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Actualiza los datos de un usuario existente a través del repositorio.
        /// </summary>
        /// <param name="usuario">La entidad Usuario con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task ActualizarAsync(Usuario usuario)
        {
            // Delega la operación al repositorio.
            _usuarioRepository.Update(usuario);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Elimina un usuario por su identificador a través del repositorio.
        /// Verifica si el usuario existe antes de intentar eliminar.
        /// (Nota: La eliminación de un usuario que puede tener datos relacionados como gastos o presupuestos
        /// requeriría un manejo más complejo, como eliminación en cascada en la base de datos o lógica adicional aquí/en el controlador).
        /// </summary>
        /// <param name="id">El identificador del usuario a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task EliminarAsync(int id)
        {
            // Obtiene el usuario por su ID.
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            // Si el usuario existe, lo marca para eliminación a través del repositorio.
            if (usuario != null)
            {
                _usuarioRepository.Delete(usuario);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
            // Si el usuario no existe, el método finaliza sin realizar ninguna operación.
        }

        /// <summary>
        /// Verifica si existe un usuario con una dirección de correo electrónico específica.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si existe un usuario con ese email; de lo contrario, false.</returns>
        public async Task<bool> ExisteEmailAsync(string email)
        {
            // Delega la operación al repositorio, filtrando por el email y verificando si existe alguno.
            var usuarios = await _usuarioRepository.GetByConditionAsync(u => u.Email == email);
            return usuarios.Any();
        }

        /// <summary>
        /// Cambia la moneda predeterminada para un usuario específico.
        /// Si el usuario no se encuentra, el método no realiza ninguna acción.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario.</param>
        /// <param name="monedaId">El identificador de la nueva moneda predeterminada.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task CambiarMonedaPredeterminadaAsync(int usuarioId, int monedaId)
        {
            // Obtiene el usuario por su ID.
            var usuario = await _usuarioRepository.GetByIdAsync(usuarioId);
            // Si el usuario existe, actualiza su MonedaPredeterminadaId.
            if (usuario != null)
            {
                usuario.MonedaPredeterminadaId = monedaId;
                _usuarioRepository.Update(usuario);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
        }
    }
}