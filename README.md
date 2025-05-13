# SggApp: Sistema de Gestión de Gastos y Presupuestos

Este proyecto, denominado SggApp, es una aplicación web desarrollada utilizando el framework **ASP.NET Core MVC**. Su propósito principal es brindar a los usuarios una herramienta intuitiva y eficaz para la **gestión de finanzas personales**, permitiendo un control detallado sobre sus gastos y la capacidad de establecer y monitorear presupuestos.

SggApp busca empoderar a los usuarios para que tomen decisiones financieras informadas, ofreciendo una interfaz clara para el registro de transacciones y una visualización del progreso de sus límites de gasto.

## Descripción General

SggApp está diseñado para ayudar a los usuarios a obtener una visión clara de sus patrones de gasto y a mantener el control financiero mediante la asignación de límites de presupuesto por categorías y períodos definidos. La aplicación se fundamenta en una arquitectura robusta que separa las responsabilidades, facilitando su mantenimiento y futura expansión.

## Funcionalidades Principales

El proyecto implementa las siguientes características clave para la gestión financiera:

* **Gestión de Usuarios:** Incorpora un sistema de registro e inicio de sesión seguro, utilizando los mecanismos de **ASP.NET Core Identity** integrados con una **entidad de usuario personalizada (`Usuario`)**. Permite a cada usuario gestionar sus propias finanzas de manera privada.
* **Gestión Completa (CRUD) de Gastos:** Permite a los usuarios registrar, visualizar los detalles, editar y eliminar sus gastos de forma individual, asociándolos a categorías y monedas específicas. Se registran detalles como monto, fecha, descripción y lugar.
* **Gestión Completa (CRUD) de Presupuestos:** Ofrece la capacidad de crear presupuestos definidos por un límite monetario, un período de tiempo y, opcionalmente, una categoría. Permite visualizar los detalles del presupuesto, editar sus parámetros y eliminarlo si ya no es necesario. La aplicación calcula y muestra el progreso del gasto frente al límite establecido.
* **Gestión de Categorías:** Administración de las categorías (propias de cada usuario) utilizadas para clasificar los gastos y, opcionalmente, los presupuestos.
* **Gestión de Monedas:** Administración de las diferentes monedas que pueden ser utilizadas en los registros financieros y para la configuración de tipos de cambio.
* **Gestión de Tipos de Cambio:** Permite registrar las tasas de conversión entre diferentes monedas.

## Arquitectura del Proyecto

El proyecto SggApp sigue un patrón de **arquitectura de capas (N-tier)**, dividiendo la aplicación en componentes lógicos con responsabilidades bien definidas. Esta separación promueve la modularidad, la mantenibilidad y la escalabilidad del sistema, asegurando que cada parte del código tenga un propósito claro. Las principales capas son:

### Capa de Presentación (UI)

* **Propósito:** Esta es la capa más externa y es directamente responsable de la interacción con el usuario. Se encarga de recibir las peticiones del usuario, mostrar la información y capturar la entrada a través de la interfaz de usuario.
* **Componentes Clave:**
    * **Controladores:** Clases que manejan las peticiones HTTP entrantes, coordinan la lógica de negocio y seleccionan la vista apropiada para responder.
    * **Vistas:** Archivos `.cshtml` que utilizan sintaxis Razor para generar dinámicamente el HTML que se envía al navegador del usuario. Muestran datos y presentan formularios.
    * **ViewModels:** Clases diseñadas específicamente para transferir datos entre los controladores y las vistas. Contienen solo la información necesaria para la UI y a menudo incluyen Data Annotations para validación del lado del cliente y del servidor.
* **Estructura de Carpetas:** Los componentes de esta capa se encuentran principalmente en las carpetas `Controllers/`, `Views/` y `Models/` dentro del proyecto principal.

