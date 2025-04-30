using SggApp.DAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Interfaces
{
    public interface ITipoCambioService
    {
        Task<TipoCambio> ObtenerTipoCambioAsync(int monedaOrigen, int monedaDestino);
        Task<decimal> ObtenerTasaCambioAsync(int monedaOrigen, int monedaDestino);
        Task ActualizarTipoCambioAsync(int monedaOrigen, int monedaDestino, decimal tasa);
        Task<decimal> ConvertirMontoAsync(decimal monto, int monedaOrigen, int monedaDestino);
    }
}
