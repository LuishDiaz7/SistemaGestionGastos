using System; 
namespace SggApp.DAL.Entidades
{
    /// <summary>
    /// Representa la entidad TipoCambio en la capa de acceso a datos (DAL).
    /// Almacena la tasa de conversión entre dos monedas específicas.
    /// </summary>
    public class TipoCambio
    {
        /// <summary>
        /// Obtiene o establece el identificador único del tipo de cambio.
        /// Es la clave primaria de la entidad.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda de origen para esta tasa de cambio.
        /// Es una clave foránea que enlaza el tipo de cambio con la Moneda de origen.
        /// </summary>
        public int MonedaOrigenId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda de destino para esta tasa de cambio.
        /// Es una clave foránea que enlaza el tipo de cambio con la Moneda de destino.
        /// </summary>
        public int MonedaDestinoId { get; set; }

        /// <summary>
        /// Obtiene o establece la tasa de conversión (cuántas unidades de MonedaDestino equivalen a una unidad de MonedaOrigen).
        /// </summary>
        public decimal Tasa { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora en que se registró la última actualización de esta tasa de cambio.
        /// Se establece automáticamente al momento de la creación si no se especifica.
        /// </summary>
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        /// <summary>
        /// Propiedad de navegación para acceder a la entidad Moneda de origen asociada a este tipo de cambio.
        /// Representa una relación de muchos a uno (muchos tipos de cambio pueden tener la misma moneda de origen).
        /// </summary>
        public Moneda MonedaOrigen { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la entidad Moneda de destino asociada a este tipo de cambio.
        /// Representa una relación de muchos a uno (muchos tipos de cambio pueden tener la misma moneda de destino).
        /// </summary>
        public Moneda MonedaDestino { get; set; }
    }
}

