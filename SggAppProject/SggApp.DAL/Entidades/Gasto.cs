using System; 

namespace SggApp.DAL.Entidades
{
    /// <summary>
    /// Representa la entidad Gasto en la capa de acceso a datos (DAL).
    /// Almacena información detallada sobre un gasto incurrido por un usuario.
    /// </summary>
    public class Gasto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del gasto.
        /// Es la clave primaria de la entidad.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del usuario que registró el gasto.
        /// Es una clave foránea que enlaza el gasto con su propietario Usuario.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la categoría a la que pertenece el gasto.
        /// Es una clave foránea que enlaza el gasto con una Categoria.
        /// </summary>
        public int CategoriaId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda en la que se registró el gasto.
        /// Es una clave foránea que enlaza el gasto con una Moneda.
        /// </summary>
        public int MonedaId { get; set; }

        /// <summary>
        /// Obtiene o establece el monto del gasto.
        /// </summary>
        public decimal Monto { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha en que se realizó el gasto.
        /// </summary>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Obtiene o establece una descripción opcional del gasto.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece el lugar opcional donde se realizó el gasto.
        /// </summary>
        public string Lugar { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si el gasto es recurrente.
        /// Por defecto, un nuevo gasto no es recurrente.
        /// </summary>
        public bool EsRecurrente { get; set; } = false;

        /// <summary>
        /// Obtiene o establece la fecha y hora en que el gasto fue registrado en el sistema.
        /// Se establece automáticamente al momento de la creación.
        /// </summary>
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        /// <summary>
        /// Propiedad de navegación para acceder al usuario que registró este gasto.
        /// Representa una relación de muchos a uno (muchos gastos pertenecen a un usuario).
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la categoría asociada a este gasto.
        /// Representa una relación de muchos a uno (muchos gastos pertenecen a una categoría).
        /// </summary>
        public Categoria Categoria { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la moneda asociada a este gasto.
        /// Representa una relación de muchos a uno (muchos gastos están en una moneda).
        /// </summary>
        public Moneda Moneda { get; set; }
    }
}
