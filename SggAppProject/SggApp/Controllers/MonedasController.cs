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
    /// <summary>
    /// Controlador MVC para la gestión de monedas.
    /// Permite administrar la información de las diferentes monedas utilizadas en la aplicación.
    /// Las acciones de creación, edición y eliminación están restringidas a usuarios con el rol "Admin".
    /// </summary>
    [Authorize(Roles = "Admin")] // Requiere que el usuario tenga el rol "Admin" para la mayoría de las acciones.
    public class MonedasController : Controller
    {
        private readonly IMonedaService _monedaService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de monedas.
        /// </summary>
        /// <param name="monedaService">Servicio para operaciones de negocio relacionadas con monedas.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
        public MonedasController(
            IMonedaService monedaService,
            IMapper mapper)
        {
            _monedaService = monedaService;
            _mapper = mapper;
        }

        /// <summary>
        /// Muestra la lista de todas las monedas disponibles.
        /// Esta acción permite acceso a usuarios no autenticados y no requiere un rol específico.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista que presenta un listado de monedas.</returns>
        [AllowAnonymous] // Permite el acceso a usuarios anónimos (anula la restricción de [Authorize] a nivel de controlador).
        public async Task<IActionResult> Index()
        {
            // Obtiene todas las monedas disponibles desde el servicio de negocio.
            var monedas = await _monedaService.ObtenerTodasAsync();
            // Mapea la colección de entidades Moneda a una colección de ViewModels para la vista.
            var viewModels = _mapper.Map<IEnumerable<MonedaViewModel>>(monedas);

            // Retorna la vista Index con la lista de ViewModels.
            return View(viewModels);
        }

        /// <summary>
        /// Muestra los detalles de una moneda específica.
        /// Esta acción permite acceso a usuarios no autenticados y no requiere un rol específico.
        /// </summary>
        /// <param name="id">El ID de la moneda a mostrar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista que presenta los detalles de la moneda o un resultado NotFound.</returns>
        [AllowAnonymous] // Permite el acceso a usuarios anónimos.
        public async Task<IActionResult> Details(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            // Obtiene la moneda por su ID desde el servicio.
            var moneda = await _monedaService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si la moneda no existe.
            if (moneda == null) return NotFound();

            // Mapea la entidad Moneda a un ViewModel para la vista de detalles.
            var viewModel = _mapper.Map<MonedaViewModel>(moneda);
            // Retorna la vista Details con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para crear una nueva moneda.
        /// Requiere que el usuario tenga el rol "Admin".
        /// </summary>
        /// <returns>Una vista que presenta el formulario de creación de moneda.</returns>
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public IActionResult Create()
        {
            // Retorna la vista Create con un ViewModel inicializado (Activa=true por defecto).
            return View(new MonedaViewModel { Activa = true });
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para crear una nueva moneda.
        /// Requiere que el usuario tenga el rol "Admin" y validación de token anti-falsificación.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene los datos de la nueva moneda enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Create(MonedaViewModel viewModel)
        {
            // Verifica si el estado del modelo es válido según las validaciones del ViewModel.
            if (ModelState.IsValid)
            {
                // Mapea el ViewModel a la entidad Moneda.
                var moneda = _mapper.Map<Moneda>(viewModel);

                try
                {
                    // Agrega la nueva moneda a través del servicio.
                    await _monedaService.AgregarAsync(moneda);
                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Moneda creada exitosamente.";
                    // Redirecciona a la acción Index.
                    return RedirectToAction(nameof(Index));
                }
                // Captura excepción si el código de moneda ya existe (violación de UNIQUE).
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") ?? false)
                {
                    // Agrega un error al ModelState para mostrar en la vista.
                    ModelState.AddModelError("Codigo", "Ya existe una moneda con este código.");
                }
                // Captura otras posibles excepciones durante el guardado.
                catch (Exception)
                {
                    // Agrega un error general al ModelState.
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al crear la moneda.");
                }
            }

            // Si el ModelState no es válido o hubo un error al guardar, regresa a la vista Create con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario precargado para editar una moneda existente.
        /// Requiere que el usuario tenga el rol "Admin".
        /// </summary>
        /// <param name="id">El ID de la moneda a editar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el formulario de edición o un resultado NotFound.</returns>
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Edit(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            // Obtiene la moneda por su ID desde el servicio.
            var moneda = await _monedaService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si la moneda no existe.
            if (moneda == null) return NotFound();

            // Mapea la entidad Moneda a un ViewModel para rellenar el formulario de edición.
            var viewModel = _mapper.Map<MonedaViewModel>(moneda);
            // Retorna la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para actualizar una moneda existente.
        /// Requiere que el usuario tenga el rol "Admin" y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID de la moneda que se espera actualizar (de la URL).</param>
        /// <param name="viewModel">ViewModel que contiene los datos actualizados de la moneda enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Edit(int id, MonedaViewModel viewModel)
        {
            // Retorna NotFound si el ID de la URL no coincide con el ID del ViewModel.
            if (id != viewModel.Id) return NotFound();

            // Verifica si el estado del modelo es válido.
            if (ModelState.IsValid)
            {
                try
                {
                    // Mapea los datos del ViewModel a una nueva entidad Moneda (o sobre una existente si se cargara primero).
                    var moneda = _mapper.Map<Moneda>(viewModel);
                    // Actualiza la moneda a través del servicio de negocio.
                    await _monedaService.ActualizarAsync(moneda);

                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Moneda actualizada exitosamente.";
                    // Redirecciona a la acción Index.
                    return RedirectToAction(nameof(Index));
                }
                // Captura excepción de concurrencia si el registro fue modificado o eliminado por otro proceso.
                catch (DbUpdateConcurrencyException)
                {
                    // Verifica si la moneda aún existe.
                    if (!await MonedaExists(viewModel.Id))
                        // Retorna NotFound si la moneda ya no existe (fue eliminada).
                        return NotFound();
                    else
                        // Relanza la excepción si la moneda existe pero hay un problema de concurrencia inesperado.
                        throw;
                }
                // Captura excepción si el nuevo código de moneda ya existe.
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") ?? false)
                {
                    // Agrega un error al ModelState.
                    ModelState.AddModelError("Codigo", "Ya existe una moneda con este código.");
                }
                // Captura otras posibles excepciones durante el guardado.
                catch (Exception)
                {
                    // Agrega un error general al ModelState.
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error al actualizar la moneda.");
                }
            }

            // Si el ModelState no es válido o hubo un error al guardar, regresa a la vista Edit con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Muestra la vista de confirmación para eliminar una moneda.
        /// Requiere que el usuario tenga el rol "Admin".
        /// </summary>
        /// <param name="id">El ID de la moneda a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con los detalles de la moneda para confirmar la eliminación o un resultado NotFound.</returns>
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> Delete(int? id)
        {
            // Retorna NotFound si el ID proporcionado es nulo.
            if (id == null) return NotFound();

            // Obtiene la moneda por su ID desde el servicio.
            var moneda = await _monedaService.ObtenerPorIdAsync(id.Value);
            // Retorna NotFound si la moneda no existe.
            if (moneda == null) return NotFound();

            // Mapea la entidad Moneda a un ViewModel para mostrar detalles en la vista de confirmación.
            var viewModel = _mapper.Map<MonedaViewModel>(moneda);
            // Retorna la vista Delete con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de eliminación confirmada de una moneda.
        /// Requiere que el usuario tenga el rol "Admin" y validación de token anti-falsificación.
        /// </summary>
        /// <param name="id">El ID de la moneda a eliminar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Index después de intentar eliminar la moneda.</returns>
        [HttpPost, ActionName("Delete")] // Designa este método como la acción POST para /Monedas/Delete.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        // [Authorize(Roles = "Admin")] // Ya implícito por el atributo a nivel de controlador.
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Intenta eliminar la moneda a través del servicio de negocio.
                await _monedaService.EliminarAsync(id);
                // Establece un mensaje de éxito temporal si la eliminación fue exitosa.
                TempData["SuccessMessage"] = "Moneda eliminada exitosamente.";
            }
            // Captura excepción si la moneda no se puede eliminar debido a restricciones de clave foránea (está en uso).
            catch (DbUpdateException)
            {
                // Establece un mensaje de error temporal indicando la restricción.
                TempData["ErrorMessage"] = "No se puede eliminar esta moneda porque está en uso por usuarios, gastos o presupuestos.";
            }
            // Opcional: Considerar capturar otras excepciones generales y registrarlas.
            // catch (Exception ex) { ... }

            // Redirecciona a la acción Index después del intento de eliminación.
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica si una moneda con el ID especificado existe.
        /// </summary>
        /// <param name="id">El ID de la moneda a verificar.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es True si la moneda existe; de lo contrario, False.</returns>
        private async Task<bool> MonedaExists(int id)
        {
            // Obtiene la moneda por su ID desde el servicio.
            var moneda = await _monedaService.ObtenerPorIdAsync(id);
            // Retorna true si la moneda obtenida no es nula.
            return moneda != null;
        }
    }
}