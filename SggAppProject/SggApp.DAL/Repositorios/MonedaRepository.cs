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
    public class MonedaRepository : GenericRepository<Moneda>
    {

        public MonedaRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        // Método específico: Obtener monedas activas
        public async Task<IEnumerable<Moneda>> GetActiveCurrenciesAsync()
        {
            return await _context.Monedas
                .Where(m => m.Activa)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        // Método específico: Obtener moneda por código ISO
        public async Task<Moneda?> GetByCodigo(string codigo)
        {
            return await _context.Monedas
                .FirstOrDefaultAsync(m => m.Codigo.ToUpper() == codigo.ToUpper());
        }
    }
}