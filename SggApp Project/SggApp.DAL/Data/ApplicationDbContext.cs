using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SggApp.DAL.Entidades;

namespace SggApp.DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Moneda> Monedas { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Presupuesto> Presupuestos { get; set; }
        public DbSet<TipoCambio> TiposCambio { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de la tabla Monedas
            modelBuilder.Entity<Moneda>()
                .HasIndex(m => m.Codigo)
                .IsUnique();

            // Configuración de la tabla Usuarios
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.MonedaPredeterminada)
                .WithMany(m => m.UsuariosConMonedaPredeterminada)
                .HasForeignKey(u => u.MonedaPredeterminadaId);

            // Configuración de la tabla Categorias
            modelBuilder.Entity<Categoria>()
                .HasIndex(c => new { c.UsuarioId, c.Nombre })
                .IsUnique();

            modelBuilder.Entity<Categoria>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Categorias)
                .HasForeignKey(c => c.UsuarioId);

            // Configuración de la tabla Gastos
            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Usuario)
                .WithMany(u => u.Gastos)
                .HasForeignKey(g => g.UsuarioId);

            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Categoria)
                .WithMany(c => c.Gastos)
                .HasForeignKey(g => g.CategoriaId);

            modelBuilder.Entity<Gasto>()
                .HasOne(g => g.Moneda)
                .WithMany(m => m.Gastos)
                .HasForeignKey(g => g.MonedaId);

            modelBuilder.Entity<Gasto>()
                .HasIndex(g => g.UsuarioId);

            modelBuilder.Entity<Gasto>()
                .HasIndex(g => g.CategoriaId);

            modelBuilder.Entity<Gasto>()
                .HasIndex(g => g.Fecha);

            modelBuilder.Entity<Gasto>()
                .HasIndex(g => new { g.UsuarioId, g.Fecha });

            // Configuración de la tabla Presupuestos
            modelBuilder.Entity<Presupuesto>()
                .HasOne(p => p.Usuario)
                .WithMany(u => u.Presupuestos)
                .HasForeignKey(p => p.UsuarioId);

            modelBuilder.Entity<Presupuesto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Presupuestos)
                .HasForeignKey(p => p.CategoriaId);

            modelBuilder.Entity<Presupuesto>()
                .HasOne(p => p.Moneda)
                .WithMany(m => m.Presupuestos)
                .HasForeignKey(p => p.MonedaId);

            modelBuilder.Entity<Presupuesto>()
                .HasIndex(p => p.UsuarioId);

            modelBuilder.Entity<Presupuesto>()
                .HasIndex(p => p.CategoriaId);

            modelBuilder.Entity<Presupuesto>()
                .HasIndex(p => new { p.FechaInicio, p.FechaFin });

            modelBuilder.Entity<TipoCambio>()
                .HasOne(tc => tc.MonedaOrigen)
                .WithMany(m => m.TiposCambioOrigen)
                .HasForeignKey(tc => tc.MonedaOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoCambio>()
                .HasOne(tc => tc.MonedaDestino)
                .WithMany(m => m.TiposCambioDestino)
                .HasForeignKey(tc => tc.MonedaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TipoCambio>()
                .HasIndex(tc => new { tc.MonedaOrigenId, tc.MonedaDestinoId })
                .IsUnique();
        }
    }
}
