using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.BLL.Servicios;
using SggApp.DAL.Entidades;
using SggApp.Models;


namespace SggApp.Web.Controllers
{
    [Authorize] // Requiere autenticación para todas las acciones de este controller
    public class GastosController : Controller
    {
        private readonly IGastoService _gastoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMonedaService _monedaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public GastosController(
            IGastoService gastoService,
            ICategoriaService categoriaService,
            IMonedaService monedaService,
            UserManager<Usuario> userManager,
            IMapper mapper)
        {
            _gastoService = gastoService;
            _categoriaService = categoriaService;
            _monedaService = monedaService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: /Gastos/ o /Gastos/Index
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge(); // O Unauthorized() dependiendo de tu flujo de autenticación
            }

            var gastos = await _gastoService.ObtenerPorUsuarioAsync(userId.Value);
            var viewModels = _mapper.Map<IEnumerable<GastoViewModel>>(gastos);

            return View(viewModels);
        }

        // GET: /Gastos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var gasto = await _gastoService.ObtenerPorIdAsync(id.Value);

            if (gasto == null || gasto.UsuarioId != userId.Value)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<GastoViewModel>(gasto);
            return View(viewModel);
        }

        // GET: /Gastos/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new GastoFormViewModel
            {
                Fecha = DateTime.Today,
                CategoriasDisponibles = await GetCategoriasSelectList(),
                MonedasDisponibles = await GetMonedasSelectList()
            };
            return View(viewModel);
        }

        // POST: /Gastos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GastoFormViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            // Eliminar los errores de validación para las propiedades que no deberían validarse
            ModelState.Remove("CategoriasDisponibles");
            ModelState.Remove("MonedasDisponibles");

            if (ModelState.IsValid)
            {
                var gasto = _mapper.Map<Gasto>(viewModel);
                gasto.UsuarioId = userId.Value;

                await _gastoService.AgregarAsync(gasto);

                TempData["SuccessMessage"] = "Gasto creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si el ModelState no es válido, recargar las listas para la vista
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            return View(viewModel);
        }

        // GET: /Gastos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var gasto = await _gastoService.ObtenerPorIdAsync(id.Value);

            if (gasto == null || gasto.UsuarioId != userId.Value)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<GastoFormViewModel>(gasto);
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            return View(viewModel);
        }

        // POST: /Gastos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GastoFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            // Eliminar los errores de validación para las propiedades que no deberían validarse
            ModelState.Remove("CategoriasDisponibles");
            ModelState.Remove("MonedasDisponibles");


            var originalGasto = await _gastoService.ObtenerPorIdAsync(id);
            if (originalGasto == null || originalGasto.UsuarioId != userId.Value)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(viewModel, originalGasto);
                    originalGasto.UsuarioId = userId.Value; // Asegurarse de que el UsuarioId no se sobrescriba si no está en el ViewModel

                    await _gastoService.ActualizarAsync(originalGasto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await GastoExists(viewModel.Id, userId.Value))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw; // Rethrow la excepción si el gasto existe pero hay un problema de concurrencia
                    }
                }

                TempData["SuccessMessage"] = "Gasto actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si el ModelState no es válido, recargar las listas para la vista
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            return View(viewModel);
        }

        // GET: /Gastos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var gasto = await _gastoService.ObtenerPorIdAsync(id.Value);
            if (gasto == null || gasto.UsuarioId != userId.Value)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<GastoViewModel>(gasto);
            return View(viewModel);
        }

        // POST: /Gastos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge();
            }

            var gasto = await _gastoService.ObtenerPorIdAsync(id);
            if (gasto == null || gasto.UsuarioId != userId.Value)
            {
                return NotFound();
            }

            try
            {
                await _gastoService.EliminarAsync(id);
                TempData["SuccessMessage"] = "Gasto eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) // Considera un manejo de errores más específico aquí
            {
                // Log the exception (ej. con ILogger)
                // TempData["ErrorMessage"] = "Ocurrió un error al eliminar el gasto: " + ex.Message;
                return RedirectToAction(nameof(Index)); // O redirigir a una vista de error
            }
        }

        // --- Helper Methods ---
        private int? GetCurrentUserId()
        {
            // Asume que el Id de Usuario en el UserClaims es un string que puede parsearse a int
            var userIdString = _userManager.GetUserId(User);
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            return null;
        }

        private async Task<IEnumerable<SelectListItem>> GetCategoriasSelectList()
        {
            var categorias = await _categoriaService.ObtenerTodasAsync();
            return categorias.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre
            });
        }

        private async Task<IEnumerable<SelectListItem>> GetMonedasSelectList()
        {
            var monedas = await _monedaService.ObtenerTodasAsync();
            return monedas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Nombre} ({m.Codigo})"
            });
        }

        private async Task<bool> GastoExists(int id, int userId)
        {
            var gasto = await _gastoService.ObtenerPorIdAsync(id);
            return gasto != null && gasto.UsuarioId == userId;
        }
    }
}