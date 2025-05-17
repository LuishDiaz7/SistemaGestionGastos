using System; 

namespace SggApp.DAL.Entidades
{
    /// <summary>
    /// Representa la entidad Presupuesto en la capa de acceso a datos (DAL).
    /// Almacena información sobre un presupuesto definido por un usuario para un período y, opcionalmente, para una categoría específica.
    /// </summary>
    public class Presupuesto
    {
        /// <summary>
        /// Obtiene o establece el identificador único del presupuesto.
        /// Es la clave primaria de la entidad.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del usuario que creó el presupuesto.
        /// Es una clave foránea que enlaza el presupuesto con su propietario Usuario.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la categoría asociada al presupuesto.
        /// Es una clave foránea opcional (permite valor nulo) que enlaza el presupuesto con una Categoria.
        /// Si es nulo, el presupuesto es general para el usuario.
        /// </summary>
        public int? CategoriaId { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador de la moneda en la que se definió el límite del presupuesto.
        /// Es una clave foránea que enlaza el presupuesto con una Moneda.
        /// </summary>
        public int MonedaId { get; set; }

        /// <summary>
        /// Obtiene o establece el monto límite definido para el presupuesto.
        /// </summary>
        public decimal Limite { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de inicio del período que cubre el presupuesto.
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha de fin del período que cubre el presupuesto.
        /// </summary>
        public DateTime FechaFin { get; set; }

        /// <summary>
        /// Obtiene o establece un porcentaje opcional para notificar al usuario cuando el gasto alcance este umbral.
        /// Permite valor nulo si no se desea notificación.
        /// </summary>
        public int? NotificarAl { get; set; }

        /// <summary>
        /// Obtiene o establece la fecha y hora en que el presupuesto fue creado en el sistema.
        /// Se establece automáticamente al momento de la creación.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        /// <summary>
        /// Obtiene o establece un valor que indica si el presupuesto está actualmente activo.
        /// (Nota: La lógica para establecer/usar esta propiedad podría estar en el servicio o controlador).
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder al usuario propietario de este presupuesto.
        /// Representa una relación de muchos a uno (muchos presupuestos pertenecen a un usuario).
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la categoría asociada a este presupuesto (si CategoriaId no es nulo).
        /// Representa una relación de muchos a uno (muchos presupuestos pueden estar asociados a una categoría, o ninguno).
        /// </summary>
        public Categoria Categoria { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la moneda asociada a este presupuesto.
        /// Representa una relación de muchos a uno (muchos presupuestos se definen en una moneda).
        /// </summary>
        public Moneda Moneda { get; set; }
    }
}
