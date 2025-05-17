using Microsoft.EntityFrameworkCore; 
using SggApp.BLL.Interfaces; 
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades; 
using SggApp.DAL.Repositorios; 
using System; 
using System.Collections.Generic; 
using System.Linq; 
using System.Threading.Tasks; 

namespace SggApp.BLL.Servicios
{
    /// <summary>
    /// Implementa la lógica de negocio para la gestión de monedas.
    /// Proporciona operaciones de consulta y manipulación de monedas, incluyendo validaciones de unicidad y verificación de uso.
    /// </summary>
    public class MonedaService : IMonedaService
    {
        private readonly MonedaRepository _monedaRepository; // Repositorio para acceder a los datos de monedas.
        private readonly ApplicationDbContext _context; // Acceso directo al contexto para verificaciones de relaciones.

        /// <summary>
        /// Inicializa una nueva instancia del servicio de monedas.
        /// </summary>
        /// <param name="monedaRepository">Repositorio para acceder a los datos de monedas.</param>
        /// <param name="context">Contexto de base de datos de la aplicación.</param>
        public MonedaService(MonedaRepository monedaRepository, ApplicationDbContext context)
        {
            _monedaRepository = monedaRepository;
            _context = context;
        }

        /// <summary>
        /// Obtiene todas las monedas disponibles en el sistema.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Moneda.</returns>
        public async Task<IEnumerable<Moneda>> ObtenerTodasAsync()
        {
            // Delega la operación al repositorio de monedas.
            return await _monedaRepository.GetAllAsync();
        }

