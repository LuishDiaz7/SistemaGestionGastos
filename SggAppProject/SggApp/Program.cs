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

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURACI�N DE SERVICIOS (builder.Services) ---

// 1. Agregar servicios para MVC con soporte para vistas
builder.Services.AddControllersWithViews();

// Agregar DbContext con la cadena de conexi�n desde appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Registro del UnitOfWork como servicio con �mbito
builder.Services.AddScoped<UnitOfWork>(); // Aseg�rate de que UnitOfWork est� correctamente definido e implementado

// Registro de Identity (AHORA DESCOMENTADO Y COMPLETO)
builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    // A�adir otras opciones como bloqueo de cuentas, confirmaci�n de email, etc.
    options.SignIn.RequireConfirmedAccount = false; // Para simplificar en el taller si no quieres confirmaci�n por email
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Conecta Identity con tu DbContext
.AddDefaultTokenProviders(); // Necesario para tokens de restablecimiento de contrase�a, etc.

// Configurar cookies de autenticaci�n (AHORA CON RUTAS M�S COMUNES PARA SCAFFOLDING DE IDENTITY)
builder.Services.ConfigureApplicationCookie(options =>
{
    // Estas son las rutas predeterminadas generadas por el scaffolding de Identity.
    // Si no usas el scaffolding o tienes rutas personalizadas, aj�stalas.
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // options.LogoutPath = "/Identity/Account/Logout"; // Opcional, ya que Logout se maneja con POST
    options.Cookie.Name = "SggAppAuthCookie"; // Nombre de la cookie
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Duraci�n de la sesi�n
    options.SlidingExpiration = true; // Refrescar la cookie si se usa antes de la mitad de su tiempo
});

// Registro de repositorios
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<GastoRepository>();
builder.Services.AddScoped<CategoriaRepository>();
builder.Services.AddScoped<MonedaRepository>();
builder.Services.AddScoped<PresupuestoRepository>();
builder.Services.AddScoped<TipoCambioRepository>();

// Registro de servicios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IGastoService, GastoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IMonedaService, MonedaService>();
builder.Services.AddScoped<IPresupuestoService, PresupuestoService>();
builder.Services.AddScoped<ITipoCambioService, TipoCambioService>();
builder.Services.AddRazorPages();
builder.Services.AddTransient<IEmailSender, EmailSender>(); 

// Configurar AutoMapper (AHORA DESCOMENTADO)
// Aseg�rate de que tu MappingProfile.cs est� en SggApp.MappingProfiles
// Y que el 'using SggApp.MappingProfiles;' est� al principio del archivo.
builder.Services.AddAutoMapper(typeof(SggApp.MappingProfiles.MappingProfile).Assembly);

// Configuraci�n para Minimal APIs / OpenAPI (NO REQUERIDO EN PROYECTOS MVC PUROS)
// Si tu proyecto es solo MVC con vistas, puedes comentar o eliminar las siguientes l�neas si no las usas:
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
app.UseRouting(); // Debe ir antes de UseAuthentication y UseAuthorization

// Middleware de autenticaci�n (AHORA DESCOMENTADO) - �MUY IMPORTANTE! Debe ir entre UseRouting y UseAuthorization
app.UseAuthentication();

// 3. Middleware de autorizaci�n (AHORA DESCOMENTADO)
app.UseAuthorization();

// Mapea las Razor Pages de Identity (AHORA DESCOMENTADO)
app.MapRazorPages();

// Configurar el ruteo para controladores MVC 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Si tienes Minimal APIs (como WeatherForecast), puedes mapearlas aqu� tambi�n
// app.MapGet("/weatherforecast", () => { ... }); // Si necesitas mantener este endpoint

app.Run(); // Inicia la aplicaci�n

// Definici�n de WeatherForecast (SI LA NECESITAS, si no, elim�nala)
// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }