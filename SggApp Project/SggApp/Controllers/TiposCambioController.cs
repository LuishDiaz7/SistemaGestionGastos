using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.Models;

namespace SggApp.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Normalmente solo los administradores deben gestionar tipos de cambio
    public class TiposCambioController : Controller
    {
        private readonly ITipoCambioService _tipoCambioService;
        private readonly IMonedaService _monedaService;
        private readonly IMapper _mapper;

        public TiposCambioController(
            ITipoCambioService tipoCambioService,
            IMonedaService monedaService,
            IMapper mapper)
        {
            _tipoCambioService = tipoCambioService;
            _monedaService = monedaService;
            _mapper = mapper;
        }

        // GET: /TiposCambio/ o /TiposCambio/Index
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var tiposCambio = await _tipoCambioService.ObtenerTodosAsync();
            var viewModels = _mapper.Map<IEnumerable<TipoCambioViewModel>>(tiposCambio);

            return View(viewModels);
        }

        // GET: /TiposCambio/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id.Value);
            if (tipoCambio == null) return NotFound();

            var viewModel = _mapper.Map<TipoCambioViewModel>(tipoCambio);
            return View(viewModel);
        }

        // GET: /TiposCambio/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new TipoCambioFormViewModel
            {
                FechaActualizacion = DateTime.Now,
                MonedasDisponibles = await GetMonedasSelectList()
            };
            return View(viewModel);
        }

        // POST: /TiposCambio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TipoCambioFormViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.MonedaOrigenId == viewModel.MonedaDestinoId)
                {
                    ModelState.AddModelError("MonedaDestinoId", "La moneda de destino debe ser diferente a la moneda de origen.");
                }
                else
                {
                    var tipoCambio = _mapper.Map<TipoCambio>(viewModel);
                    tipoCambio.FechaActualizacion = DateTime.Now;

                    try
                    {
                        await _tipoCambioService.AgregarAsync(tipoCambio);
                        TempData["SuccessMessage"] = "Tipo de cambio creado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_TiposCambio") ?? false)
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un tipo de cambio para este par de monedas.");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Ha ocurrido un error al crear el tipo de cambio.");
                    }
                }
            }

            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        // GET: /TiposCambio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id.Value);
            if (tipoCambio == null) return NotFound();

            var viewModel = _mapper.Map<TipoCambioFormViewModel>(tipoCambio);
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            return View(viewModel);
        }

        // POST: /TiposCambio/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TipoCambioFormViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (viewModel.MonedaOrigenId == viewModel.MonedaDestinoId)
                {
                    ModelState.AddModelError("MonedaDestinoId", "La moneda de destino debe ser diferente a la moneda de origen.");
                }
                else
                {
                    try
                    {
                        var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id);
                        if (tipoCambio == null) return NotFound();

                        _mapper.Map(viewModel, tipoCambio);
                        tipoCambio.FechaActualizacion = DateTime.Now;

                        await _tipoCambioService.ActualizarAsync(tipoCambio);
                        TempData["SuccessMessage"] = "Tipo de cambio actualizado exitosamente.";
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!await TipoCambioExists(viewModel.Id))
                            return NotFound();
                        else throw;
                    }
                    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_TiposCambio") ?? false)
                    {
                        ModelState.AddModelError(string.Empty, "Ya existe un tipo de cambio para este par de monedas.");
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar el tipo de cambio.");
                    }
                }
            }

            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        // GET: /TiposCambio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id.Value);
            if (tipoCambio == null) return NotFound();

            var viewModel = _mapper.Map<TipoCambioViewModel>(tipoCambio);
            return View(viewModel);
        }

        // POST: /TiposCambio/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _tipoCambioService.EliminarAsync(id);
                TempData["SuccessMessage"] = "Tipo de cambio eliminado exitosamente.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Ha ocurrido un error al eliminar el tipo de cambio.";
            }

            return RedirectToAction(nameof(Index));
        }

        // --- Helper Methods ---
        private async Task<IEnumerable<SelectListItem>> GetMonedasSelectList()
        {
            var monedas = await _monedaService.ObtenerTodasAsync();
            return monedas.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = $"{m.Nombre} ({m.Codigo})" });
        }

        private async Task<bool> TipoCambioExists(int id)
        {
            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id);
            return tipoCambio != null;
        }
    }
}
