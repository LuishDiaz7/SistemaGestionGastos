using System.Linq; 
using System.Threading.Tasks; 
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades; 


namespace SggApp.DAL.Repositorios
{
    /// <summary>
    /// Implementa el repositorio específico para la entidad TipoCambio.
    /// Extiende el repositorio genérico y añade métodos de consulta particulares para tipos de cambio.
    /// </summary>
    public class TipoCambioRepository : GenericRepository<TipoCambio> // Hereda funcionalidad CRUD básica del repositorio genérico.
    {
        /// <summary>
        /// Inicializa una nueva instancia del repositorio de tipos de cambio.
        /// </summary>
        /// <param name="context">El contexto de base de datos de la aplicación.</param>
        public TipoCambioRepository(ApplicationDbContext context) : base(context) // Pasa el contexto al constructor de la clase base.
        {
            // El cuerpo del constructor está vacío ya que la inicialización la maneja la clase base.
        }

        /// <summary>
        /// Obtiene un tipo de cambio específico buscando por los identificadores de la moneda de origen y destino.
        /// </summary>
        /// <param name="monedaOrigenId">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestinoId">El identificador de la moneda de destino.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio si se encuentra; de lo contrario, null.</returns>
        public async Task<TipoCambio?> GetExchangeRateAsync(int monedaOrigenId, int monedaDestinoId)
        {
            // Consulta el DbSet de TiposCambio en el contexto:
            return await _context.TiposCambio
                // Busca el primer tipo de cambio que coincida con los IDs de origen y destino.
                .FirstOrDefaultAsync(tc => tc.MonedaOrigenId == monedaOrigenId &&
                                           tc.MonedaDestinoId == monedaDestinoId);
            // Ejecuta la consulta de forma asíncrona y retorna el resultado o null.
        }

        /// <summary>
        /// Obtiene el tipo de cambio más reciente actualizado para un par de monedas específico.
        /// Ordena por fecha de actualización descendente y toma el primero.
        /// </summary>
        /// <param name="monedaOrigenId">El identificador de la moneda de origen.</param>
        /// <param name="monedaDestinoId">El identificador de la moneda de destino.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio más reciente si se encuentra; de lo contrario, null.</returns>
        public async Task<TipoCambio?> GetLatestExchangeRateAsync(int monedaOrigenId, int monedaDestinoId)
        {
            // Consulta el DbSet de TiposCambio en el contexto:
            return await _context.TiposCambio
                // Filtra por los IDs de moneda de origen y destino.
                .Where(tc => tc.MonedaOrigenId == monedaOrigenId &&
                       tc.MonedaDestinoId == monedaDestinoId)
                // Ordena los resultados por FechaActualizacion de forma descendente.
                .OrderByDescending(tc => tc.FechaActualizacion)
                // Toma el primer resultado (el más reciente) o null si no hay coincidencias.
                .FirstOrDefaultAsync();
            // Ejecuta la consulta de forma asíncrona.
        }

        /// <summary>
        /// Obtiene un tipo de cambio específico buscando por los identificadores de las entidades Moneda de origen y destino.
        /// Este método es funcionalmente similar a GetExchangeRateAsync pero filtra usando las propiedades de navegación.
        /// </summary>
        /// <param name="monedaOrigen">El identificador de la moneda de origen (se espera que sea el ID de la entidad Moneda).</param>
        /// <param name="monedaDestino">El identificador de la moneda de destino (se espera que sea el ID de la entidad Moneda).</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es la entidad TipoCambio si se encuentra; de lo contrario, null.</returns>
        public async Task<TipoCambio?> GetByMonedasAsync(int monedaOrigen, int monedaDestino)
        {
            // Consulta el DbSet de TiposCambio en el contexto:
            return await _context.TiposCambio
                // Busca el primer tipo de cambio que coincida con los IDs de las entidades Moneda relacionadas.
                .FirstOrDefaultAsync(tc =>
                    tc.MonedaOrigen.Id == monedaOrigen && // Utiliza la propiedad de navegación MonedaOrigen
                    tc.MonedaDestino.Id == monedaDestino); // Utiliza la propiedad de navegación MonedaDestino
                                                           // Ejecuta la consulta de forma asíncrona y retorna el resultado o null.
        }
    }
}