```text
SggApp/
├──Controllers/
│  ├── GastosController.cs           # Controlador para funcionalidad de Gastos
│  ├── PresupuestosController.cs     # Controlador para funcionalidad de Presupuestos
│  └── HomeController.cs             # Controlador para páginas principales
│
├──Views/
│  ├── Gastos/
│  │   ├── Create.cshtml
│  │   ├── Edit.cshtml
│  │   ├── Details.cshtml
│  │   └── Index.cshtml
│  ├── Presupuestos/
│  │   ├── Create.cshtml
│  │   ├── Edit.cshtml
│  │   ├── Details.cshtml
│  │   └── Index.cshtml
│  ├── Shared/
│  │   ├── _Layout.cshtml            # Plantilla principal
│  │   └── _ValidationScriptsPartial.cshtml  # Scripts de validación
│  ├── Home/
│  │   ├── Index.cshtml
│  │   └── Privacy.cshtml
│  ├── _ViewImports.cshtml           # Directivas de importación
│  └── _ViewStart.cshtml             # Código inicial de vistas
│
├──Models/
  ├── GastoFormViewModel.cs         # ViewModel para formularios de Gasto
  ├── GastoViewModel.cs             # ViewModel para visualización de Gasto
  ├── PresupuestoFormViewModel.cs   # ViewModel para formularios de Presupuesto
  ├── PresupuestoViewModel.cs       # ViewModel para visualización de Presupuesto
  ├── BaseViewModel.cs              # Clase base para ViewModels
  ├── RegistroViewModel.cs          # ViewModel para registro de usuarios
  ├── LoginViewModel.cs             # ViewModel para inicio de sesión
  ├── UsuarioViewModel.cs           # ViewModel para visualización de Usuario
  ├── PerfilViewModel.cs            # ViewModel para edición de Perfil
  ├── CategoriaViewModel.cs         # ViewModel para Categoría
  ├── MonedaViewModel.cs            # ViewModel para Moneda
  ├── TipoCambioViewModel.cs        # ViewModel para Tipos de Cambio
  └── TipoCambioFormViewModel.cs    # ViewModel para formularios de Tipo Cambio
```

### Capa de Lógica de Negocio (BLL)

* **Propósito:** El corazón de la aplicación, donde reside la lógica central del negocio, las reglas de validación complejas y la orquestación de las operaciones. Esta capa es independiente de la presentación y la base de datos.
* **Componentes Clave:**
    * **Servicios:** Clases que implementan la lógica de negocio específica (ej. lógica para calcular el progreso de un presupuesto, coordinar el guardado de un gasto que podría involucrar múltiples pasos). A menudo se definen interfaces para estos servicios para facilitar la inyección de dependencias y la testabilidad.
* **Estructura de Carpetas:** Los servicios y sus interfaces se organizan típicamente en las carpetas `BLL/Servicios/` (para las implementaciones) y `BLL/Interfaces/` (para las definiciones de interfaz).

```text
BLL/
├── Interfaces/
│   ├── IGastoService.cs          # Interfaz para servicio de Gastos
│   ├── IPresupuestoService.cs    # Interfaz para servicio de Presupuestos
│   ├── ICategoriaService.cs      # Interfaz para servicio de Categorías
│   ├── IMonedaService.cs         # Interfaz para servicio de Monedas
│   ├── ITipoCambioService.cs     # Interfaz para servicio de Tipos de Cambio
│   └── IUsuarioService.cs        # Interfaz para servicio de Usuarios
└── Servicios/
    ├── GastoService.cs           # Implementación servicio de Gastos
    ├── PresupuestoService.cs     # Implementación servicio de Presupuestos
    ├── CategoriaService.cs       # Implementación servicio de Categorías
    ├── MonedaService.cs          # Implementación servicio de Monedas
    ├── TipoCambioService.cs      # Implementación servicio de Tipos de Cambio
    └── UsuarioService.cs         # Implementación servicio de Usuarios
```

### Capa de Acceso a Datos (DAL)

* **Propósito:** Encargada exclusivamente de la comunicación y persistencia de datos con la base de datos subyacente. Traduce las solicitudes de la lógica de negocio en operaciones de base de datos.
* **Componentes Clave:**
    * **Entidades:** Clases POCO (Plain Old CLR Objects) que representan las tablas dentro del modelo de base de datos.
    * **DbContext:** Una clase de Entity Framework Core que actúa como una sesión con la base de datos. Permite consultar, insertar, actualizar y eliminar entidades. En este proyecto, `ApplicationDbContext` también está configurado para trabajar con ASP.NET Core Identity.
    * **Repositorios:** Clases que proporcionan una capa de abstracción sobre el `DbContext` para simplificar las operaciones comunes de acceso a datos (CRUD). A menudo se utiliza un patrón de Repositorio Genérico (`GenericRepository`) complementado con repositorios específicos para consultas más complejas.
    * **Migraciones:** Archivos generados por las herramientas de EF Core que permiten versionar y evolucionar el esquema de la base de datos a medida que el modelo de entidades cambia.
* **Estructura de Carpetas:** Los elementos de la DAL se encuentran organizados dentro de la carpeta `DAL/`, con subcarpetas como `DAL/Entidades/`, `DAL/Data/` (para el DbContext), `DAL/Repositorios/` y `DAL/Migrations/`.

