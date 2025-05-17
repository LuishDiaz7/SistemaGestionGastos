using System; 
using Microsoft.EntityFrameworkCore; 
using SggApp.DAL.Entidades;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace SggApp.DAL.Data
{
    /// <summary>
    /// Representa el contexto de base de datos principal de la aplicación utilizando Entity Framework Core.
    /// Gestiona la interacción con la base de datos e integra las entidades de la aplicación con el sistema de identidad de ASP.NET Core.
    /// Hereda de IdentityDbContext para incluir las tablas de identidad estándar (usuarios, roles, etc.), personalizando con la entidad Usuario y la clave primaria int.
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
    {
        /// <summary>
        /// Inicializa una nueva instancia del contexto de base de datos de la aplicación.
        /// </summary>
        /// <param name="options">Las opciones utilizadas por el DbContext.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        /// <summary>
        /// Obtiene o establece el DbSet para la entidad Categoria.
        /// Permite consultar y guardar instancias de Categoria en la base de datos.
        /// </summary>
        public DbSet<Categoria> Categorias { get; set; }

        /// <summary>
        /// Obtiene o establece el DbSet para la entidad Moneda.
        /// Permite consultar y guardar instancias de Moneda en la base de datos.
        /// </summary>
        public DbSet<Moneda> Monedas { get; set; }

        /// <summary>
        /// Obtiene o establece el DbSet para la entidad Gasto.
        /// Permite consultar y guardar instancias de Gasto en la base de datos.
        /// </summary>
        public DbSet<Gasto> Gastos { get; set; }

        /// <summary>
        /// Obtiene o establece el DbSet para la entidad Presupuesto.
        /// Permite consultar y guardar instancias de Presupuesto en la base de datos.
        /// </summary>
        public DbSet<Presupuesto> Presupuestos { get; set; }

        /// <summary>
        /// Obtiene o establece el DbSet para la entidad TipoCambio.
        /// Permite consultar y guardar instancias de TipoCambio en la base de datos.
        /// </summary>
        public DbSet<TipoCambio> TiposCambio { get; set; }

        /// <summary>
        /// Configura el modelo de la base de datos utilizando la API fluida.
        /// Define la estructura de la tabla, las relaciones, las claves foráneas, el comportamiento de eliminación en cascada, los índices y las restricciones de unicidad.
        /// </summary>
        /// <param name="modelBuilder">El constructor de modelos utilizado para configurar el esquema de la base de datos.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Llama a la implementación base para configurar el modelo Identity. Es crucial no omitir esta llamada.
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Moneda.
            modelBuilder.Entity<Moneda>()
                // Define un índice único en la columna Codigo para asegurar que no haya códigos de moneda duplicados.
                .HasIndex(m => m.Codigo)
                .IsUnique();

            // Configuración de la entidad Usuario.
            // Las tablas de usuario estándar (Users, Roles, etc.) se configuran en base.OnModelCreating.
            modelBuilder.Entity<Usuario>()
                // Define un índice único en la columna Email para asegurar que no haya emails de usuario duplicados.
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                // Configura la relación muchos a uno con Moneda para la moneda predeterminada.
                .HasOne(u => u.MonedaPredeterminada)
                // Configura la relación inversa de uno a muchos en la entidad Moneda.
                .WithMany(m => m.UsuariosConMonedaPredeterminada)
                // Especifica la clave foránea.
                .HasForeignKey(u => u.MonedaPredeterminadaId)
                // Configura que la eliminación de una Moneda NO elimine Usuarios asociados (comportamiento restringido).
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la entidad Categoria.
            modelBuilder.Entity<Categoria>()
                // Define un índice compuesto único en UsuarioId y Nombre para asegurar que un usuario no tenga dos categorías con el mismo nombre.
                .HasIndex(c => new { c.UsuarioId, c.Nombre })
                .IsUnique();

            modelBuilder.Entity<Categoria>()
                // Configura la relación muchos a uno con Usuario.
                .HasOne(c => c.Usuario)
                // Configura la relación inversa de uno a muchos en la entidad Usuario.
                .WithMany(u => u.Categorias)
                // Especifica la clave foránea.
                .HasForeignKey(c => c.UsuarioId)
                // Configura que la eliminación de un Usuario NO elimine Categorías asociadas (comportamiento restringido).
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la entidad Gasto.
            modelBuilder.Entity<Gasto>()
                // Configura la relación muchos a uno con Usuario.
                .HasOne(g => g.Usuario)
                // Configura la relación inversa de uno a muchos en la entidad Usuario.
                .WithMany(u => u.Gastos)
                // Especifica la clave foránea.
                .HasForeignKey(g => g.UsuarioId)
                // Configura que la eliminación de un Usuario NO elimine Gastos asociados (comportamiento restringido).
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gasto>()
                // Configura la relación muchos a uno con Categoria.
                .HasOne(g => g.Categoria)
                // Configura la relación inversa de uno a muchos en la entidad Categoria.
                .WithMany(c => c.Gastos)
                // Especifica la clave foránea.
                .HasForeignKey(g => g.CategoriaId)
                // Configura que la eliminación de una Categoria SÍ elimine los Gastos asociados (comportamiento en cascada).
                .OnDelete(DeleteBehavior.Cascade); // Comportamiento de eliminación en cascada.

            modelBuilder.Entity<Gasto>()
                // Configura la relación muchos a uno con Moneda.
                .HasOne(g => g.Moneda)
                // Configura la relación inversa de uno a muchos en la entidad Moneda.
                .WithMany(m => m.Gastos)
                // Especifica la clave foránea.
                .HasForeignKey(g => g.MonedaId)
                // Configura que la eliminación de una Moneda NO elimine Gastos asociados (comportamiento restringido).
                .OnDelete(DeleteBehavior.Restrict);

            // Define índices no únicos en columnas de uso frecuente para mejorar el rendimiento de las consultas.
            modelBuilder.Entity<Gasto>()
                .HasIndex(g => g.UsuarioId);
            modelBuilder.Entity<Gasto>()
                .HasIndex(g => g.CategoriaId);
            modelBuilder.Entity<Gasto>()
                .HasIndex(g => g.Fecha);
            modelBuilder.Entity<Gasto>()
                // Define un índice compuesto para consultas que filtran por usuario y fecha.
                .HasIndex(g => new { g.UsuarioId, g.Fecha });


            // Configuración de la entidad Presupuesto.
            modelBuilder.Entity<Presupuesto>()
                // Configura la relación muchos a uno con Usuario.
                .HasOne(p => p.Usuario)
                // Configura la relación inversa de uno a muchos en la entidad Usuario.
                .WithMany(u => u.Presupuestos)
                // Especifica la clave foránea.
                .HasForeignKey(p => p.UsuarioId)
                // Configura que la eliminación de un Usuario NO elimine Presupuestos asociados (comportamiento restringido).
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Presupuesto>()
                // Configura la relación muchos a uno con Categoria (clave foránea es nullable).
                .HasOne(p => p.Categoria)
                // Configura la relación inversa de uno a muchos en la entidad Categoria.
                .WithMany(c => c.Presupuestos)
                // Especifica la clave foránea (nullable).
                .HasForeignKey(p => p.CategoriaId);
            // Nota: El comportamiento de eliminación en cascada por defecto para FKs nullable es ClientSetNull o NoAction/Restrict dependiendo de la configuración global/DB.

            modelBuilder.Entity<Presupuesto>()
                // Configura la relación muchos a uno con Moneda.
                .HasOne(p => p.Moneda)
                // Configura la relación inversa de uno a muchos en la entidad Moneda.
                .WithMany(m => m.Presupuestos)
                // Especifica la clave foránea.
                .HasForeignKey(p => p.MonedaId);
            // Nota: El comportamiento de eliminación en cascada por defecto para FKs no nullable es Cascade por convención, pero aquí no se especifica explícitamente.

            // Define índices no únicos en columnas de uso frecuente.
            modelBuilder.Entity<Presupuesto>()
                 .HasIndex(p => p.UsuarioId);
            modelBuilder.Entity<Presupuesto>()
                 .HasIndex(p => p.CategoriaId); // Índice en FK nullable.
            modelBuilder.Entity<Presupuesto>()
                 // Define un índice compuesto para consultas que filtran por rango de fechas del presupuesto.
                 .HasIndex(p => new { p.FechaInicio, p.FechaFin });


            // Configuración de la entidad TipoCambio.
            modelBuilder.Entity<TipoCambio>()
                // Configura la relación muchos a uno con Moneda para la moneda de origen.
                .HasOne(tc => tc.MonedaOrigen)
                // Configura la relación inversa de uno a muchos en la entidad Moneda.
                .WithMany(m => m.TiposCambioOrigen)
                // Especifica la clave foránea.
                .HasForeignKey(tc => tc.MonedaOrigenId)
                // Configura que la eliminación de una Moneda NO elimine TiposCambio asociados como origen.
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoCambio>()
                // Configura la relación muchos a uno con Moneda para la moneda de destino.
                .HasOne(tc => tc.MonedaDestino)
                // Configura la relación inversa de uno a muchos en la entidad Moneda.
                .WithMany(m => m.TiposCambioDestino)
                // Especifica la clave foránea.
                .HasForeignKey(tc => tc.MonedaDestinoId)
                // Configura que la eliminación de una Moneda NO elimine TiposCambio asociados como destino.
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoCambio>()
                // Define un índice compuesto único en MonedaOrigenId y MonedaDestinoId para asegurar que solo haya una tasa por par de monedas.
                .HasIndex(tc => new { tc.MonedaOrigenId, tc.MonedaDestinoId })
                .IsUnique();
        }
    }
}
