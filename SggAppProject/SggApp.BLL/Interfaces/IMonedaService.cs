using SggApp.DAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Interfaces
{
    public interface IMonedaService
    {
        Task<IEnumerable<Moneda>> ObtenerTodasAsync();
        Task<Moneda> ObtenerPorIdAsync(int id);
        Task<Moneda> ObtenerPorCodigoAsync(string codigo);
        Task<IEnumerable<Moneda>> ObtenerActivasAsync();
        Task AgregarAsync(Moneda moneda);
        Task ActualizarAsync(Moneda moneda);
        Task DesactivarAsync(int id);
        Task<bool> MonedaTieneRegistrosAsociadosAsync(int monedaId);
        Task<int> ContarRegistrosAsociadosAsync(int monedaId);
        Task<bool> EliminarAsync(int id);
    }
}