```text
DAL/
├── Data/
│   └── ApplicationDbContext.cs       # Contexto principal de EF Core (gestión BD)
├── Entidades/
│   ├── Categoria.cs                 # Tabla de Categorías
│   ├── Gasto.cs                     # Tabla de Gastos
│   ├── Moneda.cs                    # Tabla de Monedas
│   ├── Presupuesto.cs               # Tabla de Presupuestos
│   ├── TipoCambio.cs                # Tabla de Tipos de Cambio
│   └── Usuario.cs                   # Usuarios (integrado con Identity)
├── Migrations/
│   └── ...                          # Archivos generados por EF Core
└── Repositorios/
    ├── GenericRepository.cs          # CRUD genérico (base)
    ├── CategoriaRepository.cs        # Operaciones específicas Categorías
    ├── GastoRepository.cs            # Operaciones específicas Gastos
    ├── MonedaRepository.cs           # Operaciones específicas Monedas
    ├── PresupuestoRepository.cs      # Operaciones específicas Presupuestos
    └── TipoCambioRepository.cs       # Operaciones específicas Tipos de Cambio
```

### Base de Datos

* **Propósito:** El almacenamiento persistente de toda la información de la aplicación (usuarios, gastos, presupuestos, etc.).
* **Componentes Clave:** Tablas, columnas, relaciones (claves foráneas), índices, restricciones.
* **Integración:** La capa DAL interactúa directamente con la base de datos a través de Entity Framework Core. La estructura de la base de datos se describe con más detalle en la sección "Estructura de la Base de Datos".

## Tecnologías y Herramientas

Las principales tecnologías y herramientas utilizadas en el desarrollo de SggApp incluyen:

* **Framework Web:** ASP.NET Core MVC
* **ORM:** Entity Framework Core
* **Sistema de Usuarios:** ASP.NET Core Identity (mecanismos de autenticación/autorización)
* **Mapeo de Objetos:** AutoMapper
* **Base de Datos:** SQL Server
* **Lenguaje de Programación:** C#
* **Interfaz de Usuario:** HTML5, CSS3, JavaScript (con soporte de **Bootstrap** para estilos y diseño responsivo, y potencialmente **jQuery/Unobtrusive Validation** para validación del lado del cliente)
* **Gestor de Paquetes:** NuGet

## Estructura de la Base de Datos

La base de datos `SistemaGG` está diseñada para almacenar y organizar la información financiera del usuario. Su esquema se define a través de entidades mapeadas en la capa DAL y gestionadas por EF Core Migrations.

Este repositorio incluye scripts SQL (`created_database.sql`, `seed_data.sql`) que reflejan la estructura y permiten poblar la base de datos.

* **Archivos incluidos:**
    * `created_database.sql`: Contiene las instrucciones necesarias para crear la base de datos `SistemaGG`, así como sus tablas, relaciones (claves foráneas) e índices.
    * `seed_data.sql`: Permite insertar algunos datos de ejemplo y realizar consultas básicas para verificar que todo funciona correctamente.

* **Cómo ejecutar los scripts (opcional para configuración manual o referencia):**
    * Abrir SQL Server Management Studio (SSMS) u otra herramienta compatible con SQL Server.
    * Ejecutar primero el archivo `created_database.sql` para crear toda la estructura de la base de datos.
    * Ejecutar luego el archivo `seed_data.sql` para insertar datos de prueba y hacer algunas consultas.
    * Es importante seguir este orden, ya que el segundo script depende de que las tablas ya estén creadas.

* **Descripción general del modelo de datos (Tablas):**
    La base de datos está pensada para llevar el control de los gastos personales de diferentes usuarios, permitiendo también definir presupuestos y realizar conversiones de moneda. A continuación se describe cada tabla:

    * **Monedas:** Guarda información sobre monedas internacionales, incluyendo su código (como USD o EUR), su nombre, símbolo y si están activas.
    * **Usuarios:** Registra a las personas que usan el sistema, incluyendo su nombre, correo electrónico, una contraseña cifrada (gestionada por Identity) y su moneda predeterminada. Esta es una tabla personalizada utilizada con ASP.NET Core Identity.
    * **Categorías:** Cada usuario puede definir sus propias categorías de gasto (como alimentación, transporte o salud), para organizar mejor sus finanzas. Una categoría está asociada a un usuario específico.
    * **Gastos:** Contiene los registros detallados de los gastos realizados, incluyendo el monto, la fecha, la categoría asociada (requerida), la moneda, la descripción opcional, el lugar y si el gasto es recurrente o no. Se incluye la fecha de registro del gasto.
    * **Presupuestos:** Permite establecer límites de gasto (límite monetario) por categoría (opcional) dentro de un rango de fechas específico (fecha de inicio y fecha de fin). También ofrece la opción de configurar un porcentaje para recibir alertas. Almacena la fecha de creación.
    * **Tipos de cambio:** Incluye las tasas de conversión entre monedas (moneda de origen y moneda de destino), necesarias para comparar o convertir montos en diferentes divisas. Se registra la tasa y la fecha de actualización.

