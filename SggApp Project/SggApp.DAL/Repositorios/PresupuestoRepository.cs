using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SggApp.DAL.Data;
using SggApp.DAL.Entidades;

namespace SggApp.DAL.Repositorios
{
    public class PresupuestoRepository : GenericRepository<Presupuesto>
    {

        public PresupuestoRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        // Método específico: Obtener presupuestos activos de un usuario
        public async Task<IEnumerable<Presupuesto>> GetActiveByUsuarioIdAsync(int usuarioId)
        {
            var today = DateTime.Today;
            return await _context.Presupuestos
                .Where(p => p.UsuarioId == usuarioId && p.FechaFin >= today)
                .Include(p => p.Categoria)
                .Include(p => p.Moneda)
                .OrderBy(p => p.FechaFin)
                .ToListAsync();
        }

        // Método específico: Obtener presupuesto con detalles
        public async Task<Presupuesto?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Presupuestos
                .Include(p => p.Categoria)
                .Include(p => p.Moneda)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Método específico: Verificar si existe un presupuesto para una categoría en un período
        public async Task<bool> ExistsForCategoryInPeriodAsync(int usuarioId, int categoriaId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Presupuestos
                .AnyAsync(p => p.UsuarioId == usuarioId &&
                          p.CategoriaId == categoriaId &&
                          ((p.FechaInicio <= fechaInicio && p.FechaFin >= fechaInicio) ||
                           (p.FechaInicio <= fechaFin && p.FechaFin >= fechaFin) ||
                           (p.FechaInicio >= fechaInicio && p.FechaFin <= fechaFin)));
        }
    }
}
