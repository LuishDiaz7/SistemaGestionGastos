using Microsoft.AspNetCore.Identity; 
using System; 
using System.Collections.Generic; 


namespace SggApp.DAL.Entidades
{
    /// <summary>
    /// Representa la entidad de usuario en la capa de acceso a datos (DAL), extendiendo la funcionalidad estándar de IdentityUser.
    /// Almacena información adicional sobre un usuario, además de los datos gestionados por ASP.NET Core Identity.
    /// </summary>
    public class Usuario : IdentityUser<int> // Hereda las propiedades estándar de usuario de Identity, usando un entero para la clave primaria.
    {
        /// <summary>
        /// Obtiene o establece el nombre del usuario.
        /// </summary>
        public string Name { get; set; }

        // Propiedades como Email, PasswordHash, etc., son heredadas de IdentityUser<int>.
        // public string Email { get; set; } // Esta propiedad ya existe en IdentityUser.
        // public byte[] PasswordHash { get; set; } // Esta propiedad ya existe en IdentityUser.

        /// <summary>
        /// Obtiene o establece la fecha y hora en que el usuario fue registrado en el sistema.
        /// Se establece automáticamente al momento de la creación si no se especifica.
        /// </summary>
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        /// <summary>
        /// Obtiene o establece el identificador de la moneda predeterminada seleccionada por el usuario.
        /// Es una clave foránea opcional (permite valor nulo) que enlaza el usuario con una Moneda.
        /// </summary>
        public int? MonedaPredeterminadaId { get; set; }

        /// <summary>
        /// Obtiene o establece un valor que indica si la cuenta del usuario está activa en la aplicación.
        /// Por defecto, una nueva cuenta está activa.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Propiedad de navegación para acceder a la entidad Moneda predeterminada asociada a este usuario (si MonedaPredeterminadaId no es nulo).
        /// Representa una relación de muchos a uno (muchos usuarios pueden tener la misma moneda predeterminada).
        /// </summary>
        public Moneda MonedaPredeterminada { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de categorías creadas por este usuario.
        /// Representa una relación de uno a muchos (un usuario puede tener muchas categorías).
        /// </summary>
        public ICollection<Categoria> Categorias { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de gastos registrados por este usuario.
        /// Representa una relación de uno a muchos (un usuario puede registrar muchos gastos).
        /// </summary>
        public ICollection<Gasto> Gastos { get; set; }

        /// <summary>
        /// Propiedad de navegación para acceder a la colección de presupuestos creados por este usuario.
        /// Representa una relación de uno a muchos (un usuario puede crear muchos presupuestos).
        /// </summary>
        public ICollection<Presupuesto> Presupuestos { get; set; }
    }
}