* **Índices incluidos (según `created_database.sql`):**
    Para mejorar el rendimiento al consultar la base de datos, se han creado varios índices en columnas como `UsuarioId`, `Fecha`, `CategoriaId` y otras, optimizando las búsquedas comunes.

* **Datos de ejemplo (según `seed_data.sql`):**
    El script `seed_data.sql` agrega algunos datos para probar el funcionamiento del modelo: 8 monedas populares, 1 usuario ficticio, 15 categorías de gasto personalizadas para ese usuario. Incluye consultas para visualizar estos datos de ejemplo.

## Configuración y Ejecución

Para configurar y ejecutar el proyecto SggApp localmente, se deben seguir los siguientes pasos:

1.  **Prerrequisitos:**
    * Tener instalado el SDK de .NET (versión compatible con el proyecto).
    * Contar con una instancia de SQL Server accesible (local o remota).
    * Tener un entorno de desarrollo como Visual Studio o Visual Studio Code.

2.  **Clonar el Repositorio:**
    * Obtener una copia del código fuente desde el repositorio correspondiente.

3.  **Configurar la Conexión a la Base de Datos:**
    * Abrir el archivo `appsettings.json` (o `appsettings.Development.json`).
    * Actualizar la cadena de conexión en la sección `ConnectionStrings` con los datos de tu instancia de SQL Server, asegurándote de que la base de datos se llame `SistemaGG` (o ajustando el nombre en la cadena de conexión si usas otro). Por ejemplo:
        ```json
        "ConnectionStrings": {
          "DefaultConnection": "Server=TU_SERVIDOR;Database=SistemaGG;User Id=TU_USUARIO;Password=TU_PASSWORD;Encrypt=False;TrustServerCertificate=True;"
        }
        ```
    * Asegurarse de que el usuario de la base de datos tenga permisos para crear/modificar bases de datos si se aplican migraciones iniciales.

4.  **Crear la Base de Datos y Aplicar Migraciones de Entity Framework Core:**
    * Abrir la **Consola del Administrador de Paquetes** en Visual Studio (`Tools` -> `NuGet Package Manager` -> `Package Manager Console`).
    * Asegurarse de que el "Default project" sea el proyecto principal de SggApp.
    * Ejecutar el siguiente comando para aplicar las migraciones pendientes y crear la base de datos con el esquema definido (incluyendo las tablas de Identity y tus tablas personalizadas):
        ```powershell
        Update-Database
        ```
    * *(Nota: Si ya has ejecutado `created_database.sql` manualmente, este paso aún es necesario para que Entity Framework Core configure correctamente la base de datos con el contexto del proyecto y las tablas de Identity, o para aplicar migraciones posteriores si el modelo de datos evoluciona).*

5.  **(Opcional) Poblar la Base de Datos con Datos de Ejemplo:**
    * Si deseas cargar los datos de prueba definidos en `seed_data.sql`, puedes ejecutar este script manualmente en SQL Server Management Studio u otra herramienta SQL después de que la base de datos haya sido creada por las migraciones.

6.  **Ejecutar la Aplicación:**
    * Desde Visual Studio, presionar `F5` o el botón de inicio.
    * Desde la línea de comandos en la carpeta del proyecto, ejecutar:
        ```bash
        dotnet run
        ```
    * La aplicación se iniciará y estará accesible a través de la URL indicada en la consola (generalmente `https://localhost:xxxx`).



- Explicación de la implementación de la Capa de Acceso a Datos (DALL): https://share.vidyard.com/watch/hBVHBKsxHS88jtktbyvs7Y
- Explicación de la implementación de la Capa de Lógica de Negocio (BLL): https://share.vidyard.com/watch/ViGFD4o8SiwTJtMbz3GCP4
- Explicación de la implementación de la Capa de Presentación (API): https://share.vidyard.com/watch/1m7FTfGvcHU1fmbtF6WW8T
