using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.BLL.Servicios;
using SggApp.DAL;
using SggApp.DAL.Data;
using SggApp.DAL.Entidades;
using SggApp.DAL.Repositorios;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SggApp.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

// Crea el constructor de la aplicación web
var builder = WebApplication.CreateBuilder(args);

// --- Configuración de Servicios (builder.Services) ---
// Esta sección configura los servicios que la aplicación utilizará e inyectará (IoC Container).

// Agrega servicios para MVC con soporte para controladores y vistas.
builder.Services.AddControllersWithViews();

// Configura el DbContext de Entity Framework Core para conectarse a SQL Server.
// La cadena de conexión se lee desde el archivo de configuración (appsettings.json).
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registra la implementación de UnitOfWork como un servicio con ámbito de petición.
builder.Services.AddScoped<UnitOfWork>();

// Configura e integra ASP.NET Core Identity para la gestión de usuarios.
// Se utiliza la entidad de usuario personalizada 'Usuario' y el rol 'IdentityRole<int>'.
// Se especifica que la gestión de datos de Identity se realice a través de Entity Framework usando ApplicationDbContext.
builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    // Configuración de requisitos de contraseña para mayor seguridad.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    // Configuración de opciones de inicio de sesión (ej. no requerir email confirmado para iniciar sesión).
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Configura el almacenamiento de Identity usando EF Core y ApplicationDbContext.
.AddDefaultTokenProviders(); // Agrega proveedores de tokens predeterminados (para resetear contraseña, etc.).

// Configura las cookies de autenticación.
// Se definen rutas personalizadas para login y acceso denegado, nombre de la cookie, y tiempo de expiración.
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; // Define la ruta de la página de inicio de sesión.
    options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // Define la ruta para acceso denegado.
    options.Cookie.Name = "SggAppAuthCookie"; // Establece el nombre de la cookie de autenticación.
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Define la duración de la sesión de la cookie.
    options.SlidingExpiration = true; // Permite que la cookie se refresque en cada petición.
});

// Registra los repositorios específicos de la capa DAL como servicios con ámbito de petición.
// Esto permite que sean inyectados en los servicios de la BLL.
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<GastoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<MonedaRepository>();
builder.Services.AddScoped<PresupuestoRepository>();
builder.Services.AddScoped<TipoCambioRepository>();

// Registra las implementaciones de los servicios de la capa BLL como servicios con ámbito de petición.
// Esto permite que las interfaces de servicio sean inyectadas en los controladores u otros servicios.
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<ITipoCambioService, TipoCambioService>();

// Agrega servicios para Razor Pages.
builder.Services.AddRazorPages();

// Registra un servicio de envío de correo electrónico como transitorio (se crea una nueva instancia por cada solicitud).
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Configura AutoMapper para el mapeo automático entre entidades y ViewModels.
// Se busca el perfil de mapeo (MappingProfile) en el ensamblado actual.
builder.Services.AddAutoMapper(typeof(SggApp.MappingProfiles.MappingProfile).Assembly);

// --- Configuración de Pipeline de Solicitudes HTTP (app.Use...) ---
// Esta sección define la secuencia de middlewares que procesarán cada solicitud HTTP.

// Construye la aplicación web.
var app = builder.Build();

// Configura el pipeline de solicitudes HTTP para el entorno de desarrollo.
if (app.Environment.IsDevelopment())
{
    // Muestra una página de excepciones detallada en caso de errores.
    app.UseDeveloperExceptionPage();
    // Habilita middleware para servir la especificación Swagger generada.
    //app.UseSwagger();
    // Habilita middleware para servir la interfaz de usuario de Swagger (Swagger UI).
    //app.UseSwaggerUI();
}
// Configura el pipeline de solicitudes HTTP para entornos que no son de desarrollo (producción, staging, etc.).
else
{
    // Configura un manejador de excepciones para redirigir a una página de error genérica.
    app.UseExceptionHandler("/Home/Error");
    // Agrega el middleware HSTS (HTTP Strict Transport Security) para forzar conexiones HTTPS.
    app.UseHsts();
}

// Agrega el middleware para redirigir solicitudes HTTP a HTTPS.
app.UseHttpsRedirection();

// Habilita el middleware para servir archivos estáticos desde wwwroot (CSS, JS, imágenes).
app.UseStaticFiles();

// Agrega el middleware de enrutamiento para que la aplicación pueda hacer coincidir URLs con endpoints.
// Este middleware debe ir antes de UseAuthentication y UseAuthorization.
app.UseRouting();

// Agrega el middleware de autenticación, necesario para identificar al usuario actual.
// Debe ir después de UseRouting y antes de UseAuthorization.
app.UseAuthentication();

// Agrega el middleware de autorización, que verifica si el usuario actual tiene permiso para acceder a un recurso.
// Debe ir después de UseAuthentication.
app.UseAuthorization();

// Mapea los endpoints para las Razor Pages.
app.MapRazorPages();

// Mapea los endpoints para los controladores MVC usando el patrón de ruta predeterminado.
// Define la estructura URL básica de la aplicación ({controlador}/{acción}/{id?}).
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Inicia la aplicación web.
app.Run();