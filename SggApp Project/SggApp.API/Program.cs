using Microsoft.EntityFrameworkCore;
using SggApp.BLL.Interfaces;
using SggApp.BLL.Servicios;
using SggApp.DAL; // Aseg�rate de que UnitOfWork est� en este namespace
using SggApp.DAL.Data; // Aseg�rate de que ApplicationDbContext est� en este namespace
using SggApp.DAL.Repositorios; // Aseg�rate de que tus repositorios est�n en este namespace
using System.Reflection; // Necesario para AutoMapper si lo registraste con typeof().Assembly
// using SggApp.MappingProfiles; // Descomentar si registraste AutoMapper con typeof(TuPerfil).Assembly
// using SggApp.Models; // Descomentar si tus ViewModels est�n aqu�
// using SggApp.Web.Models; // Descomentar si tus ViewModels est�n aqu� (m�s com�n para proyectos Web)
using AutoMapper; // Necesario para AutoMapper

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACI�N DE SERVICIOS (builder.Services) ---

// 1. Agregar servicios para MVC con soporte para vistas
builder.Services.AddControllersWithViews();

// Agregar DbContext con la cadena de conexi�n desde appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro del UnitOfWork como servicio con �mbito
builder.Services.AddScoped<UnitOfWork>(); // Aseg�rate de que UnitOfWork est� correctamente definido e implementado

// Registro de Identity (SI USAS Identity, basado en tu c�digo anterior)
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

// Configurar cookies de autenticaci�n (SI USAS Identity)
// builder.Services.ConfigureApplicationCookie(options =>
// {
//     options.LoginPath = "/Usuarios/Login";
//     options.AccessDeniedPath = "/Usuarios/AccessDenied";
// });

// Registro de repositorios (Aseg�rate que estos repositorios est�n en SggApp.DAL.Repositorios y correctos)
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<GastoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<MonedaRepository>();
builder.Services.AddScoped<PresupuestoRepository>();
builder.Services.AddScoped<TipoCambioRepository>();

// Registro de servicios (Aseg�rate que estas interfaces/clases est�n en SggApp.BLL.Interfaces/Servicios)
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<ITipoCambioService, TipoCambioService>();

// Configurar AutoMapper (Aseg�rate de que tu MappingProfile.cs est� en SggApp.MappingProfiles)
// Aseg�rate de que este using SggApp.MappingProfiles; est� presente si usaste typeof(MappingProfile).Assembly
// builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly); // Descomentar si usas esta forma de registro

// Configuraci�n para Minimal APIs / OpenAPI (SI LO NECESITAS, si no, puedes eliminarlo)
builder.Services.AddEndpointsApiExplorer(); // Necesario para OpenAPI
builder.Services.AddSwaggerGen(); // Necesario para Swagger UI


var app = builder.Build();

// --- CONFIGURACI�N DEL PIPELINE DE SOLICITUDES HTTP (app.Use...) ---

// Configurar el pipeline para desarrollo (p�gina de excepciones detallada)
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi(); // Mapea los endpoints de OpenAPI/Swagger en desarrollo (si los usas)
    app.UseDeveloperExceptionPage(); // Muestra errores detallados en desarrollo
    app.UseSwagger(); // Habilita Swagger UI en desarrollo (si lo usas)
    app.UseSwaggerUI(); // Habilita Swagger UI en desarrollo (si lo usas)
}
else
{
    // Configuraci�n para producci�n
    app.UseExceptionHandler("/Home/Error"); // Redirige a una p�gina de error en producci�n
    // El valor predeterminado HSTS es 30 d�as. Puedes que desee cambiar esto para escenarios de producci�n.
    app.UseHsts(); // Habilita HSTS
}

// Middleware de redirecci�n HTTPS
app.UseHttpsRedirection();

// Middleware para servir archivos est�ticos (CSS, JS, im�genes, etc.)
app.UseStaticFiles();

// 2. Middleware de enrutamiento - �CRUCIAL PARA MVC!
app.UseRouting(); // Debe ir antes de UseAuthorization y MapControllerRoute/MapEndpoints

// Middleware de autenticaci�n (SI USAS Identity) - Debe ir entre UseRouting y UseAuthorization
// app.UseAuthentication();

// 3. Middleware de autorizaci�n
app.UseAuthorization();

// 4. Configuraci�n de endpoints para Controladores MVC
// Define la ruta predeterminada: Controlador=Home, Acci�n=Index
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Si tienes Minimal APIs (como WeatherForecast), puedes mapearlas aqu� tambi�n
// app.MapGet("/weatherforecast", () => { ... }); // Si necesitas mantener este endpoint

// app.MapRazorPages(); // Si usas Razor Pages

app.Run(); // Inicia la aplicaci�n

// Definici�n de WeatherForecast (SI LA NECESITAS, si no, elim�nala)
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

