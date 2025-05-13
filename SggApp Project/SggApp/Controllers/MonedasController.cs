using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.Models;

namespace SggApp.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Posiblemente solo los administradores deben gestionar monedas
    public class MonedasController : Controller
    {
        private readonly IMonedaService _monedaService;
        private readonly IMapper _mapper;

        public MonedasController(
            IMonedaService monedaService,
            IMapper mapper)
        {
            _monedaService = monedaService;
            _mapper = mapper;
        }

        // GET: /Monedas/ o /Monedas/Index
        // Permite a los usuarios normales ver la lista de monedas
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var monedas = await _monedaService.ObtenerTodasAsync();
            var viewModels = _mapper.Map<IEnumerable<MonedaViewModel>>(monedas);

            return View(viewModels);
        }

        // GET: /Monedas/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var moneda = await _monedaService.ObtenerPorIdAsync(id.Value);
            if (moneda == null) return NotFound();

            var viewModel = _mapper.Map<MonedaViewModel>(moneda);
            return View(viewModel);
        }

        // GET: /Monedas/Create
        public IActionResult Create()
        {
            return View(new MonedaViewModel { Activa = true });
        }

        // POST: /Monedas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonedaViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var moneda = _mapper.Map<Moneda>(viewModel);

                try
                {
                    await _monedaService.AgregarAsync(moneda);
                    TempData["SuccessMessage"] = "Moneda creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") ?? false)
                {
                    ModelState.AddModelError("Codigo", "Ya existe una moneda con este código.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al crear la moneda.");
                }
            }

            return View(viewModel);
        }

        // GET: /Monedas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var moneda = await _monedaService.ObtenerPorIdAsync(id.Value);
            if (moneda == null) return NotFound();

            var viewModel = _mapper.Map<MonedaViewModel>(moneda);
            return View(viewModel);
        }

        // POST: /Monedas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MonedaViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var moneda = _mapper.Map<Moneda>(viewModel);
                    await _monedaService.ActualizarAsync(moneda);

                    TempData["SuccessMessage"] = "Moneda actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MonedaExists(viewModel.Id))
                        return NotFound();
                    else throw;
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") ?? false)
                {
                    ModelState.AddModelError("Codigo", "Ya existe una moneda con este código.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar la moneda.");
                }
            }

            return View(viewModel);
        }

        // GET: /Monedas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var moneda = await _monedaService.ObtenerPorIdAsync(id.Value);
            if (moneda == null) return NotFound();

            var viewModel = _mapper.Map<MonedaViewModel>(moneda);
            return View(viewModel);

        }

        // POST: /Monedas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _monedaService.EliminarAsync(id);
                TempData["SuccessMessage"] = "Moneda eliminada exitosamente.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "No se puede eliminar esta moneda porque está en uso por usuarios, gastos o presupuestos.";
            }

            return RedirectToAction(nameof(Index));
        }

        // --- Helper Methods ---
        private async Task<bool> MonedaExists(int id)
        {
            var moneda = await _monedaService.ObtenerPorIdAsync(id);
            return moneda != null;
        }
    }
}