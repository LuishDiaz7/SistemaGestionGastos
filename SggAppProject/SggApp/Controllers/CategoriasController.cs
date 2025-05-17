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
    /// <summary>
    /// Controlador MVC para la gestión de categorías por parte del usuario autenticado.
    /// Permite listar, ver detalles, crear, editar y eliminar categorías personales.
    /// </summary>
    // [Authorize] // Se puede descomentar para requerir autenticación en todas las acciones.
    public class CategoriasController : Controller
    {
        private readonly ICategoriaService _categoriaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de categorías.
        /// </summary>
        /// <param name="categoriaService">Servicio para operaciones de negocio relacionadas con categorías.</param>
        /// <param name="userManager">Gestor de usuarios de ASP.NET Core Identity para obtener el usuario actual.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
        public CategoriasController(
            ICategoriaService categoriaService,
            UserManager<Usuario> userManager,
            IMapper mapper)
        {
            _categoriaService = categoriaService;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Muestra la lista de categorías pertenecientes al usuario autenticado.
        /// </summary>
        /// <returns>Una vista con la lista de categorías.</returns>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene las categorías del usuario desde el servicio.
            var categorias = await _categoriaService.ObtenerPorUsuarioAsync(userId.Value);
            // Mapea las entidades Categoria a ViewModels para la vista.
            var viewModels = _mapper.Map<IEnumerable<CategoriaViewModel>>(categorias);

            // Retorna la vista Index con los ViewModels.
            return View(viewModels);
        }

        /// <summary>
        /// Muestra los detalles de una categoría específica.
        /// </summary>
        /// <param name="id">El ID de la categoría a mostrar.</param>
        /// <returns>Una vista con los detalles de la categoría o NotFound si no existe/no pertenece al usuario.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            // Retorna NotFound si el ID es nulo.
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene la categoría por su ID desde el servicio.
            var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si la categoría no existe o no pertenece al usuario actual.
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            // Mapea la entidad Categoria a un ViewModel para la vista de detalles.
            var viewModel = _mapper.Map<CategoriaViewModel>(categoria);
            // Retorna la vista Details con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para crear una nueva categoría.
        /// </summary>
        /// <returns>Una vista con el formulario de creación.</returns>
        public IActionResult Create()
        {
            // Retorna la vista Create con un ViewModel inicializado (Activa=true por defecto).
            return View(new CategoriaViewModel { Activa = true });
        }

        /// <summary>
        /// Procesa la solicitud de creación de una nueva categoría.
        /// </summary>
        /// <param name="viewModel">ViewModel con los datos del formulario enviado.</param>
        /// <returns>Redirecciona a la lista de categorías si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoriaViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Verifica si el estado del modelo es válido según las validaciones del ViewModel.
            if (ModelState.IsValid)
            {
                // Mapea el ViewModel a la entidad Categoria.
                var categoria = _mapper.Map<Categoria>(viewModel);
                // Asigna el ID del usuario actual a la categoría.
                categoria.UsuarioId = userId.Value;

                try
                {
                    // Agrega la nueva categoría a través del servicio.
                    await _categoriaService.AgregarAsync(categoria);
                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Categoría creada exitosamente.";
                    // Redirecciona a la acción Index.
                    return RedirectToAction(nameof(Index));
                }
                // Captura excepción si el nombre de la categoría ya existe para este usuario (violación de UQ_CategoriaUsuario).
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_CategoriaUsuario") ?? false)
                {
                    // Agrega un error al ModelState para mostrar en la vista.
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
                }
                // Captura otras posibles excepciones durante el guardado.
                catch (Exception)
                {
                    // Agrega un error general al ModelState.
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al crear la categoría.");
                }
            }

            // Si el ModelState no es válido o hubo un error al guardar, regresa a la vista Create con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para editar una categoría existente.
        /// </summary>
        /// <param name="id">El ID de la categoría a editar.</param>
        /// <returns>Una vista con el formulario de edición precargado o NotFound si no existe/no pertenece al usuario.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // Retorna NotFound si el ID es nulo.
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene la categoría por su ID desde el servicio.
            var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si la categoría no existe o no pertenece al usuario actual.
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            // Mapea la entidad Categoria a un ViewModel para rellenar el formulario de edición.
            var viewModel = _mapper.Map<CategoriaViewModel>(categoria);
            // Retorna la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de actualización de una categoría existente.
        /// </summary>
        /// <param name="id">El ID de la categoría que se está editando.</param>
        /// <param name="viewModel">ViewModel con los datos enviados desde el formulario de edición.</param>
        /// <returns>Redirecciona a la lista de categorías si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoriaViewModel viewModel)
        {
            // Retorna NotFound si el ID de la URL no coincide con el ID del ViewModel.
            if (id != viewModel.Id) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene la categoría original de la base de datos para verificar existencia y pertenencia.
            var originalCategoria = await _categoriaService.ObtenerPorIdAsync(id);
            // Retorna NotFound si la categoría original no existe o no pertenece al usuario actual.
            if (originalCategoria == null || originalCategoria.UsuarioId != userId.Value) return NotFound();

            // Verifica si el estado del modelo es válido.
            if (ModelState.IsValid)
            {
                try
                {
                    // Mapea los datos del ViewModel a la entidad original obtenida de la base de datos.
                    _mapper.Map(viewModel, originalCategoria);
                    // Asegura que el UsuarioId de la entidad original no sea sobrescrito si no está en el ViewModel.
                    originalCategoria.UsuarioId = userId.Value;

                    // Actualiza la categoría a través del servicio.
                    await _categoriaService.ActualizarAsync(originalCategoria);
                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Categoría actualizada exitosamente.";
                    // Redirecciona a la acción Index.
                    return RedirectToAction(nameof(Index));
                }
                // Captura excepción de concurrencia si el registro fue modificado por otro proceso.
                catch (DbUpdateConcurrencyException)
                {
                    // Verifica si la categoría aún existe para el usuario actual.
                    if (!await CategoriaExists(viewModel.Id, userId.Value))
                        // Retorna NotFound si la categoría ya no existe (fue eliminada).
                        return NotFound();
                    else
                        // Relanza la excepción si la categoría existe pero hay un problema de concurrencia.
                        throw;
                }
                // Captura excepción si el nuevo nombre de categoría ya existe para este usuario.
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_CategoriaUsuario") ?? false)
                {
                    // Agrega un error al ModelState para mostrar en la vista.
                    ModelState.AddModelError("Nombre", "Ya existe una categoría con este nombre.");
                }
                // Captura otras posibles excepciones durante el guardado.
                catch (Exception)
                {
                    // Agrega un error general al ModelState.
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar la categoría.");
                }
            }

            // Si el ModelState no es válido o hubo un error al guardar, regresa a la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra la vista de confirmación para eliminar una categoría.
        /// </summary>
        /// <param name="id">El ID de la categoría a eliminar.</param>
        /// <returns>Una vista con los detalles de la categoría para confirmar la eliminación o NotFound si no existe/no pertenece al usuario.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // Retorna NotFound si el ID es nulo.
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene la categoría por su ID desde el servicio.
            var categoria = await _categoriaService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si la categoría no existe o no pertenece al usuario actual.
            if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            // Mapea la entidad Categoria a un ViewModel para mostrar detalles en la vista de eliminación.
            var viewModel = _mapper.Map<CategoriaViewModel>(categoria);
            // Retorna la vista Delete con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de eliminación confirmada de una categoría.
        /// </summary>
        /// <param name="id">El ID de la categoría a eliminar.</param>
        /// <returns>Redirecciona a la lista de categorías si es exitoso; de lo contrario, regresa a la lista con un mensaje de error.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Opcional: Verificar si la categoría aún existe y pertenece al usuario antes de intentar eliminar.
            // var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            // if (categoria == null || categoria.UsuarioId != userId.Value) return NotFound();

            try
            {
                // Elimina la categoría a través del servicio.
                await _categoriaService.EliminarAsync(id);
                // Establece un mensaje de éxito temporal.
                TempData["SuccessMessage"] = "Categoría eliminada exitosamente.";
            }
            // Captura excepción si la categoría no se puede eliminar debido a relaciones existentes (gastos/presupuestos asociados).
            catch (DbUpdateException)
            {
                // Establece un mensaje de error temporal indicando la restricción.
                TempData["ErrorMessage"] = "No se puede eliminar esta categoría porque tiene gastos o presupuestos asociados.";
            }
            // Opcional: Considerar capturar otras excepciones generales y registrarlas.
            // catch (Exception ex) { ... }

            // Redirecciona a la acción Index después del intento de eliminación.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Obtiene el ID del usuario actualmente autenticado.
        /// </summary>
        /// <returns>El ID del usuario autenticado como un entero anulable, o null si el usuario no está autenticado o el ID no es válido.</returns>
        private int? GetCurrentUserId()
        {
            // Utiliza UserManager para obtener el ID del usuario autenticado como string.
            var userIdString = _userManager.GetUserId(User);
            // Intenta parsear el ID string a un entero.
            if (int.TryParse(userIdString, out int userId))
            {
                // Retorna el ID entero si el parseo es exitoso.
                return userId;
            }
            // Retorna null si el usuario no está autenticado o el ID no es un entero válido.
            return null;
        }

        /// <summary>
        /// Verifica si una categoría con el ID especificado existe y pertenece al usuario dado.
        /// </summary>
        /// <param name="id">El ID de la categoría a verificar.</param>
        /// <param name="userId">El ID del usuario al que debe pertenecer la categoría.</param>
        /// <returns>True si la categoría existe y pertenece al usuario; de lo contrario, False.</returns>
        private async Task<bool> CategoriaExists(int id, int userId)
        {
            // Obtiene la categoría por su ID desde el servicio.
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            // Retorna true si la categoría no es nula y su UsuarioId coincide con el ID proporcionado.
            return categoria != null && categoria.UsuarioId == userId;
        }
    }
}
