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
    /// Controlador MVC para la gestión de presupuestos por parte del usuario autenticado.
    /// Permite listar, ver detalles, crear, editar y eliminar presupuestos personales.
    /// Requiere que el usuario esté autenticado para la mayoría de sus acciones.
    /// </summary>
    // [Authorize] // Este atributo puede descomentarse para aplicar autorización a nivel de controlador.
    public class PresupuestosController : Controller
    {
        private readonly IPresupuestoService _presupuestoService;
        private readonly ICategoriaService _categoriaService;
        private readonly IMonedaService _monedaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de presupuestos.
        /// </summary>
        /// <param name="presupuestoService">Servicio para operaciones de negocio relacionadas con presupuestos.</param>
        /// <param name="categoriaService">Servicio para operaciones de negocio relacionadas con categorías.</param>
        /// <param name="monedaService">Servicio para operaciones de negocio relacionadas con monedas.</param>
        /// <param name="userManager">Gestor de usuarios de ASP.NET Core Identity para obtener el usuario actual.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
        public PresupuestosController(
            IPresupuestoService presupuestoService,
            ICategoriaService categoriaService,
            IMonedaService monedaService,
            UserManager<Usuario> userManager,
            IMapper mapper)
        {
            _presupuestoService = presupuestoService;
            _categoriaService = categoriaService;
            _monedaService = monedaService;
            _userManager = userManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Muestra la lista de presupuestos pertenecientes al usuario autenticado.
        /// Requiere que el usuario esté identificado.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con la lista de presupuestos o un resultado Challenge.</returns>
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene los presupuestos del usuario desde el servicio.
            var presupuestos = await _presupuestoService.ObtenerPorUsuarioAsync(userId.Value);
            // Mapea la colección de entidades Presupuesto a una colección de ViewModels para la vista.
            var viewModels = _mapper.Map<IEnumerable<PresupuestoViewModel>>(presupuestos);

            // Retorna la vista Index con la lista de ViewModels.
            return View(viewModels);
        }

        /// <summary>
        /// Muestra los detalles de un presupuesto específico perteneciente al usuario autenticado.
        /// Requiere que el usuario esté identificado.
        /// </summary>
        /// <param name="id">El ID del presupuesto a mostrar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con los detalles del presupuesto o un resultado NotFound/Challenge.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                // Retorna NotFound si el ID es nulo.
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene el presupuesto por su ID desde el servicio.
            var presupuesto = await _presupuestoService.ObtenerPorIdAsync(id.Value);

            if (presupuesto == null || presupuesto.UsuarioId != userId.Value)
            {
                // Retorna NotFound si el presupuesto no existe o no pertenece al usuario actual.
                return NotFound();
            }

            // Mapea la entidad Presupuesto a un ViewModel para la vista de detalles.
            var viewModel = _mapper.Map<PresupuestoViewModel>(presupuesto);
            // Retorna la vista Details con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo presupuesto.
        /// Requiere que el usuario esté identificado (a través del método GetCurrentUserId).
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el formulario de creación.</returns>
        public async Task<IActionResult> Create()
        {
            // Aunque no tiene [Authorize], GetCurrentUserId() y las acciones POST subsiguientes requieren usuario.
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Challenge(); // Asegura que el usuario esté identificado para cargar las listas.
            }

            // Inicializa un nuevo ViewModel para el formulario de creación.
            var viewModel = new PresupuestoFormViewModel
            {
                // Obtiene y asigna las listas de categorías y monedas disponibles para los selectores.
                CategoriasDisponibles = await GetCategoriasSelectList(),
                MonedasDisponibles = await GetMonedasSelectList()
            };
            // Retorna la vista Create con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para crear un nuevo presupuesto.
        /// Requiere que el usuario esté identificado y validación de token anti-falsificación.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene los datos del nuevo presupuesto enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index si el presupuesto se crea exitosamente; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Create(PresupuestoFormViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Elimina del estado del modelo los errores relacionados con las listas de selectores,
            // ya que estos ViewModels son solo para la vista y no vienen del formulario POST.
            ModelState.Remove("CategoriasDisponibles");
            ModelState.Remove("MonedasDisponibles");

            // Verifica si el estado del modelo es válido según las validaciones definidas en el ViewModel.
            if (ModelState.IsValid)
            {
                // Mapea los datos validados del ViewModel a la entidad Presupuesto.
                var presupuesto = _mapper.Map<Presupuesto>(viewModel);
                // Asigna el ID del usuario actual a la entidad presupuesto antes de guardarla.
                presupuesto.UsuarioId = userId.Value;

                // Agrega el nuevo presupuesto a través del servicio de negocio.
                await _presupuestoService.AgregarAsync(presupuesto);

                // Establece un mensaje de éxito temporal para mostrar en la próxima solicitud (Index).
                TempData["SuccessMessage"] = "Presupuesto creado exitosamente.";
                // Redirecciona al usuario a la página de lista de presupuestos.
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
        /// Muestra el formulario precargado para editar un presupuesto existente.
        /// Requiere que el usuario esté identificado.
        /// </summary>
        /// <param name="id">El ID del presupuesto a editar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el formulario de edición o un resultado NotFound/Challenge.</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                // Retorna NotFound si el ID proporcionado es nulo.
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene el presupuesto por su ID desde el servicio.
            var presupuesto = await _presupuestoService.ObtenerPorIdAsync(id.Value);

            if (presupuesto == null || presupuesto.UsuarioId != userId.Value)
            {
                // Retorna NotFound si el presupuesto no existe o no pertenece al usuario actual.
                return NotFound();
            }

            // Mapea la entidad Presupuesto a un ViewModel de formulario para rellenar los campos de edición.
            var viewModel = _mapper.Map<PresupuestoFormViewModel>(presupuesto);
            // Obtiene y asigna las listas de categorías y monedas disponibles para los selectores.
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para actualizar un presupuesto existente.
        /// Requiere que el usuario esté identificado y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID del presupuesto que se espera actualizar (de la URL).</param>
        /// <param name="viewModel">ViewModel que contiene los datos actualizados del presupuesto enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index si el presupuesto se actualiza exitosamente; de lo contrario, regresa a la vista del formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Edit(int id, PresupuestoFormViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                // Retorna NotFound si el ID de la URL no coincide con el ID del ViewModel.
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene la entidad presupuesto original de la base de datos para verificar existencia y pertenencia.
            var originalPresupuesto = await _presupuestoService.ObtenerPorIdAsync(id);
            if (originalPresupuesto == null || originalPresupuesto.UsuarioId != userId.Value)
            {
                // Retorna NotFound si el presupuesto original no existe o no pertenece al usuario actual.
                return NotFound();
            }

            // Elimina del estado del modelo los errores relacionados con las listas de selectores.
            ModelState.Remove("CategoriasDisponibles");
            ModelState.Remove("MonedasDisponibles");

            // Verifica si el estado del modelo es válido.
            if (ModelState.IsValid)
            {
                try
                {
                    // Mapea los datos actualizados del ViewModel sobre la entidad original.
                    _mapper.Map(viewModel, originalPresupuesto);
                    // Asegura que el UsuarioId de la entidad original no sea sobrescrito por el mapeo.
                    originalPresupuesto.UsuarioId = userId.Value;

                    // Actualiza el presupuesto a través del servicio de negocio.
                    await _presupuestoService.ActualizarAsync(originalPresupuesto);

                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Presupuesto actualizado exitosamente.";
                    // Redirecciona a la acción Index.
                    return RedirectToAction(nameof(Index));
                }
                // Captura excepción de concurrencia si el registro fue modificado o eliminado por otro proceso.
                catch (DbUpdateConcurrencyException)
                {
                    // Verifica si el presupuesto aún existe para el usuario actual.
                    if (!await PresupuestoExists(viewModel.Id, userId.Value))
                    {
                        // Retorna NotFound si el presupuesto ya no existe (fue eliminado).
                        return NotFound();
                    }
                    else
                    {
                        // Relanza la excepción si el presupuesto existe pero hay un problema de concurrencia.
                        throw;
                    }
                }
                // Opcional: Considerar captura de otras excepciones genéricas o específicas de base de datos.
                // catch (Exception ex) { ... }
            }

            // Si el estado del modelo no es válido, recarga las listas y regresa a la vista Edit.
            viewModel.CategoriasDisponibles = await GetCategoriasSelectList();
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra la vista de confirmación para eliminar un presupuesto.
        /// Requiere que el usuario esté identificado.
        /// </summary>
        /// <param name="id">El ID del presupuesto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con los detalles del presupuesto para confirmar la eliminación o un resultado NotFound/Challenge.</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                // Retorna NotFound si el ID es nulo.
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene el presupuesto por su ID desde el servicio.
            var presupuesto = await _presupuestoService.ObtenerPorIdAsync(id.Value);

            if (presupuesto == null || presupuesto.UsuarioId != userId.Value)
            {
                // Retorna NotFound si el presupuesto no existe o no pertenece al usuario actual.
                return NotFound();
            }

            // Mapea la entidad Presupuesto a un ViewModel para mostrar detalles en la vista de confirmación.
            var viewModel = _mapper.Map<PresupuestoViewModel>(presupuesto);
            // Retorna la vista Delete con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de eliminación confirmada de un presupuesto.
        /// Requiere que el usuario esté identificado y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID del presupuesto a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index después de intentar eliminar el presupuesto.</returns>
        [HttpPost, ActionName("Delete")] // Designa este método como la acción POST para /Presupuestos/Delete.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                // Retorna un desafío de autenticación si el usuario no está identificado.
                return Challenge();
            }

            // Obtiene el presupuesto por su ID para verificar existencia y pertenencia antes de eliminar.
            var presupuesto = await _presupuestoService.ObtenerPorIdAsync(id);
            if (presupuesto == null || presupuesto.UsuarioId != userId.Value)
            {
                // Retorna NotFound si el presupuesto no existe o no pertenece al usuario actual.
                return NotFound();
            }

            try
            {
                // Elimina el presupuesto a través del servicio de negocio.
                await _presupuestoService.EliminarAsync(id);
                // Establece un mensaje de éxito temporal.
                TempData["SuccessMessage"] = "Presupuesto eliminado exitosamente.";
                // Redirecciona a la acción Index.
                return RedirectToAction(nameof(Index));
            }
            // Captura cualquier excepción durante la eliminación.
            catch (Exception)
            {
                // Establece un mensaje de error genérico temporal. Considerar loggear la excepción real.
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el presupuesto.";
                // Nota: Un manejo de errores más específico para restricciones de clave foránea podría ser útil aquí, similar a Categorias/Gastos.
                return RedirectToAction(nameof(Index)); // O redirigir a una vista de error
            }
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
        /// Verifica si un presupuesto con el ID especificado existe y pertenece al usuario dado.
        /// </summary>
        /// <param name="id">El ID del presupuesto a verificar.</param>
        /// <param name="userId">El ID del usuario al que debe pertenecer el presupuesto.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es True si el presupuesto existe y pertenece al usuario; de lo contrario, False.</returns>
        private async Task<bool> PresupuestoExists(int id, int userId)
        {
            // Obtiene el presupuesto por su ID desde el servicio.
            var presupuesto = await _presupuestoService.ObtenerPorIdAsync(id);
            // Retorna true si el presupuesto no es nulo y su UsuarioId coincide con el ID proporcionado.
            return presupuesto != null && presupuesto.UsuarioId == userId;
        }
    }
}