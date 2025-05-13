using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.DAL.Data; 
using SggApp.DAL.Entidades;
using SggApp.DAL.Repositorios; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Servicios
{
    public class MonedaService : IMonedaService 
    {
        private readonly MonedaRepository _monedaRepository;
        private readonly ApplicationDbContext _context; 

        public MonedaService(MonedaRepository monedaRepository, ApplicationDbContext context) 
        {
            _monedaRepository = monedaRepository;
            _context = context; 
        }

        public async Task<IEnumerable<Moneda>> ObtenerTodasAsync()
        {
            return await _monedaRepository.GetAllAsync();
        }

        public async Task<Moneda?> ObtenerPorIdAsync(int id)
        {
            return await _monedaRepository.GetByIdAsync(id);
        }

        public async Task<Moneda?> ObtenerPorCodigoAsync(string codigo)
        {
            var monedas = await _monedaRepository.GetByConditionAsync(m => m.Codigo == codigo);
            return monedas.FirstOrDefault();
        }

        public async Task<IEnumerable<Moneda>> ObtenerActivasAsync()
        {
            return await _monedaRepository.GetByConditionAsync(m => m.Activa);
        }

        public async Task AgregarAsync(Moneda moneda)
        {
            // Validar que el código no exista
            var monedas = await _monedaRepository.GetByConditionAsync(m => m.Codigo == moneda.Codigo);
            if (monedas.Any())
                throw new ArgumentException("Ya existe una moneda con este código.");

            await _monedaRepository.AddAsync(moneda);
        }

        public async Task ActualizarAsync(Moneda moneda)
        {
            // Validar que el código no exista en otra moneda
            var monedas = await _monedaRepository.GetByConditionAsync(m =>
                m.Codigo == moneda.Codigo &&
                m.Id != moneda.Id);

            if (monedas.Any())
                throw new ArgumentException("Ya existe otra moneda con este código.");

            _monedaRepository.Update(moneda);
        }

        public async Task DesactivarAsync(int id)
        {
            var moneda = await _monedaRepository.GetByIdAsync(id);
            if (moneda != null)
            {
                moneda.Activa = false;
                _monedaRepository.Update(moneda);

            }
        }

        public async Task<bool> EliminarAsync(int id)
        {
            if (await MonedaTieneRegistrosAsociadosAsync(id))
            {
                return false;
            }

            var monedaToDelete = await _monedaRepository.GetByIdAsync(id);

            if (monedaToDelete == null)
            {
                return false;
            }

            _monedaRepository.Delete(monedaToDelete);

            await _context.SaveChangesAsync(); 

            return true; 
        }


        public async Task<bool> MonedaTieneRegistrosAsociadosAsync(int monedaId)
        {
            return await _context.Gastos.AnyAsync(g => g.MonedaId == monedaId) ||
                   await _context.Presupuestos.AnyAsync(p => p.MonedaId == monedaId) ||
                   await _context.Users.AnyAsync(u => u.MonedaPredeterminadaId == monedaId);
        }

        public async Task<int> ContarRegistrosAsociadosAsync(int monedaId)
        {
            var gastos = await _context.Gastos.CountAsync(g => g.MonedaId == monedaId);
            var presupuestos = await _context.Presupuestos.CountAsync(p => p.MonedaId == monedaId);
            var usuarios = await _context.Users.CountAsync(u => u.MonedaPredeterminadaId == monedaId);

            return gastos + presupuestos + usuarios;
        }
    }
}
