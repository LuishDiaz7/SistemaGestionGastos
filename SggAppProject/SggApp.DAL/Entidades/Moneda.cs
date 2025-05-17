using System; 
using System.Collections.Generic; 

namespace SggApp.DAL.Entidades
{
    /// <summary>
    /// Representa la entidad Moneda en la capa de acceso a datos (DAL).
    /// Almacena información sobre las diferentes monedas soportadas por la aplicación y sus relaciones con otras entidades.
    /// </summary>
    public class Moneda
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la moneda.
        /// Es la clave primaria de la entidad.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el código ISO 4217 de la moneda (ej. "USD", "COP").
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre completo de la moneda (ej. "Dólar estadounidense").
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece el símbolo de la moneda (ej. "$", "€").
        /// </summary>
        public string Simbolo { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la moneda está activa y disponible para su uso en la aplicación.
        /// Por defecto, una nueva moneda está activa.
        /// </summary>
        public bool Activa { get; set; } = true;

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de gastos registrados en esta moneda.
        /// Representa una relación de uno a muchos (una moneda puede estar asociada a muchos gastos).
        /// </summary>
        public ICollection<Gasto> Gastos { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de presupuestos definidos en esta moneda.
        /// Representa una relación de uno a muchos (una moneda puede ser la base para muchos presupuestos).
        /// </summary>
        public ICollection<Presupuesto> Presupuestos { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de usuarios que tienen esta moneda como su moneda predeterminada.
        /// Representa una relación de uno a muchos (una moneda puede ser la predeterminada para muchos usuarios).
        /// </summary>
        public ICollection<Usuario> UsuariosConMonedaPredeterminada { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de tipos de cambio donde esta moneda es la moneda de origen.
        /// Representa una relación de uno a muchos (una moneda puede ser el origen de muchos tipos de cambio).
        /// </summary>
        public ICollection<TipoCambio> TiposCambioOrigen { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de tipos de cambio donde esta moneda es la moneda de destino.
        /// Representa una relación de uno a muchos (una moneda puede ser el destino de muchos tipos de cambio).
        /// </summary>
        public ICollection<TipoCambio> TiposCambioDestino { get; set; }
    }
}
