using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.DAL.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Servicios
{
    public class GastoService : IGastoService
    {
        private readonly GastoRepository _gastoRepository;
        private readonly ITipoCambioService _tipoCambioService;

        public GastoService(
            GastoRepository gastoRepository,
            ITipoCambioService tipoCambioService)
        {
            _gastoRepository = gastoRepository;
            _tipoCambioService = tipoCambioService;
        }

        public async Task<IEnumerable<Gasto>> ObtenerTodosAsync()
        {
            return await _gastoRepository.GetAllAsync();
        }

        public async Task<Gasto> ObtenerPorIdAsync(int id)
        {
            return await _gastoRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Gasto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _gastoRepository.GetByConditionAsync(g => g.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Gasto>> ObtenerPorUsuarioYFechasAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _gastoRepository.GetByConditionAsync(g =>
                g.UsuarioId == usuarioId &&
                g.Fecha >= fechaInicio &&
                g.Fecha <= fechaFin);
        }

        public async Task<IEnumerable<Gasto>> ObtenerPorCategoriaAsync(int categoriaId)
        {
            return await _gastoRepository.GetByConditionAsync(g => g.CategoriaId == categoriaId);
        }

        public async Task<decimal> ObtenerTotalGastosPorUsuarioAsync(int usuarioId, int monedaDestinoId, DateTime fechaInicio, DateTime fechaFin)
        {
            var gastos = await ObtenerPorUsuarioYFechasAsync(usuarioId, fechaInicio, fechaFin);
            decimal total = 0;

            foreach (var gasto in gastos)
            {
                if (gasto.MonedaId == monedaDestinoId)
                {
                    // Si la moneda del gasto coincide con la moneda destino, sumar directamente
                    total += gasto.Monto;
                }
                else
                {
                    try
                    {
                        // Convertir el monto utilizando el servicio de tipos de cambio
                        var tasa = await _tipoCambioService.ObtenerTasaCambioAsync(gasto.MonedaId, monedaDestinoId);
                        total += gasto.Monto * tasa;
                    }
                    catch (Exception)
                    {
                        // Si no hay tasa de cambio disponible, no incluir este gasto en el total
                        // En un caso real, podrías querer registrar este error o manejarlo de otra forma
                    }
                }
            }

            return total;
        }

        public async Task AgregarAsync(Gasto gasto)
        {
            gasto.FechaRegistro = DateTime.Now;
            await _gastoRepository.AddAsync(gasto);
        }

        public async Task ActualizarAsync(Gasto gasto)
        {
            _gastoRepository.Update(gasto);
        }

        public async Task EliminarAsync(int id)
        {
            var gasto = await _gastoRepository.GetByIdAsync(id);
            if (gasto != null)
            {
                _gastoRepository.Delete(gasto);
            }
        }
    }
}
