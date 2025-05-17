
using System.Linq; 
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades; 


namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Implementa el repositorio específico para la entidad Usuario, que extiende IdentityUser.
    /// Extiende el repositorio genérico y añade métodos de consulta particulares para usuarios, incluyendo búsqueda por email y carga de relaciones.
    /// Trabaja con el DbSet 'Users' del ApplicationDbContext.
    /// </summary>
    public class UsuarioRepository : GenericRepository<Usuario> // Hereda funcionalidad CRUD básica del repositorio genérico.
    {
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de usuarios.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación, que contiene el DbSet 'Users'.</param>
        public UsuarioRepository(ApplicationDbContext context) : base(context) // Pasa el contexto al constructor de la clase base.
        {
            // El cuerpo del constructor está vacío ya que la inicialización la maneja la clase base.
        }

        /// <summary>
        /// Obtiene un usuario específico por su dirección de correo electrónico.
        /// Utiliza el DbSet 'Users' del contexto.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Usuario si se encuentra; de lo contrario, null.</returns>
        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            // Consulta el DbSet 'Users' en el contexto (que es el DbSet<Usuario>):
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            // Ejecuta la consulta de forma asíncrona y retorna el primer usuario con el email dado o null.
        }

        /// <summary>
        /// Verifica si ya existe un usuario con una dirección de correo electrónico específica.
        /// Utiliza el DbSet 'Users' del contexto.
        /// </summary>
        /// <param name="email">La dirección de correo electrónico a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es true si existe un usuario con ese email; de lo contrario, false.</returns>
        public async Task<bool> EmailExistsAsync(string email)
        {
            // Consulta el DbSet 'Users' en el contexto:
            return await _context.Users.AnyAsync(u => u.Email == email);
            // Ejecuta la verificación de existencia de forma asíncrona.
        }

        /// <summary>
        /// Obtiene todos los usuarios, incluyendo la carga de su colección de gastos asociados.
        /// Utiliza el DbSet 'Users' del contexto.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de entidades Usuario con sus propiedades de navegación Gastos cargadas.</returns>
        public async Task<IEnumerable<Usuario>> GetAllWithGastosAsync()
        {
            // Consulta el DbSet 'Users' en el contexto:
            return await _context.Users
                // Incluye la colección de entidades Gasto relacionadas.
                .Include(u => u.Gastos)
                // Ejecuta la consulta de forma asíncrona y retorna una lista de usuarios.
                .ToListAsync();
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador, incluyendo la carga de todas sus entidades relacionadas (Gastos, Presupuestos, Moneda Predeterminada).
        /// Utiliza el DbSet 'Users' del contexto.
        /// </summary>
        /// <param name="id">El identificador del usuario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad Usuario con sus relaciones cargadas si se encuentra; de lo contrario, null.</returns>
        public async Task<Usuario?> GetByIdWithDetailsAsync(int id)
        {
            // Consulta el DbSet 'Users' en el contexto:
            return await _context.Users
                // Incluye la colección de entidades Gasto relacionadas.
                .Include(u => u.Gastos)
                // Incluye la colección de entidades Presupuesto relacionadas.
                .Include(u => u.Presupuestos)
                // Incluye la entidad Moneda Predeterminada relacionada.
                .Include(u => u.MonedaPredeterminada)
                // Busca el primer usuario que coincida con el ID.
                .FirstOrDefaultAsync(u => u.Id == id);
            // Ejecuta la consulta de forma asíncrona y retorna el resultado o null.
        }
    }
}
