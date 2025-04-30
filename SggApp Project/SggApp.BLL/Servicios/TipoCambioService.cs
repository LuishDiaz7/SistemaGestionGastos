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
    public class TipoCambioService : ITipoCambioService
    {
        private readonly TipoCambioRepository _tipoCambioRepository;

        public TipoCambioService(TipoCambioRepository tipoCambioRepository)
        {
            _tipoCambioRepository = tipoCambioRepository;
        }

        public async Task<TipoCambio?> ObtenerTipoCambioAsync(int monedaOrigen, int monedaDestino)
        {
            return await _tipoCambioRepository.GetByMonedasAsync(monedaOrigen, monedaDestino);
        }

        public async Task<decimal> ObtenerTasaCambioAsync(int monedaOrigen, int monedaDestino)
        {
            var tipoCambio = await ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);
            return tipoCambio?.Tasa ?? throw new KeyNotFoundException("No se encontró el tipo de cambio especificado.");
        }

        public async Task ActualizarTipoCambioAsync(int monedaOrigen, int monedaDestino, decimal tasa)
        {
            var tipoCambio = await ObtenerTipoCambioAsync(monedaOrigen, monedaDestino);

            if (tipoCambio == null)
            {
                tipoCambio = new TipoCambio
                {
                    MonedaOrigenId = monedaOrigen,
                    MonedaDestinoId = monedaDestino,
                    Tasa = tasa
                };
                await _tipoCambioRepository.AddAsync(tipoCambio);
            }
            else
            {
                tipoCambio.Tasa = tasa;
                _tipoCambioRepository.Update(tipoCambio);
            }
        }

        public async Task<decimal> ConvertirMontoAsync(decimal monto, int monedaOrigen, int monedaDestino)
        {
            if (monedaOrigen == monedaDestino)
                return monto;

            var tasa = await ObtenerTasaCambioAsync(monedaOrigen, monedaDestino);
            return monto * tasa;
        }
    }
}
