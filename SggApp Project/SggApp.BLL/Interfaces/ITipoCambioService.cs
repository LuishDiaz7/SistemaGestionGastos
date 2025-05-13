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
        Task<IEnumerable<TipoCambio>> ObtenerTodosAsync(); // Para la acción Index
        Task<TipoCambio> ObtenerPorIdAsync(int id); // Para Details, Edit, Delete
        Task AgregarAsync(TipoCambio tipoCambio); // Para la acción Create (POST)
        Task ActualizarAsync(TipoCambio tipoCambio); // Para la acción Edit (POST)
        Task EliminarAsync(int id); // Para la acción Delete (POST)
    }
}
