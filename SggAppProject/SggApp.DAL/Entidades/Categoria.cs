using System; 
using System.Collections.Generic; 

namespace SggApp.DAL.Entidades
{
    /// <summary>
    /// Representa la entidad Categoria en la capa de acceso a datos (DAL).
    /// Almacena información sobre las categorías que un usuario puede definir para organizar gastos y presupuestos.
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Obtiene o establece el identificador único de la categoría.
        /// Es la clave primaria de la entidad.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Obtiene o establece el identificador del usuario al que pertenece esta categoría.
        /// Es una clave foránea que enlaza la categoría con su propietario Usuario.
        /// </summary>
        public int UsuarioId { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre de la categoría.
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Obtiene o establece la descripción opcional de la categoría.
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la categoría está activa.
        /// Por defecto, una nueva categoría está activa.
        /// </summary>
        public bool Activa { get; set; } = true;

        /// <summary>
        /// Propiedad de navegación para acceder al usuario propietario de esta categoría.
        /// Representa una relación de muchos a uno (muchas categorías pertenecen a un usuario).
        /// </summary>
        public Usuario Usuario { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de gastos asociados a esta categoría.
        /// Representa una relación de uno a muchos (una categoría puede tener muchos gastos).
        /// </summary>
        public ICollection<Gasto> Gastos { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de presupuestos asociados a esta categoría (presupuestos específicos por categoría).
        /// Representa una relación de uno a muchos (una categoría puede tener muchos presupuestos asociados directamente a ella).
        /// </summary>
        public ICollection<Presupuesto> Presupuestos { get; set; }
    }
}