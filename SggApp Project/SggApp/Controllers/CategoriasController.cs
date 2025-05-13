using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.Models;

namespace SggApp.Web.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        public CategoriasController(
            ICategoriaService categoriaService,
            UserManager<Usuario> userManager,
            IMapper mapper)
        {
            _categoriaService = categoriaService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: /Categorias/ o /Categorias/Index
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var categorias = await _categoriaService.ObtenerPorUsuarioAsync(userId.Value);
            var viewModels = _mapper.Map<IEnumerable<CategoriaViewModel>>(categorias);

            return View(viewModels);
        }

        // GET: /Categorias/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            var viewModel = _mapper.Map<CategoriaViewModel>(categoria);
            return View(viewModel);
        }

        // GET: /Categorias/Create
        public IActionResult Create()
        {
            return View(new CategoriaViewModel { Activa = true });
        }

        // POST: /Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            if (ModelState.IsValid)
            {
                var categoria = _mapper.Map<Categoria>(viewModel);
                categoria.UsuarioId = userId.Value;

                try
                {
                    await _categoriaService.AgregarAsync(categoria);
                    TempData["SuccessMessage"] = "Categoría creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_CategoriaUsuario") ?? false)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al crear la categoría.");
                }
            }

            return View(viewModel);
        }

        // GET: /Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            var viewModel = _mapper.Map<CategoriaViewModel>(categoria);
            return View(viewModel);
        }

        // POST: /Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var originalCategoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (originalCategoria == null || originalCategoria.UsuarioId != userId.Value) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _mapper.Map(viewModel, originalCategoria);
                    originalCategoria.UsuarioId = userId.Value; // Asegurar que no se cambie el usuario

                    await _categoriaService.ActualizarAsync(originalCategoria);
                    TempData["SuccessMessage"] = "Categoría actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await CategoriaExists(viewModel.Id, userId.Value))
                        return NotFound();
                    else throw;
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_CategoriaUsuario") ?? false)
                {
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar la categoría.");
                }
            }

            return View(viewModel);
        }

        // GET: /Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            var viewModel = _mapper.Map<CategoriaViewModel>(categoria);
            return View(viewModel);
        }

        // POST: /Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            try
            {
                await _categoriaService.EliminarAsync(id);
                TempData["SuccessMessage"] = "Categoría eliminada exitosamente.";
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = "No se puede eliminar esta categoría porque tiene gastos o presupuestos asociados.";
            }

            return RedirectToAction(nameof(Index));
        }

        // --- Helper Methods ---
        private int? GetCurrentUserId()
        {
            var userIdString = _userManager.GetUserId(User);
            if (int.TryParse(userIdString, out int userId)) return userId;
            return null;
        }

        private async Task<bool> CategoriaExists(int id, int userId)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            return categoria != null && categoria.UsuarioId == userId;
        }
    }
}
