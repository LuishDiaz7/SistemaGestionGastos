using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// Controlador MVC para la gestión de tipos de cambio entre monedas.
    /// Permite administrar las tasas de conversión utilizadas en la aplicación.
    /// Las acciones de creación, edición y eliminación están restringidas a usuarios con el rol "Admin",
    /// mientras que la visualización (lista y detalles) está permitida a todos los usuarios.
    /// </summary>
    [Authorize(Roles = "Admin")] // Requiere que el usuario tenga el rol "Admin" para la mayoría de las acciones.
    public class TiposCambioController : Controller
    {
        private readonly ITipoCambioService _tipoCambioService;
        private readonly IMonedaService _monedaService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de tipos de cambio.
        /// </summary>
        /// <param name="tipoCambioService">Servicio para operaciones de negocio relacionadas con tipos de cambio.</param>
        /// <param name="monedaService">Servicio para operaciones de negocio relacionadas con monedas.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
        public TiposCambioController(
            ITipoCambioService tipoCambioService,
            IMonedaService monedaService,
            IMapper mapper)
        {
            _tipoCambioService = tipoCambioService;
            _monedaService = monedaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Muestra la lista de todos los tipos de cambio registrados.
        /// Esta acción permite acceso a usuarios no autenticados y no requiere un rol específico.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista que presenta un listado de tipos de cambio.</returns>
        [AllowAnonymous] // Permite el acceso a usuarios anónimos (anula la restricción de [Authorize] a nivel de controlador).
        public async Task<IActionResult> Index()
        {
            // Obtiene todos los tipos de cambio disponibles desde el servicio de negocio.
            var tiposCambio = await _tipoCambioService.ObtenerTodosAsync();
            // Mapea la colección de entidades TipoCambio a una colección de ViewModels para la vista.
            var viewModels = _mapper.Map<IEnumerable<TipoCambioViewModel>>(tiposCambio);

            // Retorna la vista Index con la lista de ViewModels.
            return View(viewModels);
        }

        /// <summary>
        /// Muestra los detalles de un tipo de cambio específico.
        /// Esta acción permite acceso a usuarios no autenticados y no requiere un rol específico.
        /// </summary>
        /// <param name="id">El ID del tipo de cambio a mostrar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista que presenta los detalles del tipo de cambio o un resultado NotFound.</returns>
        [AllowAnonymous] // Permite el acceso a usuarios anónimos.
        public async Task<IActionResult> Details(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            // Obtiene el tipo de cambio por su ID desde el servicio.
            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si el tipo de cambio no existe.
            if (tipoCambio == null) return NotFound();

            // Mapea la entidad TipoCambio a un ViewModel para la vista de detalles.
            var viewModel = _mapper.Map<TipoCambioViewModel>(tipoCambio);
            // Retorna la vista Details con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para crear un nuevo tipo de cambio.
        /// Requiere que el usuario tenga el rol "Admin".
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el formulario de creación de tipo de cambio.</returns>
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Create()
        {
            // Inicializa un nuevo ViewModel para el formulario de creación, estableciendo la fecha actual
            // y obteniendo las listas de monedas disponibles para los selectores.
            var viewModel = new TipoCambioFormViewModel
            {
                FechaActualizacion = DateTime.Now,
                MonedasDisponibles = await GetMonedasSelectList()
            };
            // Retorna la vista Create con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para crear un nuevo tipo de cambio.
        /// Requiere que el usuario tenga el rol "Admin" y validación de token anti-falsificación.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene los datos del nuevo tipo de cambio enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Create(TipoCambioFormViewModel viewModel)
        {
            // Verifica si el estado del modelo es válido según las validaciones del ViewModel.
            if (ModelState.IsValid)
            {
                // Valida que las monedas de origen y destino sean diferentes.
                if (viewModel.MonedaOrigenId == viewModel.MonedaDestinoId)
                {
                    // Agrega un error al ModelState si las monedas son iguales.
                    ModelState.AddModelError("MonedaDestinoId", "La moneda de destino debe ser diferente a la moneda de origen.");
                }
                else
                {
                    // Mapea el ViewModel a la entidad TipoCambio.
                    var tipoCambio = _mapper.Map<TipoCambio>(viewModel);
                    // Establece explícitamente la fecha de actualización al momento de la creación.
                    tipoCambio.FechaActualizacion = DateTime.Now;

                    try
                    {
                        // Agrega el nuevo tipo de cambio a través del servicio.
                        await _tipoCambioService.AgregarAsync(tipoCambio);
                        // Establece un mensaje de éxito temporal.
                        TempData["SuccessMessage"] = "Tipo de cambio creado exitosamente.";
                        // Redirecciona a la acción Index.
                        return RedirectToAction(nameof(Index));
                    }
                    // Captura excepción si ya existe un tipo de cambio con este par de monedas (violación de UQ_TiposCambio).
                    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_TiposCambio") ?? false)
                    {
                        // Agrega un error general al ModelState indicando la duplicación.
                        ModelState.AddModelError(string.Empty, "Ya existe un tipo de cambio para este par de monedas.");
                    }
                    // Captura otras posibles excepciones durante el guardado.
                    catch (Exception)
                    {
                        // Agrega un error general al ModelState indicando un error al crear.
                        ModelState.AddModelError(string.Empty, "Ha ocurrido un error al crear el tipo de cambio.");
                    }
                }
            }

            // Si el estado del modelo no es válido o hubo un error al guardar/validar,
            // se recargan las listas de selectores y se regresa a la vista Create.
            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario precargado para editar un tipo de cambio existente.
        /// Requiere que el usuario tenga el rol "Admin".
        /// </summary>
        /// <param name="id">El ID del tipo de cambio a editar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el formulario de edición o un resultado NotFound.</returns>
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Edit(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            // Obtiene el tipo de cambio por su ID desde el servicio.
            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si el tipo de cambio no existe.
            if (tipoCambio == null) return NotFound();

            // Mapea la entidad TipoCambio a un ViewModel de formulario para rellenar los campos de edición.
            var viewModel = _mapper.Map<TipoCambioFormViewModel>(tipoCambio);
            // Obtiene y asigna las listas de monedas disponibles para los selectores.
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para actualizar un tipo de cambio existente.
        /// Requiere que el usuario tenga el rol "Admin" y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID del tipo de cambio que se espera actualizar (de la URL).</param>
        /// <param name="viewModel">ViewModel que contiene los datos actualizados del tipo de cambio enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Edit(int id, TipoCambioFormViewModel viewModel)
        {
            // Retorna NotFound si el ID de la URL no coincide con el ID del ViewModel.
            if (id != viewModel.Id) return NotFound();

            // Verifica si el estado del modelo es válido.
            if (ModelState.IsValid)
            {
                // Valida que las monedas de origen y destino sean diferentes.
                if (viewModel.MonedaOrigenId == viewModel.MonedaDestinoId)
                {
                    // Agrega un error al ModelState si las monedas son iguales.
                    ModelState.AddModelError("MonedaDestinoId", "La moneda de destino debe ser diferente a la moneda de origen.");
                }
                else
                {
                    try
                    {
                        // Obtiene la entidad original del tipo de cambio de la base de datos para verificar existencia.
                        var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id);
                        // Retorna NotFound si el tipo de cambio original no existe.
                        if (tipoCambio == null) return NotFound();

                        // Mapea los datos actualizados del ViewModel sobre la entidad original.
                        _mapper.Map(viewModel, tipoCambio);
                        // Actualiza la fecha de actualización al momento de la edición.
                        tipoCambio.FechaActualizacion = DateTime.Now;

                        // Actualiza el tipo de cambio a través del servicio de negocio.
                        await _tipoCambioService.ActualizarAsync(tipoCambio);

                        // Establece un mensaje de éxito temporal.
                        TempData["SuccessMessage"] = "Tipo de cambio actualizado exitosamente.";
                        // Redirecciona a la acción Index.
                        return RedirectToAction(nameof(Index));
                    }
                    // Captura excepción de concurrencia si el registro fue modificado o eliminado por otro proceso.
                    catch (DbUpdateConcurrencyException)
                    {
                        // Verifica si el tipo de cambio aún existe.
                        if (!await TipoCambioExists(viewModel.Id))
                            // Retorna NotFound si el tipo de cambio ya no existe.
                            return NotFound();
                        else
                            // Relanza la excepción si existe pero hay un problema de concurrencia.
                            throw;
                    }
                    // Captura excepción si el nuevo par de monedas ya existe.
                    catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UQ_TiposCambio") ?? false)
                    {
                        // Agrega un error general al ModelState indicando la duplicación.
                        ModelState.AddModelError(string.Empty, "Ya existe un tipo de cambio para este par de monedas.");
                    }
                    // Captura otras posibles excepciones durante el guardado.
                    catch (Exception)
                    {
                        // Agrega un error general al ModelState indicando un error al actualizar.
                        ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar el tipo de cambio.");
                    }
                }
            }

            // Si el estado del modelo no es válido o hubo un error, recarga las listas y regresa a la vista Edit.
            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        /// <summary>
        /// Muestra la vista de confirmación para eliminar un tipo de cambio.
        /// Requiere que el usuario tenga el rol "Admin".
        /// </summary>
        /// <param name="id">El ID del tipo de cambio a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con los detalles para confirmar la eliminación o un resultado NotFound.</returns>
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Delete(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            // Obtiene el tipo de cambio por su ID desde el servicio.
            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si el tipo de cambio no existe.
            if (tipoCambio == null) return NotFound();

            // Mapea la entidad TipoCambio a un ViewModel para mostrar detalles en la vista de confirmación.
            var viewModel = _mapper.Map<TipoCambioViewModel>(tipoCambio);
            // Retorna la vista Delete con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de eliminación confirmada de un tipo de cambio.
        /// Requiere que el usuario tenga el rol "Admin" y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID del tipo de cambio a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index después de intentar eliminar.</returns>
        [HttpPost, ActionName("Delete")] // Designa este método como la acción POST para /TiposCambio/Delete.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Intenta eliminar el tipo de cambio a través del servicio de negocio.
                await _tipoCambioService.EliminarAsync(id);
                // Establece un mensaje de éxito temporal si la eliminación fue exitosa.
                TempData["SuccessMessage"] = "Tipo de cambio eliminado exitosamente.";
            }
            // Captura cualquier excepción durante la eliminación.
            catch (Exception)
            {
                // Establece un mensaje de error genérico temporal. Considerar loggear la excepción real.
                TempData["ErrorMessage"] = "Ha ocurrido un error al eliminar el tipo de cambio.";
                // Nota: Similar a otras entidades, podría haber una excepción de restricción de clave foránea si está en uso.
            }

            // Redirecciona a la acción Index después del intento de eliminación.
            return RedirectToAction(nameof(Index));
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
            return monedas.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = $"{m.Nombre} ({m.Codigo})" });
        }

        /// <summary>
        /// Verifica si un tipo de cambio con el ID especificado existe.
        /// </summary>
        /// <param name="id">El ID del tipo de cambio a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es True si el tipo de cambio existe; de lo contrario, False.</returns>
        private async Task<bool> TipoCambioExists(int id)
        {
            // Obtiene el tipo de cambio por su ID desde el servicio.
            var tipoCambio = await _tipoCambioService.ObtenerPorIdAsync(id);
            // Retorna true si el tipo de cambio obtenido no es nulo.
            return tipoCambio != null;
        }
    }
}