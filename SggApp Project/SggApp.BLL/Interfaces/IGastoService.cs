using SggApp.DAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Interfaces
{
    public interface IGastoService
    {
        Task<IEnumerable<Gasto>> ObtenerTodosAsync();
        Task<Gasto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Gasto>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Gasto>> ObtenerPorUsuarioYFechasAsync(int usuarioId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Gasto>> ObtenerPorCategoriaAsync(int categoriaId);
        Task<decimal> ObtenerTotalGastosPorUsuarioAsync(int usuarioId, int monedaId, DateTime fechaInicio, DateTime fechaFin);
        Task AgregarAsync(Gasto gasto);
        Task ActualizarAsync(Gasto gasto);
        Task EliminarAsync(int id);
        Task<IEnumerable<Gasto>> ObtenerPorUsuarioYRangoFechaAsync(int userId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Gasto>> ObtenerRecientesPorUsuarioAsync(int userId, int cantidad);


    }
}
