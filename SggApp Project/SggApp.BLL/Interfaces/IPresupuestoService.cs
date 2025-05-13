using SggApp.DAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SggApp.BLL.Interfaces
{
    public interface IPresupuestoService
    {
        Task<IEnumerable<Presupuesto>> ObtenerTodosAsync();
        Task<Presupuesto> ObtenerPorIdAsync(int id);
        Task<IEnumerable<Presupuesto>> ObtenerPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Presupuesto>> ObtenerPresupuestosVigentesAsync(int usuarioId);
        Task<IEnumerable<Presupuesto>> ObtenerPresupuestosPorCategoriaAsync(int categoriaId);
        Task<decimal> ObtenerGastoActualAsync(int presupuestoId);
        Task<decimal> ObtenerPorcentajeConsumidoAsync(int presupuestoId);
        Task<bool> VerificarAlertaPresupuestoAsync(int presupuestoId);
        Task AgregarAsync(Presupuesto presupuesto);
        Task ActualizarAsync(Presupuesto presupuesto);
        Task EliminarAsync(int id);
        Task<IEnumerable<Presupuesto>> ObtenerActivosPorUsuarioAsync(int userId);

    }
}
