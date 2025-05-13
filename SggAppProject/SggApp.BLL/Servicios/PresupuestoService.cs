using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.DAL;
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
    public class PresupuestoService : IPresupuestoService
    {
        private readonly PresupuestoRepository _presupuestoRepository;
        private readonly GastoRepository _gastoRepository;
        private readonly ITipoCambioService _tipoCambioService;
        private readonly ApplicationDbContext _context;

        public PresupuestoService(
            PresupuestoRepository presupuestoRepository,
            GastoRepository gastoRepository,
            ITipoCambioService tipoCambioService)
        {
            _presupuestoRepository = presupuestoRepository;
            _gastoRepository = gastoRepository;
            _tipoCambioService = tipoCambioService;
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerTodosAsync()
        {
            return await _presupuestoRepository.GetAllAsync();
        }

        public async Task<Presupuesto> ObtenerPorIdAsync(int id)
        {
            return await _presupuestoRepository.GetByIdAsync(id);

        }

        public async Task<IEnumerable<Presupuesto>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            return await _presupuestoRepository.GetByConditionAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerPresupuestosVigentesAsync(int usuarioId)
        {
            DateTime hoy = DateTime.Today;
            return await _presupuestoRepository.GetByConditionAsync(p =>
                p.UsuarioId == usuarioId &&
                p.FechaInicio <= hoy &&
                p.FechaFin >= hoy);
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerPresupuestosPorCategoriaAsync(int categoriaId)
        {
            return await _presupuestoRepository.GetByConditionAsync(p => p.CategoriaId == categoriaId);
        }

        public async Task<decimal> ObtenerGastoActualAsync(int presupuestoId)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(presupuestoId);
            if (presupuesto == null)
                throw new Exception($"No existe presupuesto con ID {presupuestoId}");

            IEnumerable<Gasto> gastos;

            if (presupuesto.CategoriaId.HasValue)
            {
                // Si el presupuesto es para una categoría específica
                gastos = await _gastoRepository.GetByConditionAsync(g =>
                    g.UsuarioId == presupuesto.UsuarioId &&
                    g.CategoriaId == presupuesto.CategoriaId &&
                    g.Fecha >= presupuesto.FechaInicio &&
                    g.Fecha <= presupuesto.FechaFin);
            }
            else
            {
                // Si el presupuesto es general (todas las categorías)
                gastos = await _gastoRepository.GetByConditionAsync(g =>
                    g.UsuarioId == presupuesto.UsuarioId &&
                    g.Fecha >= presupuesto.FechaInicio &&
                    g.Fecha <= presupuesto.FechaFin);
            }

            decimal total = 0;
            foreach (var gasto in gastos)
            {
                if (gasto.MonedaId == presupuesto.MonedaId)
                {
                    total += gasto.Monto;
                }
                else
                {
                    try
                    {
                        var tasa = await _tipoCambioService.ObtenerTasaCambioAsync(gasto.MonedaId, presupuesto.MonedaId);
                        total += gasto.Monto * tasa;
                    }
                    catch (Exception)
                    {
                        // Si no hay tasa de cambio disponible, no incluir este gasto en el total
                    }
                }
            }

            return total;
        }

        public async Task<decimal> ObtenerPorcentajeConsumidoAsync(int presupuestoId)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(presupuestoId);
            if (presupuesto == null)
                throw new Exception($"No existe presupuesto con ID {presupuestoId}");

            decimal gastoActual = await ObtenerGastoActualAsync(presupuestoId);

            // Calcular el porcentaje consumido
            if (presupuesto.Limite <= 0)
                return 0;

            return Math.Round((gastoActual / presupuesto.Limite) * 100, 2);
        }

        public async Task<bool> VerificarAlertaPresupuestoAsync(int presupuestoId)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(presupuestoId);
            if (presupuesto == null || !presupuesto.NotificarAl.HasValue)
                return false;

            decimal porcentajeConsumido = await ObtenerPorcentajeConsumidoAsync(presupuestoId);

            // Verificar si el porcentaje consumido supera el nivel de alerta configurado
            return porcentajeConsumido >= presupuesto.NotificarAl.Value;
        }

        public async Task AgregarAsync(Presupuesto presupuesto)
        {
            presupuesto.FechaCreacion = DateTime.Now;
            await _presupuestoRepository.AddAsync(presupuesto);
        }

        public async Task ActualizarAsync(Presupuesto presupuesto)
        {
            _presupuestoRepository.Update(presupuesto);
        }

        public async Task EliminarAsync(int id)
        {
            var presupuesto = await _presupuestoRepository.GetByIdAsync(id);
            if (presupuesto != null)
            {
                _presupuestoRepository.Delete(presupuesto);
            }
        }

        public async Task<IEnumerable<Presupuesto>> ObtenerActivosPorUsuarioAsync(int userId)
        {
            return await _presupuestoRepository.GetByConditionAsync(p => p.UsuarioId == userId && p.Activo);
        }
    }
}
