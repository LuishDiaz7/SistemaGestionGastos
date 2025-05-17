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
using SggApp.DAL.Entidades;
using SggApp.Models;

namespace SggApp.Web.Controllers
{
    /// <summary>
    /// Controlador MVC para la gestión de gastos por parte del usuario autenticado.
    /// Proporciona acciones para listar, ver detalles, crear, editar y eliminar gastos personales.
    /// </summary>
    [Authorize] // Requiere que el usuario esté autenticado para acceder a cualquier acción en este controlador.
    public class GastosController : Controller
    {
        private readonly IGastoService _gastoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMonedaService _monedaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de gastos.
        /// </summary>
        /// <param name="gastoService">Servicio para operaciones de negocio relacionadas con gastos.</param>
        /// <param name="categoriaService">Servicio para operaciones de negocio relacionadas con categorías.</param>
        /// <param name="monedaService">Servicio para operaciones de negocio relacionadas con monedas.</param>
        /// <param name="userManager">Gestor de usuarios de ASP.NET Core Identity para obtener el usuario actual.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
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

        /// <summary>
        /// Muestra la lista de gastos registrados por el usuario autenticado.
        /// Requiere autenticación.
        /// </summary>
        /// <returns>Una vista que presenta un listado de gastos.</returns>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado (aunque [Authorize] ya lo maneja).
            if (userId == null) return Challenge();

            // Obtiene los gastos del usuario desde el servicio de negocio.
            var gastos = await _gastoService.ObtenerPorUsuarioAsync(userId.Value);
            // Mapea la colección de entidades Gasto a una colección de ViewModels para la vista.
            var viewModels = _mapper.Map<IEnumerable<GastoViewModel>>(gastos);

            // Retorna la vista Index con la lista de ViewModels.
            return View(viewModels);
        }

        /// <summary>
        /// Muestra los detalles de un gasto específico perteneciente al usuario autenticado.
        /// Requiere autenticación.
        /// </summary>
        /// <param name="id">El ID del gasto a mostrar.</param>
        /// <returns>Una vista que presenta los detalles del gasto o un resultado NotFound/Challenge.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene el gasto por su ID desde el servicio.
            var gasto = await _gastoService.ObtenerPorIdAsync(id.Value);

            // Retorna NotFound si el gasto no existe o no pertenece al usuario actual.
            if (gasto == null || gasto.UsuarioId != userId.Value) return NotFound();