        /// <summary>
        /// Obtiene una moneda específica por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la moneda a obtener.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Moneda si se encuentra; de lo contrario, null.</returns>
        public async Task<Moneda?> ObtenerPorIdAsync(int id)
        {
            // Delega la operación al repositorio de monedas.
            return await _monedaRepository.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene una moneda específica por su código ISO 4217.
        /// </summary>
        /// <param name="codigo">El código de la moneda (ej. "USD", "COP").</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Moneda si se encuentra; de lo contrario, null.</returns>
        public async Task<Moneda?> ObtenerPorCodigoAsync(string codigo)
        {
            // Delega la operación al repositorio, filtrando por el código.
            var monedas = await _monedaRepository.GetByConditionAsync(m => m.Codigo == codigo);
            // Retorna el primer resultado encontrado o null si la colección está vacía.
            return monedas.FirstOrDefault();
        }

        /// <summary>
        /// Obtiene todas las monedas que están marcadas como activas.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Moneda activas.</returns>
        public async Task<IEnumerable<Moneda>> ObtenerActivasAsync()
        {
            // Delega la operación al repositorio, filtrando por la propiedad Activa.
            return await _monedaRepository.GetByConditionAsync(m => m.Activa);
        }

        /// <summary>
        /// Agrega una nueva moneda al sistema.
        /// Incluye validación para asegurar que el código de la moneda sea único.
        /// </summary>
        /// <param name="moneda">La entidad Moneda a agregar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="ArgumentException">Lanzada si ya existe una moneda con el mismo código.</exception>
        public async Task AgregarAsync(Moneda moneda)
        {
            // Realiza una validación de negocio: verifica si ya existe una moneda con el mismo código.
            var monedasConMismoCodigo = await _monedaRepository.GetByConditionAsync(m => m.Codigo == moneda.Codigo);
            if (monedasConMismoCodigo.Any())
            {
                // Lanza una excepción si se encuentra una duplicación por código.
                throw new ArgumentException("Ya existe una moneda con este código.");
            }

            // Agrega la moneda a través del repositorio.
            await _monedaRepository.AddAsync(moneda);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Actualiza una moneda existente.
        /// Incluye validación para asegurar que el nuevo código sea único (excluyendo la propia moneda que se actualiza).
        /// </summary>
        /// <param name="moneda">La entidad Moneda con los datos actualizados.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        /// <exception cref="ArgumentException">Lanzada si ya existe otra moneda con el mismo código (excluyendo la que se está actualizando).</exception>
        public async Task ActualizarAsync(Moneda moneda)
        {
            // Realiza una validación de negocio: verifica si existe otra moneda con el mismo código,
            // excluyendo la moneda que se está actualizando por su ID.
            var monedasConMismoCodigo = await _monedaRepository.GetByConditionAsync(m =>
                m.Codigo == moneda.Codigo &&
                m.Id != moneda.Id); // Excluye la moneda actual.

            if (monedasConMismoCodigo.Any())
            {
                // Lanza una excepción si se encuentra otra moneda con el mismo código.
                throw new ArgumentException("Ya existe otra moneda con este código.");
            }

            // Marca la moneda para ser actualizada a través del repositorio.
            _monedaRepository.Update(moneda);
            // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
        }

        /// <summary>
        /// Desactiva una moneda marcando su propiedad 'Activa' como false.
        /// Si la moneda no existe, el método no realiza ninguna acción.
        /// </summary>
        /// <param name="id">El identificador de la moneda a desactivar.</param>
        /// <returns>Una tarea que representa la operación asíncrona.</returns>
        public async Task DesactivarAsync(int id)
        {
            // Obtiene la moneda por su ID.
            var moneda = await _monedaRepository.GetByIdAsync(id);
            // Si la moneda existe, la marca como inactiva.
            if (moneda != null)
            {
                moneda.Activa = false;
                _monedaRepository.Update(moneda);
                // Nota: Se asume que el repositorio o la Unidad de Trabajo gestionan SaveChangesAsync().
            }
        }

        /// <summary>
        /// Elimina una moneda por su identificador si no tiene registros asociados (gastos, presupuestos, usuarios).
        /// </summary>
        /// <param name="id">El identificador de la moneda a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Retorna true si la moneda fue eliminada exitosamente;
        /// retorna false si la moneda tiene registros asociados o no fue encontrada.</returns>
        public async Task<bool> EliminarAsync(int id)
        {
            // Verifica si la moneda tiene registros asociados utilizando un método auxiliar.
            if (await MonedaTieneRegistrosAsociadosAsync(id))
            {
                // Retorna false si hay registros asociados.
                return false;
            }

            // Obtiene la moneda a eliminar por su ID.
            var monedaToDelete = await _monedaRepository.GetByIdAsync(id);

            // Retorna false si la moneda no fue encontrada.
            if (monedaToDelete == null)
            {
                return false;
            }

            // Marca la moneda para ser eliminada a través del repositorio.
            _monedaRepository.Delete(monedaToDelete);

            // Guarda los cambios en la base de datos utilizando el contexto.
            await _context.SaveChangesAsync();

            // Retorna true indicando que la eliminación fue exitosa.
            return true;
        }

        /// <summary>
        /// Verifica si una moneda está siendo utilizada como moneda en Gastos, Presupuestos o como moneda predeterminada en Usuarios.
        /// </summary>
        /// <param name="monedaId">El identificador de la moneda a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si la moneda tiene al menos un registro asociado; de lo contrario, false.</returns>
        public async Task<bool> MonedaTieneRegistrosAsociadosAsync(int monedaId)
        {
            // Consulta directamente el contexto para verificar la existencia de registros asociados en las tablas relevantes.
            return await _context.Gastos.AnyAsync(g => g.MonedaId == monedaId) ||
                   await _context.Presupuestos.AnyAsync(p => p.MonedaId == monedaId) ||
                   await _context.Users.AnyAsync(u => u.MonedaPredeterminadaId == monedaId);
        }

        /// <summary>
        /// Cuenta el número total de registros (Gastos, Presupuestos, Usuarios) que utilizan una moneda específica.
        /// </summary>
        /// <param name="monedaId">El identificador de la moneda a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el número total de registros asociados.</returns>
        public async Task<int> ContarRegistrosAsociadosAsync(int monedaId)
        {
            // Consulta directamente el contexto para contar los registros asociados en cada tabla relevante.
            var gastos = await _context.Gastos.CountAsync(g => g.MonedaId == monedaId);
            var presupuestos = await _context.Presupuestos.CountAsync(p => p.MonedaId == monedaId);
            var usuarios = await _context.Users.CountAsync(u => u.MonedaPredeterminadaId == monedaId);

            // Retorna la suma de los conteos.
            return gastos + presupuestos + usuarios;
        }
    }
}