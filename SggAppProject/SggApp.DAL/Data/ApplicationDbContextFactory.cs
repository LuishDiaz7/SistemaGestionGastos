using Microsoft.EntityFrameworkCore.Design; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using System; 
using System.IO; 

namespace SggApp.DAL.Data
{
    /// <summary>
    /// Fábrica para crear instancias de ApplicationDbContext en tiempo de diseño.
    /// Implementa la interfaz IDesignTimeDbContextFactory para ser utilizada por las herramientas de línea de comandos de Entity Framework Core (como dotnet ef migrations).
    /// Permite que las herramientas accedan al contexto de base de datos durante la fase de diseño, incluso cuando la configuración normal de la aplicación (ej. a través de Dependency Injection) no está disponible.
    /// </summary>
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        /// <summary>
        /// Crea una nueva instancia de ApplicationDbContext con las opciones de configuración necesarias para las herramientas de diseño de Entity Framework Core.
        /// Este método es llamado automáticamente por las herramientas de EF Core.
        /// </summary>
        /// <param name="args">Argumentos de línea de comandos pasados a la operación (no utilizados en esta implementación).</param>
        /// <returns>Una nueva instancia de ApplicationDbContext configurada.</returns>
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Construye la configuración de la aplicación manualmente para acceder a la cadena de conexión.
            var configuration = new ConfigurationBuilder()
                // Establece el directorio base para buscar archivos de configuración.
                .SetBasePath(Directory.GetCurrentDirectory())
                // Añade el archivo appsettings.json a la configuración.
                // Nota: Se utiliza una ruta absoluta hardcodeada ("C:/Users/...") que puede causar problemas de portabilidad.
                .AddJsonFile("C:/Users/luish/OneDrive/Documents/Herramientas de Programacion III/SistemaGestionGastos/SggApp Project/SggApp/appsettings.json")
                // Construye el objeto de configuración.
                .Build();

            // Crea un constructor de opciones para el DbContext.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Configura el DbContext para usar SQL Server con la cadena de conexión "DefaultConnection" obtenida de la configuración.
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // Crea y retorna una nueva instancia de ApplicationDbContext pasando las opciones configuradas.
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
