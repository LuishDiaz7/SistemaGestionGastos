using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SggApp.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<GastoViewModel> GastosMesActual { get; set; }
        public decimal TotalGastosMes { get; set; }
        public IEnumerable<PresupuestoViewModel> PresupuestosActivos { get; set; }
        public IEnumerable<GastoViewModel> GastosRecientes { get; set; }
    }
}
