
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.Models;

namespace SggApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGastoService _gastoService;
        private readonly IPresupuestoService _presupuestoService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public HomeController(
            ILogger<HomeController> logger,
            IGastoService gastoService,
            IPresupuestoService presupuestoService,
            UserManager<Usuario> userManager,
            IMapper mapper)
        {
            _logger = logger;
            _gastoService = gastoService;
            _presupuestoService = presupuestoService;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return await Dashboard();
            }
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var viewModel = new DashboardViewModel();

            // Obtener gastos del mes actual
            var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);
            var gastosMes = await _gastoService.ObtenerPorUsuarioYRangoFechaAsync(userId.Value, fechaInicio, fechaFin);
            viewModel.GastosMesActual = _mapper.Map<IEnumerable<GastoViewModel>>(gastosMes);
            viewModel.TotalGastosMes = gastosMes.Sum(g => g.Monto);

            // Obtener presupuestos activos
            var presupuestosActivos = await _presupuestoService.ObtenerActivosPorUsuarioAsync(userId.Value);
            viewModel.PresupuestosActivos = _mapper.Map<IEnumerable<PresupuestoViewModel>>(presupuestosActivos);

            // Calcular estado de los presupuestos
            foreach (var presupuesto in viewModel.PresupuestosActivos)
            {
                if (presupuesto.CategoriaId.HasValue)
                {
                    // Presupuesto por categoría
                    var gastosCategoria = gastosMes.Where(g => g.CategoriaId == presupuesto.CategoriaId);
                    presupuesto.MontoGastado = gastosCategoria.Sum(g => g.Monto);
                }
                else
                {
                    // Presupuesto general
                    presupuesto.MontoGastado = viewModel.TotalGastosMes;
                }

                presupuesto.PorcentajeUtilizado = presupuesto.Limite > 0
                    ? Math.Min(100, (presupuesto.MontoGastado / presupuesto.Limite) * 100)
                    : 0;
            }

            // Obtener gastos recientes
            var gastosRecientes = await _gastoService.ObtenerRecientesPorUsuarioAsync(userId.Value, 5);
            viewModel.GastosRecientes = _mapper.Map<IEnumerable<GastoViewModel>>(gastosRecientes);

            return View("Dashboard", viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // --- Helper Methods ---
        private int? GetCurrentUserId()
        {
            var userIdString = _userManager.GetUserId(User);
            if (int.TryParse(userIdString, out int userId)) return userId;
            return null;
        }
    }
}
