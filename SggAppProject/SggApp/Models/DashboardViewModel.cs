using System.Collections.Generic; 

namespace SggApp.Models
{
    /// <summary>
    /// ViewModel utilizado para agrupar y presentar datos en la vista del dashboard.
    /// Contiene resúmenes de gastos y presupuestos para el usuario autenticado.
    /// </summary>
    public class DashboardViewModel
    {
        /// <summary>
        /// Obtiene o establece la colección de gastos registrados durante el mes actual.
        /// </summary>
        public IEnumerable<GastoViewModel> GastosMesActual { get; set; }

        /// <summary>
        /// Obtiene o establece el monto total de los gastos registrados durante el mes actual.
        /// </summary>
        public decimal TotalGastosMes { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de presupuestos activos para el usuario.
        /// Estos ViewModels incluyen el cálculo del monto gastado y porcentaje utilizado.
        /// </summary>
        public IEnumerable<PresupuestoViewModel> PresupuestosActivos { get; set; }

        /// <summary>
        /// Obtiene o establece la colección de los gastos más recientes registrados por el usuario.
        /// </summary>
        public IEnumerable<GastoViewModel> GastosRecientes { get; set; }
    }
}
