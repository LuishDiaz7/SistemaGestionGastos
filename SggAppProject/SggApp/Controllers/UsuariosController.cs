using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SggApp.BLL.Interfaces;
using SggApp.DAL.Entidades;
using SggApp.Models;

namespace SggApp.Web.Controllers
{
    /// <summary>
    /// Controlador MVC para la gestión de cuentas de usuario.
    /// Proporciona acciones para registro, inicio y cierre de sesión, gestión de perfil y cambio de contraseña.
    /// </summary>
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService; // Servicio para operaciones de negocio sobre la entidad Usuario (fuera de Identity core)
        private readonly IMonedaService _monedaService;
        private readonly UserManager<Usuario> _userManager; // Gestor de usuarios de ASP.NET Core Identity
        private readonly SignInManager<Usuario> _signInManager; // Gestor de inicio de sesión de ASP.NET Core Identity
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa una nueva instancia del controlador de usuarios.
        /// </summary>
        /// <param name="usuarioService">Servicio para operaciones de negocio relacionadas con la entidad Usuario.</param>
        /// <param name="monedaService">Servicio para operaciones de negocio relacionadas con monedas.</param>
        /// <param name="userManager">Gestor de usuarios de ASP.NET Core Identity.</param>
        /// <param name="signInManager">Gestor de inicio de sesión de ASP.NET Core Identity.</param>
        /// <param name="mapper">Instancia de AutoMapper para mapeo entre entidades y ViewModels.</param>
        public UsuariosController(
            IUsuarioService usuarioService,
            IMonedaService monedaService,
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IMapper mapper)
        {
            _usuarioService = usuarioService;
            _monedaService = monedaService;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Muestra el formulario de registro de nuevos usuarios.
        /// Permite acceso a usuarios no autenticados.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el formulario de registro.</returns>
        [AllowAnonymous] // Permite el acceso a usuarios anónimos.
        public async Task<IActionResult> Registro()
        {
            // Inicializa el ViewModel para el formulario de registro.
            var viewModel = new RegistroViewModel
            {
                // Obtiene y asigna las listas de monedas disponibles para el selector.
                MonedasDisponibles = await GetMonedasSelectList()
            };
            // Retorna la vista Registro con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para registrar un nuevo usuario.
        /// Permite acceso a usuarios no autenticados y requiere validación de token anti-falsificación.
        /// Utiliza ASP.NET Core Identity para la creación del usuario.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene los datos del nuevo usuario (nombre, email, contraseña, moneda) enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la página de inicio si el registro y login son exitosos; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [AllowAnonymous] // Permite el acceso a usuarios anónimos.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Registro(RegistroViewModel viewModel)
        {
            // Verifica si el estado del modelo es válido según las validaciones del ViewModel.
            if (ModelState.IsValid)
            {
                // Mapea los datos del ViewModel a la entidad Usuario.
                var usuario = _mapper.Map<Usuario>(viewModel);
                // Intenta crear el usuario utilizando el UserManager de Identity.
                var result = await _userManager.CreateAsync(usuario, viewModel.Password);

                // Verifica si la creación del usuario fue exitosa.
                if (result.Succeeded)
                {
                    // Si es exitoso, inicia sesión automáticamente para el nuevo usuario.
                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    // Establece un mensaje de éxito temporal.
                    TempData["SuccessMessage"] = "Cuenta creada exitosamente. ¡Bienvenido!";
                    // Redirecciona a la página principal (Home).
                    return RedirectToAction("Index", "Home");
                }

                // Si la creación del usuario falló, agrega los errores de Identity al ModelState.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si el estado del modelo no es válido o la creación/login falló,
            // recarga las listas de monedas y regresa a la vista de registro con los errores.
            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario de inicio de sesión.
        /// Permite acceso a usuarios no autenticados.
        /// </summary>
        /// <param name="returnUrl">La URL a la que redirigir después del inicio de sesión exitoso (opcional).</param>
        /// <returns>Una vista que presenta el formulario de inicio de sesión.</returns>
        [AllowAnonymous] // Permite el acceso a usuarios anónimos.
        public IActionResult Login(string returnUrl = null)
        {
            // Guarda la URL de retorno en ViewData para usarla después del login.
            ViewData["ReturnUrl"] = returnUrl;
            // Retorna la vista Login.
            return View();
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para iniciar sesión.
        /// Permite acceso a usuarios no autenticados y requiere validación de token anti-falsificación.
        /// Utiliza ASP.NET Core Identity para el inicio de sesión.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene las credenciales (email, password) y la opción "RememberMe" enviado desde el formulario.</param>
        /// <param name="returnUrl">La URL a la que redirigir después del inicio de sesión exitoso (opcional, del parámetro GET o POST).</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la URL de retorno o a la página de inicio si el login es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [AllowAnonymous] // Permite el acceso a usuarios anónimos.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Login(LoginViewModel viewModel, string returnUrl = null)
        {
            // Guarda la URL de retorno en ViewData.
            ViewData["ReturnUrl"] = returnUrl;

            // Verifica si el estado del modelo es válido según las validaciones del ViewModel.
            if (ModelState.IsValid)
            {
                // Intenta iniciar sesión utilizando el SignInManager de Identity con email y contraseña.
                var result = await _signInManager.PasswordSignInAsync(
                    viewModel.Email, // Identity usa UserName por defecto para login, pero se puede configurar Email. Asumiendo que Email se usa como UserName.
                    viewModel.Password,
                    viewModel.RememberMe,
                    lockoutOnFailure: false); // Deshabilita el bloqueo por intentos fallidos aquí, si no se configura.

                // Verifica si el inicio de sesión fue exitoso.
                if (result.Succeeded)
                {
                    // Si es exitoso, redirige a la URL de retorno si es válida y local, de lo contrario, a la página de inicio.
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Si el inicio de sesión falló, agrega un error genérico al ModelState.
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    // Regresa a la vista de login con el ViewModel actual (mostrando el error).
                    return View(viewModel);
                }
            }

            // Si el estado del modelo no es válido, regresa a la vista de login con el ViewModel actual (mostrando errores de validación).
            return View(viewModel);
        }

        /// <summary>
        /// Procesa la solicitud de cierre de sesión del usuario actual.
        /// Requiere que el usuario esté autenticado (aunque la acción POST no siempre lo enforcea estrictamente para SignOutAsync).
        /// Requiere validación de token anti-falsificación.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la página de inicio después del cierre de sesión.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Logout()
        {
            // Cierra la sesión del usuario actual utilizando el SignInManager de Identity.
            await _signInManager.SignOutAsync();
            // Redirecciona a la página principal (Home).
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Muestra el perfil del usuario autenticado.
        /// Requiere autenticación.
        /// </summary>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado es una vista con el perfil del usuario o un resultado Challenge/NotFound.</returns>
        [Authorize] // Requiere que el usuario esté autenticado.
        public async Task<IActionResult> Perfil()
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el ID de usuario no se puede obtener.
            if (userId == null) return Challenge();

            // Obtiene el usuario por su ID utilizando el servicio de negocio.
            var usuario = await _usuarioService.ObtenerPorIdAsync(userId.Value);
            // Retorna NotFound si el usuario no se encuentra.
            if (usuario == null) return NotFound();

            // Mapea la entidad Usuario a un ViewModel de perfil.
            var viewModel = _mapper.Map<PerfilViewModel>(usuario);
            // Obtiene y asigna las listas de monedas disponibles para el selector.
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            // Retorna la vista Perfil con el ViewModel.
            return View(viewModel);
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para actualizar el perfil del usuario autenticado.
        /// Requiere autenticación y validación de token anti-falsificación.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene los datos actualizados del perfil (nombre, moneda predeterminada) enviado desde el formulario.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Perfil si es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [Authorize] // Requiere que el usuario esté autenticado.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> Perfil(PerfilViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            // Retorna un desafío de autenticación si el ID de usuario no se puede obtener.
            if (userId == null) return Challenge();

            // Verifica si el estado del modelo es válido.
            if (ModelState.IsValid)
            {
                // Obtiene la entidad usuario actual de la base de datos para actualizar.
                var usuario = await _usuarioService.ObtenerPorIdAsync(userId.Value);
                // Retorna NotFound si el usuario no se encuentra (situación inesperada si está autenticado).
                if (usuario == null) return NotFound();

                // Actualiza las propiedades del usuario con los datos del ViewModel.
                usuario.Name = viewModel.Nombre; // Asumiendo que la entidad Usuario tiene una propiedad 'Name'
                usuario.MonedaPredeterminadaId = viewModel.MonedaPredeterminadaId; // Asumiendo que la entidad Usuario tiene esta propiedad

                // Actualiza el usuario a través del servicio de negocio.
                await _usuarioService.ActualizarAsync(usuario);
                // Establece un mensaje de éxito temporal.
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                // Redirecciona a la acción Perfil para mostrar el perfil actualizado.
                return RedirectToAction(nameof(Perfil));
            }

            // Si el estado del modelo no es válido, recarga las listas de monedas y regresa a la vista Perfil.
            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        /// <summary>
        /// Muestra el formulario para cambiar la contraseña del usuario autenticado.
        /// Requiere autenticación.
        /// </summary>
        /// <returns>Una vista que presenta el formulario de cambio de contraseña.</returns>
        [Authorize] // Requiere que el usuario esté autenticado.
        public IActionResult CambiarPassword()
        {
            // Retorna la vista CambiarPassword.
            return View();
        }

        /// <summary>
        /// Procesa los datos enviados desde el formulario para cambiar la contraseña del usuario autenticado.
        /// Requiere autenticación y validación de token anti-falsificación.
        /// Utiliza ASP.NET Core Identity para el cambio de contraseña.
        /// </summary>
        /// <param name="viewModel">ViewModel que contiene la contraseña actual y la nueva contraseña.</param>
        /// <returns>Una tarea que representa la operación asíncrona. Redirecciona a la acción Perfil si el cambio es exitoso; de lo contrario, regresa al formulario con errores.</returns>
        [HttpPost]
        [Authorize] // Requiere que el usuario esté autenticado.
        [ValidateAntiForgeryToken] // Valida el token para prevenir ataques CSRF.
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel viewModel)
        {
            // Verifica si el estado del modelo es válido.
            if (!ModelState.IsValid)
            {
                // Si no es válido, regresa a la vista con el ViewModel actual (mostrando errores de validación).
                return View(viewModel);
            }

            // Obtiene la entidad de usuario actual utilizando el UserManager.
            var user = await _userManager.GetUserAsync(User);
            // Retorna NotFound si el usuario no se encuentra (situación inesperada si está autenticado).
            if (user == null)
            {
                return NotFound();
            }

            // Intenta cambiar la contraseña utilizando el UserManager, verificando la contraseña actual.
            var result = await _userManager.ChangePasswordAsync(user, viewModel.PasswordActual, viewModel.NuevoPassword);

            // Verifica si el cambio de contraseña fue exitoso.
            if (!result.Succeeded)
            {
                // Si falló, agrega los errores de Identity al ModelState.
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                // Regresa a la vista con el ViewModel (mostrando los errores).
                return View(viewModel);
            }

            // Si fue exitoso, refresca la cookie de autenticación para reflejar cualquier posible cambio relacionado con la seguridad.
            await _signInManager.RefreshSignInAsync(user);
            // Establece un mensaje de éxito temporal.
            TempData["SuccessMessage"] = "Tu contraseña ha sido cambiada exitosamente.";
            // Redirecciona a la acción Perfil.
            return RedirectToAction(nameof(Perfil));
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
    }
}