            // Mapea la entidad Gasto a un ViewModel para la vista de detalles.
            var viewModel = _mapper.Map<GastoViewModel>(gasto);
            // Retorna la vista Details con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para registrar un nuevo gasto.
        /// Requiere autenticación.
        /// </summary>
        /// <returns>Una vista que presenta el formulario de creación de gasto.</returns>
        public async Task<IActionResult> Create()
        {
            // Inicializa el ViewModel para el formulario de creación.
            var viewModel = new GastoFormViewModel
            {
                Fecha = DateTime.Today, // Establece la fecha por defecto al día actual.
                // Obtiene y asigna las listas de categorías y monedas disponibles para los selectores.
                CategoriasDisponibles = await GetCategoriasSelectList(),
                MonedasDisponibles = await GetMonedasSelectList()
            };
            // Retorna la vista Create con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para crear un nuevo gasto.
        /// Requiere autenticación y validación de token anti-falsificación.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene los datos del nuevo gasto enviado desde el formulario.</param>
        /// <returns>Redirecciona a la acción Index si el gasto se crea exitosamente; de lo contrario, regresa a la vista del formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Create(GastoFormViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Elimina del estado del modelo los errores relacionados con las listas de selectores,
            // ya que estos ViewModels son solo para la vista y no vienen del formulario POST.
            ModelState.Remove("CategoriasDisponibles");
            ModelState.Remove("MonedasDisponibles");

            // Verifica si el estado del modelo es válido según las validaciones definidas en el ViewModel.
            if (ModelState.IsValid)
            {
                // Mapea los datos validados del ViewModel a la entidad Gasto.
                var gasto = _mapper.Map<Gasto>(viewModel);
                // Asigna el ID del usuario actual a la entidad gasto antes de guardarla.
                gasto.UsuarioId = userId.Value;

                // Agrega el nuevo gasto a través del servicio de negocio.
                await _gastoService.AgregarAsync(gasto);

                // Establece un mensaje de éxito temporal para mostrar en la próxima solicitud (Index).
                TempData["SuccessMessage"] = "Gasto creado exitosamente.";
                // Redirecciona al usuario a la página de lista de gastos.
                return RedirectToAction(nameof(Index));
            }

            // Si el estado del modelo no es válido, se recargan las listas de selectores
            // para que se muestren correctamente en la vista del formulario al regresar.
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Create con el ViewModel actual (conteniendo errores de validación y listas recargadas).
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario precargado para editar un gasto existente.
        /// Requiere autenticación.
        /// </summary>
        /// <param name="id">El ID del gasto a editar.</param>
        /// <returns>Una vista que presenta el formulario de edición con los datos del gasto o un resultado NotFound/Challenge.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene el gasto por su ID desde el servicio.
            var gasto = await _gastoService.ObtenerPorIdAsync(id.Value);

            // Retorna NotFound si el gasto no existe o no pertenece al usuario actual.
            if (gasto == null || gasto.UsuarioId != userId.Value) return NotFound();

            // Mapea la entidad Gasto a un ViewModel de formulario para rellenar los campos de edición.
            var viewModel = _mapper.Map<GastoFormViewModel>(gasto);
            // Obtiene y asigna las listas de categorías y monedas disponibles para los selectores.
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para actualizar un gasto existente.
        /// Requiere autenticación y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID del gasto que se espera actualizar (de la URL).</param>
        /// <param name="viewModel">ViewModel que contiene los datos actualizados del gasto enviado desde el formulario.</param>
        /// <returns>Redirecciona a la acción Index si el gasto se actualiza exitosamente; de lo contrario, regresa a la vista del formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Edit(int id, GastoFormViewModel viewModel)
        {
            // Retorna NotFound si el ID de la URL no coincide con el ID del ViewModel.
            if (id != viewModel.Id) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Elimina del estado del modelo los errores relacionados con las listas de selectores,
            // ya que estos ViewModels no vienen del formulario POST.
            ModelState.Remove("CategoriasDisponibles");
            ModelState.Remove("MonedasDisponibles");

            // Obtiene la entidad gasto original de la base de datos para verificar existencia y pertenencia antes de actualizar.
            var originalGasto = await _gastoService.ObtenerPorIdAsync(id);
            // Retorna NotFound si el gasto original no existe o no pertenece al usuario actual.
            if (originalGasto == null || originalGasto.UsuarioId != userId.Value) return NotFound();

            // Verifica si el estado del modelo es válido.
            if (ModelState.IsValid)
            {
                try
                {
                    // Mapea los datos actualizados del ViewModel sobre la entidad original obtenida de la base de datos.
                    // Esto actualiza las propiedades de la entidad con los nuevos valores del formulario.
                    _mapper.Map(viewModel, originalGasto);
                    // Asegura que el UsuarioId de la entidad original no sea sobrescrito por el mapeo, manteniendo al dueño.
                    originalGasto.UsuarioId = userId.Value;

                    // Actualiza el gasto a través del servicio de negocio.
                    await _gastoService.ActualizarAsync(originalGasto);

                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Gasto actualizado exitosamente.";
                    // Redirecciona al usuario a la página de lista de gastos.
                    return RedirectToAction(nameof(Index));
                }
                // Captura excepción de concurrencia si el registro fue modificado o eliminado por otro proceso.
                catch (DbUpdateConcurrencyException)
                {
                    // Verifica si el gasto aún existe para el usuario actual.
                    if (!await GastoExists(viewModel.Id, userId.Value))
                    {
                        // Retorna NotFound si el gasto ya no existe (fue eliminado por otro proceso).
                        return NotFound();
                    }
                    else
                    {
                        // Relanza la excepción si el gasto existe pero hay un problema de concurrencia inesperado.
                        throw;
                    }
                }
                // Opcional: Considerar captura de otras excepciones genéricas o específicas de base de datos.
                // catch (Exception ex) { ... }
            }

            // Si el estado del modelo no es válido, se recargan las listas de selectores
            // para que se muestren correctamente en la vista del formulario al regresar.
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Edit con el ViewModel actual (conteniendo errores de validación y listas recargadas).
            return View(viewModel);
        }

        /// <summary>
        /// Muestra la vista de confirmación para eliminar un gasto.
        /// Requiere autenticación.
        /// </summary>
        /// <param name="id">El ID del gasto a eliminar.</param>
        /// <returns>Una vista que presenta los detalles del gasto para confirmar la eliminación o un resultado NotFound/Challenge.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene el gasto por su ID desde el servicio.
            var gasto = await _gastoService.ObtenerPorIdAsync(id.Value);

            // Retorna NotFound si el gasto no existe o no pertenece al usuario actual.
            if (gasto == null || gasto.UsuarioId != userId.Value) return NotFound();

            // Mapea la entidad Gasto a un ViewModel para mostrar detalles en la vista de confirmación.
            var viewModel = _mapper.Map<GastoViewModel>(gasto);
            // Retorna la vista Delete con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de eliminación confirmada de un gasto.
        /// Requiere autenticación y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID del gasto a eliminar.</param>
        /// <returns>Redirecciona a la acción Index después de intentar eliminar el gasto.</returns>
        [HttpPost, ActionName("Delete")] // Designa este método como la acción POST para /Gastos/Delete.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el usuario no está identificado.
            if (userId == null) return Challenge();

            // Obtiene el gasto por su ID para verificar existencia y pertenencia antes de eliminar.
            var gasto = await _gastoService.ObtenerPorIdAsync(id);
            // Retorna NotFound si el gasto no existe o no pertenece al usuario actual.
            if (gasto == null || gasto.UsuarioId != userId.Value) return NotFound();

            try
            {
                // Elimina el gasto a través del servicio de negocio.
                await _gastoService.EliminarAsync(id);
                // Establece un mensaje de éxito temporal.
                TempData["SuccessMessage"] = "Gasto eliminado exitosamente.";
            }
            // Captura cualquier excepción durante la eliminación.
            catch (Exception)
            {
                // Establece un mensaje de error genérico temporal. Considerar loggear la excepción real.
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el gasto.";
                // Nota: Un manejo de errores más específico para restricciones de clave foránea podría ser útil aquí.
            }

            // Redirecciona a la acción Index después del intento de eliminación.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Obtiene el ID del usuario actualmente autenticado.
        /// </summary>
        /// <returns>El ID del usuario autenticado como un entero anulable, o null si el usuario no está autenticado o el ID no es válido.</returns>
        private int? GetCurrentUserId()
        {
            // Utiliza UserManager para obtener el ID del usuario autenticado como string desde los Claims.
            var userIdString = _userManager.GetUserId(User);
            // Intenta convertir el ID string a un entero.
            if (int.TryParse(userIdString, out int userId))
            {
                // Retorna el ID entero si la conversión es exitosa.
                return userId;
            }
            // Retorna null si el usuario no está autenticado o el ID string no es un entero válido.
            return null;
        }

        /// <summary>
        /// Obtiene una lista de elementos SelectListItem con las categorías disponibles para el usuario autenticado.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de SelectListItem para categorías.</returns>
        private async Task<IEnumerable<SelectListItem>> GetCategoriasSelectList()
        {
            // Obtiene todas las categorías a través del servicio.
            var categorias = await _categoriaService.ObtenerTodasAsync();
            // Proyecta las entidades Categoria a objetos SelectListItem, usando el ID como valor y el Nombre como texto.
            return categorias.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Nombre
            });
        }

        /// <summary>
        /// Obtiene una lista de elementos SelectListItem con las monedas disponibles.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una colección de SelectListItem para monedas.</returns>
        private async Task<IEnumerable<SelectListItem>> GetMonedasSelectList()
        {
            // Obtiene todas las monedas a través del servicio.
            var monedas = await _monedaService.ObtenerTodasAsync();
            // Proyecta las entidades Moneda a objetos SelectListItem, usando el ID como valor y Nombre (Código) como texto.
            return monedas.Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Nombre} ({m.Codigo})" // Formato común para mostrar moneda (ej. "Dólar (USD)").
            });
        }

        /// <summary>
        /// Verifica si un gasto con el ID especificado existe y pertenece al usuario dado.
        /// </summary>
        /// <param name="id">El ID del gasto a verificar.</param>
        /// <param name="userId">El ID del usuario al que debe pertenecer el gasto.</param>
        /// <returns>True si el gasto existe y pertenece al usuario; de lo contrario, False.</returns>
        private async Task<bool> GastoExists(int id, int userId)
        {
            // Obtiene el gasto por su ID desde el servicio.
            var gasto = await _gastoService.ObtenerPorIdAsync(id);
            // Retorna true si el gasto no es nulo y su UsuarioId coincide con el ID proporcionado.
            return gasto != null && gasto.UsuarioId == userId;
        }
    }
}