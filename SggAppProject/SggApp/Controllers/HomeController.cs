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
    /// <summary>
    /// Controlador MVC para las p�ginas principales de la aplicaci�n, incluyendo la p�gina de inicio y el dashboard.
    /// Gestiona la navegaci�n inicial y presenta un resumen financiero al usuario autenticado.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGastoService _gastoService;
        private readonly IPresupuestoService _presupuestoService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador Home.
        /// </summary>
        /// <param name="logger">Instancia del logger para registrar informaci�n.</param>
        /// <param name="gastoService">Servicio para operaciones de negocio relacionadas con gastos.</param>
        /// <param name="presupuestoService">Servicio para operaciones de negocio relacionadas con presupuestos.</param>
        /// <param name="userManager">Gestor de usuarios de ASP.NET Core Identity para obtener el usuario actual.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
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

        /// <summary>
        /// Acci�n principal que redirige al dashboard si el usuario est� autenticado,
        /// de lo contrario, muestra la p�gina de inicio p�blica.
        /// </summary>
        /// <returns>Una tarea que representa la operaci�n as�ncrona. El resultado es una redirecci�n al Dashboard o la vista Index.</returns>
        public async Task<IActionResult> Index()
        {
            // Verifica si el usuario actual est� autenticado.
            if (User.Identity?.IsAuthenticated == true)
            {
                // Si est� autenticado, redirige o llama a la acci�n Dashboard.
                return await Dashboard();
            }
            // Si no est� autenticado, muestra la vista de inicio predeterminada (p�gina p�blica).
            return View();
        }

        /// <summary>
        /// Muestra el dashboard del usuario autenticado con un resumen financiero.
        /// Requiere autenticaci�n.
        /// </summary>
        /// <returns>Una tarea que representa la operaci�n as�ncrona. El resultado es la vista Dashboard con datos o un resultado Challenge.</returns>
        [Authorize] // Asegura que solo los usuarios autenticados puedan acceder a este dashboard.
        public async Task<IActionResult> Dashboard()
        {
            var userId = GetCurrentUserId();
            // Retorna un desaf�o de autenticaci�n si el ID de usuario no se puede obtener.
            if (userId == null) return Challenge();

            // Inicializa un nuevo ViewModel para la vista del dashboard.
            var viewModel = new DashboardViewModel();

            // --- Obtenci�n y C�lculo de Datos ---

            // Define el rango de fechas para el mes actual.
            var fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            // Obtiene los gastos del usuario para el mes actual desde el servicio.
            var gastosMes = await _gastoService.ObtenerPorUsuarioYRangoFechaAsync(userId.Value, fechaInicio, fechaFin);
            // Mapea las entidades de gastos a ViewModels y calcula el total de gastos del mes.
            viewModel.GastosMesActual = _mapper.Map<IEnumerable<GastoViewModel>>(gastosMes);
            viewModel.TotalGastosMes = gastosMes.Sum(g => g.Monto);

            // Obtiene los presupuestos activos del usuario desde el servicio.
            var presupuestosActivos = await _presupuestoService.ObtenerActivosPorUsuarioAsync(userId.Value);
            // Mapea las entidades de presupuestos a ViewModels.
            viewModel.PresupuestosActivos = _mapper.Map<IEnumerable<PresupuestoViewModel>>(presupuestosActivos);

            // Calcula el monto gastado y el porcentaje utilizado para cada presupuesto activo.
            foreach (var presupuesto in viewModel.PresupuestosActivos)
            {
                // Si el presupuesto es por categor�a, suma los gastos de esa categor�a en el mes.
                if (presupuesto.CategoriaId.HasValue)
                {
                    var gastosCategoria = gastosMes.Where(g => g.CategoriaId == presupuesto.CategoriaId);
                    presupuesto.MontoGastado = gastosCategoria.Sum(g => g.Monto);
                }
                // Si es un presupuesto general, usa el total de gastos del mes.
                else
                {
                    presupuesto.MontoGastado = viewModel.TotalGastosMes;
                }

                // Calcula el porcentaje utilizado, evitando divisi�n por cero.
                presupuesto.PorcentajeUtilizado = presupuesto.Limite > 0
                    ? Math.Min(100, (presupuesto.MontoGastado / presupuesto.Limite) * 100) // Limita el porcentaje a 100% para la visualizaci�n.
                    : 0;
            }

            // Obtiene una lista de gastos recientes del usuario desde el servicio.
            var gastosRecientes = await _gastoService.ObtenerRecientesPorUsuarioAsync(userId.Value, 5);
            // Mapea las entidades de gastos recientes a ViewModels.
            viewModel.GastosRecientes = _mapper.Map<IEnumerable<GastoViewModel>>(gastosRecientes);

            // Retorna la vista "Dashboard" con el ViewModel cargado de datos.
            return View("Dashboard", viewModel);
        }

        /// <summary>
        /// Muestra la p�gina de pol�tica de privacidad de la aplicaci�n.
        /// </summary>
        /// <returns>Una vista que presenta la pol�tica de privacidad.</returns>
        public IActionResult Privacy()
        {
            // Retorna la vista Privacy.
            return View();
        }

        /// <summary>
        /// Muestra la p�gina de error de la aplicaci�n.
        /// No almacena en cach� la respuesta.
        /// </summary>
        /// <returns>Una vista que presenta los detalles del error.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Crea un ViewModel b�sico para la vista de error, incluyendo el ID de la solicitud actual.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Obtiene el ID del usuario actualmente autenticado.
        /// </summary>
        /// <returns>El ID del usuario autenticado como un entero anulable, o null si el usuario no est� autenticado o el ID no es v�lido.</returns>
        private int? GetCurrentUserId()
        {
            // Utiliza UserManager para obtener el ID del usuario autenticado como string desde los Claims.
            var userIdString = _userManager.GetUserId(User);
            // Intenta convertir el ID string a un entero.
            if (int.TryParse(userIdString, out int userId))
            {
                // Retorna el ID entero si la conversi�n es exitosa.
                return userId;
            }
            // Retorna null si el usuario no est� autenticado o el ID string no es un entero v�lido.
            return null;
        }
    }
}
