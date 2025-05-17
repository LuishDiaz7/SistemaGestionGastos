using System; 
using System.Threading.Tasks; 
using SggApp.DAL.Data; 
using SggApp.DAL.Repositorios; 

namespace SggApp.DAL 
{
    /// <summary>
    /// Implementa el patrón Unidad de Trabajo (Unit of Work).
    /// Gestiona una instancia de ApplicationDbContext y proporciona acceso centralizado a varios repositorios,
    /// coordinando las operaciones de base de datos y permitiendo guardar todos los cambios como una única transacción lógica.
    /// Implementa IDisposable para asegurar la correcta liberación de recursos del contexto.
    /// </summary>
    public class UnitOfWork : IDisposable
    {
        private readonly ApplicationDbContext _context; // Contexto de base de datos gestionado por la Unidad de Trabajo.
        private bool _disposed = false; // Flag para detectar llamadas redundantes a Dispose.

        // Campos privados para las instancias de los repositorios (para inicialización perezosa).
        private UsuarioRepository _usuarioRepository;
        private CategoriaRepository _categoriaRepository;
        private MonedaRepository _monedaRepository;
        private GastoRepository _gastoRepository;
        private PresupuestoRepository _presupuestoRepository;
        private TipoCambioRepository _tipoCambioRepository;

        /// <summary>
        /// Inicializa una nueva instancia de la Unidad de Trabajo.
        /// </summary>
        /// <param name="context">La instancia de ApplicationDbContext que será gestionada por esta Unidad de Trabajo.</param>
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;

            // Removido: Inicialización de repositorios aquí. Se inicializan perezosamente en las propiedades.
            // _usuarioRepository = new UsuarioRepository(_context);
            // _categoriaRepository = new CategoriaRepository(_context);
            // _monedaRepository = new MonedaRepository(_context);
            // _gastoRepository = new GastoRepository(_context);
            // _presupuestoRepository = new PresupuestoRepository(_context);
            // _tipoCambioRepository = new TipoCambioRepository(_context);
        }

        /// <summary>
        /// Obtiene una instancia del repositorio de Usuarios, inicializándola si es la primera vez que se accede.
        /// </summary>
        public UsuarioRepository UsuarioRepository
        {
            get
            {
                // Inicialización perezosa: si el repositorio aún no ha sido creado, lo crea.
                if (_usuarioRepository == null)
                {
                    _usuarioRepository = new UsuarioRepository(_context);
                }
                // Retorna la instancia del repositorio.
                return _usuarioRepository;
            }
        }

        /// <summary>
        /// Obtiene una instancia del repositorio de Categorías, inicializándola si es la primera vez que se accede.
        /// </summary>
        public CategoriaRepository CategoriaRepository
        {
            get
            {
                // Inicialización perezosa.
                if (_categoriaRepository == null)
                {
                    _categoriaRepository = new CategoriaRepository(_context);
                }
                // Retorna la instancia del repositorio.
                return _categoriaRepository;
            }
        }

        /// <summary>
        /// Obtiene una instancia del repositorio de Monedas, inicializándola si es la primera vez que se accede.
        /// </summary>
        public MonedaRepository MonedaRepository
        {
            get
            {
                // Inicialización perezosa.
                if (_monedaRepository == null)
                {
                    _monedaRepository = new MonedaRepository(_context);
                }
                // Retorna la instancia del repositorio.
                return _monedaRepository;
            }
        }

        /// <summary>
        /// Obtiene una instancia del repositorio de Gastos, inicializándola si es la primera vez que se accede.
        /// </summary>
        public GastoRepository GastoRepository
        {
            get
            {
                // Inicialización perezosa.
                if (_gastoRepository == null)
                {
                    _gastoRepository = new GastoRepository(_context);
                }
                // Retorna la instancia del repositorio.
                return _gastoRepository;
            }
        }

        /// <summary>
        /// Obtiene una instancia del repositorio de Presupuestos, inicializándola si es la primera vez que se accede.
        /// </summary>
        public PresupuestoRepository PresupuestoRepository
        {
            get
            {
                // Inicialización perezosa.
                if (_presupuestoRepository == null)
                {
                    _presupuestoRepository = new PresupuestoRepository(_context);
                }
                // Retorna la instancia del repositorio.
                return _presupuestoRepository;
            }
        }

        /// <summary>
        /// Obtiene una instancia del repositorio de Tipos de Cambio, inicializándola si es la primera vez que se accede.
        /// </summary>
        public TipoCambioRepository TipoCambioRepository
        {
            get
            {
                // Inicialización perezosa.
                if (_tipoCambioRepository == null)
                {
                    _tipoCambioRepository = new TipoCambioRepository(_context);
                }
                // Retorna la instancia del repositorio.
                return _tipoCambioRepository;
            }
        }

        /// <summary>
        /// Guarda todos los cambios pendientes realizados en el contexto de base de datos gestionado por esta Unidad de Trabajo.
        /// Esto constituye el punto de commit de la transacción lógica.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es el número de objetos de estado escritos en la base de datos.</returns>
        public async Task<int> SaveChangesAsync()
        {
            // Delega la operación de guardado asíncrono al contexto.
            return await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Implementación protegida del patrón Dispose para liberar recursos gestionados.
        /// </summary>
        /// <param name="disposing">Indica si el método fue llamado desde Dispose() (true) o desde el finalizador (false).</param>
        protected virtual void Dispose(bool disposing)
        {
            // Verifica si ya se ha ejecutado Dispose.
            if (!_disposed)
            {
                // Si disposing es true, libera los recursos gestionados (el DbContext).
                if (disposing)
                {
                    _context.Dispose(); // Libera el contexto de base de datos.
                }
            }
            // Marca como desechado.
            _disposed = true;
        }

        /// <summary>
        /// Implementación de la interfaz IDisposable.
        /// Libera los recursos utilizados por la Unidad de Trabajo.
        /// </summary>
        public void Dispose()
        {
            // Llama al método Dispose(bool) pasando true.
            Dispose(true);
            // Suprime la finalización para evitar que el recolector de basura llame al finalizador.
            GC.SuppressFinalize(this);
        }
    }
}