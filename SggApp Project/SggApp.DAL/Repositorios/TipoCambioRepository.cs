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
    public class TipoCambioRepository : GenericRepository<TipoCambio>
    {

        public TipoCambioRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        // Método específico: Obtener tipo de cambio entre dos monedas
        public async Task<TipoCambio?> GetExchangeRateAsync(int monedaOrigenId, int monedaDestinoId)
        {
            return await _context.TiposCambio
                .FirstOrDefaultAsync(tc => tc.MonedaOrigenId == monedaOrigenId &&
                                     tc.MonedaDestinoId == monedaDestinoId);
        }

        // Método específico: Obtener el último tipo de cambio actualizado
        public async Task<TipoCambio?> GetLatestExchangeRateAsync(int monedaOrigenId, int monedaDestinoId)
        {
            return await _context.TiposCambio
                .Where(tc => tc.MonedaOrigenId == monedaOrigenId &&
                       tc.MonedaDestinoId == monedaDestinoId)
                .OrderByDescending(tc => tc.FechaActualizacion)
                .FirstOrDefaultAsync();
        }

        public async Task<TipoCambio?> GetByMonedasAsync(int monedaOrigen, int monedaDestino)
        {
            return await _context.TiposCambio
                .FirstOrDefaultAsync(tc =>
                    tc.MonedaOrigen.Id == monedaOrigen &&
                    tc.MonedaDestino.Id == monedaDestino);
        }
    }
}
