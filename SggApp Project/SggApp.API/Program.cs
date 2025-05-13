using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.BLL.Servicios;
using SggApp.DAL; // Asegúrate de que UnitOfWork esté en este namespace
using SggApp.DAL.Data; // Asegúrate de que ApplicationDbContext esté en este namespace
using SggApp.DAL.Repositorios; // Asegúrate de que tus repositorios estén en este namespace
using System.Reflection; // Necesario para AutoMapper si lo registraste con typeof().Assembly
// using SggApp.MappingProfiles; // Descomentar si registraste AutoMapper con typeof(TuPerfil).Assembly
// using SggApp.Models; // Descomentar si tus ViewModels están aquí
// using SggApp.Web.Models; // Descomentar si tus ViewModels están aquí (más común para proyectos Web)
using AutoMapper; // Necesario para AutoMapper

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACIÓN DE SERVICIOS (builder.Services) ---

// 1. Agregar servicios para MVC con soporte para vistas
builder.Services.AddControllersWithViews();

// Agregar DbContext con la cadena de conexión desde appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro del UnitOfWork como servicio con ámbito
builder.Services.AddScoped<UnitOfWork>(); // Asegúrate de que UnitOfWork esté correctamente definido e implementado

// Registro de Identity (SI USAS Identity, basado en tu código anterior)
// builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
// {
//     options.Password.RequireDigit = true;
//     options.Password.RequireLowercase = true;
//     options.Password.RequireUppercase = true;
//     options.Password.RequireNonAlphanumeric = true;
//     options.Password.RequiredLength = 8;
// })
// .AddEntityFrameworkStores<ApplicationDbContext>()
// .AddDefaultTokenProviders();

// Configurar cookies de autenticación (SI USAS Identity)
// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.LoginPath = "/Usuarios/Login";
//     options.AccessDeniedPath = "/Usuarios/AccessDenied";
// });

// Registro de repositorios (Asegúrate que estos repositorios estén en SggApp.DAL.Repositorios y correctos)
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<GastoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<MonedaRepository>();
builder.Services.AddScoped<PresupuestoRepository>();
builder.Services.AddScoped<TipoCambioRepository>();

// Registro de servicios (Asegúrate que estas interfaces/clases estén en SggApp.BLL.Interfaces/Servicios)
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<ITipoCambioService, TipoCambioService>();

// Configurar AutoMapper (Asegúrate de que tu MappingProfile.cs esté en SggApp.MappingProfiles)
// Asegúrate de que este using SggApp.MappingProfiles; esté presente si usaste typeof(MappingProfile).Assembly
// builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly); // Descomentar si usas esta forma de registro

// Configuración para Minimal APIs / OpenAPI (SI LO NECESITAS, si no, puedes eliminarlo)
builder.Services.AddEndpointsApiExplorer(); // Necesario para OpenAPI
builder.Services.AddSwaggerGen(); // Necesario para Swagger UI


var app = builder.Build();

// --- CONFIGURACIÓN DEL PIPELINE DE SOLICITUDES HTTP (app.Use...) ---

// Configurar el pipeline para desarrollo (página de excepciones detallada)
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi(); // Mapea los endpoints de OpenAPI/Swagger en desarrollo (si los usas)
    app.UseDeveloperExceptionPage(); // Muestra errores detallados en desarrollo
    app.UseSwagger(); // Habilita Swagger UI en desarrollo (si lo usas)
    app.UseSwaggerUI(); // Habilita Swagger UI en desarrollo (si lo usas)
}
else
{
    // Configuración para producción
    app.UseExceptionHandler("/Home/Error"); // Redirige a una página de error en producción
    // El valor predeterminado HSTS es 30 días. Puedes que desee cambiar esto para escenarios de producción.
    app.UseHsts(); // Habilita HSTS
}

// Middleware de redirección HTTPS
app.UseHttpsRedirection();

// Middleware para servir archivos estáticos (CSS, JS, imágenes, etc.)
app.UseStaticFiles();

// 2. Middleware de enrutamiento - ¡CRUCIAL PARA MVC!
app.UseRouting(); // Debe ir antes de UseAuthorization y MapControllerRoute/MapEndpoints

// Middleware de autenticación (SI USAS Identity) - Debe ir entre UseRouting y UseAuthorization
// app.UseAuthentication();

// 3. Middleware de autorización
app.UseAuthorization();

// 4. Configuración de endpoints para Controladores MVC
// Define la ruta predeterminada: Controlador=Home, Acción=Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Si tienes Minimal APIs (como WeatherForecast), puedes mapearlas aquí también
// app.MapGet("/weatherforecast", () => { ... }); // Si necesitas mantener este endpoint

// app.MapRazorPages(); // Si usas Razor Pages

app.Run(); // Inicia la aplicación

// Definición de WeatherForecast (SI LA NECESITAS, si no, elimínala)
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

