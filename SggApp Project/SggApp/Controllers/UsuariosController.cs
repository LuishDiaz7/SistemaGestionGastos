using System;
using System.Collections.Generic;
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
    public class UsuariosController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMonedaService _monedaService;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IMapper _mapper;

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

        // GET: /Usuarios/Registro
        [AllowAnonymous]
        public async Task<IActionResult> Registro()
        {
            var viewModel = new RegistroViewModel
            {
                MonedasDisponibles = await GetMonedasSelectList()
            };
            return View(viewModel);
        }

        // POST: /Usuarios/Registro
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = _mapper.Map<Usuario>(viewModel);
                var result = await _userManager.CreateAsync(usuario, viewModel.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(usuario, isPersistent: false);
                    TempData["SuccessMessage"] = "Cuenta creada exitosamente. ¡Bienvenido!";
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si llegamos aquí, algo falló, redisplay form
            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        // GET: /Usuarios/Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Usuarios/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    viewModel.Email,
                    viewModel.Password,
                    viewModel.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
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
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    return View(viewModel);
                }
            }

            // Si llegamos aquí, algo falló, redisplay form
            return View(viewModel);
        }

        // POST: /Usuarios/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Usuarios/Perfil
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            var usuario = await _usuarioService.ObtenerPorIdAsync(userId.Value);
            if (usuario == null) return NotFound();

            var viewModel = _mapper.Map<PerfilViewModel>(usuario);
            viewModel.MonedasDisponibles = await GetMonedasSelectList();

            return View(viewModel);
        }

        // POST: /Usuarios/Perfil
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(PerfilViewModel viewModel)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Challenge();

            if (ModelState.IsValid)
            {
                var usuario = await _usuarioService.ObtenerPorIdAsync(userId.Value);
                if (usuario == null) return NotFound();

                usuario.Name = viewModel.Nombre;
                usuario.MonedaPredeterminadaId = viewModel.MonedaPredeterminadaId;

                await _usuarioService.ActualizarAsync(usuario);
                TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                return RedirectToAction(nameof(Perfil));
            }

            viewModel.MonedasDisponibles = await GetMonedasSelectList();
            return View(viewModel);
        }

        // GET: /Usuarios/CambiarPassword
        [Authorize]
        public IActionResult CambiarPassword()
        {
            return View();
        }

        // POST: /Usuarios/CambiarPassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, viewModel.PasswordActual, viewModel.NuevoPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(viewModel);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["SuccessMessage"] = "Tu contraseña ha sido cambiada exitosamente.";
            return RedirectToAction(nameof(Perfil));
        }

        // --- Helper Methods ---
        private int? GetCurrentUserId()
        {
            var userIdString = _userManager.GetUserId(User);
            if (int.TryParse(userIdString, out int userId)) return userId;
            return null;
        }

        private async Task<IEnumerable<SelectListItem>> GetMonedasSelectList()
        {
            var monedas = await _monedaService.ObtenerTodasAsync();
            return monedas.Select(m => new SelectListItem { Value = m.Id.ToString(), Text = $"{m.Nombre} ({m.Codigo})" });
        }
    }
}