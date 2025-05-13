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
    public class GastoRepository : GenericRepository<Gasto>
    {

        public GastoRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task<IEnumerable<Gasto>> GetByUsuarioYRangoFechaAsync(int userId, DateTime startDate, DateTime endDate)
        {
            // Aquí es donde se realiza la consulta a la base de datos
            // Filtra por UsuarioId y por el rango de fechas (inclusive)
            return await _context.Gastos
                .Where(g => g.UsuarioId == userId && g.Fecha >= startDate && g.Fecha <= endDate)
                .ToListAsync(); // Convierte los resultados a una lista asíncronamente
        }

        // Método específico: Obtener gastos de un usuario
        public async Task<IEnumerable<Gasto>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId)
                .ToListAsync();
        }

        // Método específico: Obtener gastos por rango de fechas
        public async Task<IEnumerable<Gasto>> GetByDateRangeAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId && g.Fecha >= fechaInicio && g.Fecha <= fechaFin)
                .OrderByDescending(g => g.Fecha)
                .ToListAsync();
        }

        // Método específico: Obtener gastos con todos los detalles relacionados
        public async Task<IEnumerable<Gasto>> GetWithDetailsAsync(int usuarioId)
        {
            return await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId)
                .Include(g => g.Categoria)
                .Include(g => g.Moneda)
                .OrderByDescending(g => g.Fecha)
                .ToListAsync();
        }

        // Método específico: Obtener el total de gastos por categoría
        public async Task<Dictionary<string, decimal>> GetTotalByCategoriaAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            var result = await _context.Gastos
                .Where(g => g.UsuarioId == usuarioId && g.Fecha >= fechaInicio && g.Fecha <= fechaFin)
                .Include(g => g.Categoria)
                .GroupBy(g => g.Categoria.Nombre)
                .Select(g => new { Categoria = g.Key, Total = g.Sum(x => x.Monto) })
                .ToDictionaryAsync(k => k.Categoria, v => v.Total);

            return result;
        }
    }
}
