# SistemaGG – Base de Datos para Gestión de Gastos

Este repositorio contiene dos scripts SQL que permiten crear y poblar una base de datos llamada **SistemaGG**, pensada para registrar y controlar los gastos personales de los usuarios. 

---

##  Archivos incluidos

- **created_database.sql**  
  Contiene las instrucciones necesarias para crear la base de datos, así como sus tablas, relaciones e índices.

- **seed_data.sql**  
  Permite insertar algunos datos de ejemplo y realizar consultas básicas para verificar que todo funciona correctamente.

---

##  ¿Cómo ejecutar los scripts?

1. Abrir SQL Server Management Studio (SSMS) u otra herramienta compatible con SQL Server.
2. Ejecutar primero el archivo `created_database.sql` para crear toda la estructura de la base de datos.
3. Ejecutar luego el archivo `seed_data.sql` para insertar datos de prueba y hacer algunas consultas.

> Es importante seguir este orden, ya que el segundo script depende de que las tablas ya estén creadas.

---

##  Descripción general del modelo de datos

La base de datos está pensada para llevar el control de los gastos personales de diferentes usuarios, permitiendo también definir presupuestos y realizar conversiones de moneda. A continuación se describe cada tabla:

### Monedas
Guarda información sobre monedas internacionales, incluyendo su código (como USD o EUR), su nombre, símbolo y si están activas.

### Usuarios
Registra a las personas que usan el sistema, incluyendo su nombre, correo electrónico, una contraseña cifrada y su moneda predeterminada.

### Categorías
Cada usuario puede definir sus propias categorías de gasto (como alimentación, transporte o salud), para organizar mejor sus finanzas.

### Gastos
Contiene los registros de los gastos realizados, incluyendo el monto, la fecha, la categoría y si el gasto es recurrente o no.

### Presupuestos
Permite establecer límites de gasto por categoría dentro de un rango de fechas específico. También ofrece la opción de configurar alertas.

### Tipos de cambio
Incluye las tasas de conversión entre monedas, necesarias para comparar o convertir montos en diferentes divisas.

---

##  Índices incluidos

Para mejorar el rendimiento al consultar la base de datos, se han creado varios índices en columnas como `UsuarioId`, `Fecha`, `CategoriaId` y otras.

---

##  Datos de ejemplo

El script `seed_data.sql` agrega algunos datos para probar el funcionamiento del modelo:

- 8 monedas populares como el dólar estadounidense, euro y peso colombiano.
- 1 usuario ficticio llamado **Juan Pérez**, con correo electrónico `juan.perez@example.com`.
- 15 categorías de gasto personalizadas para ese usuario, como alimentación, transporte, salud y tecnología.

Además, el script incluye tres consultas básicas:

```sql
-- Ver todas las monedas
SELECT * FROM Monedas;


-- Ver los usuarios registrados
SELECT * FROM Usuarios;

-- Ver las categorías del usuario con ID 2
SELECT * FROM Categorias WHERE UsuarioId = 2;

```
- Explicación de la implementación de la Capa de Acceso a Datos (DALL): https://share.vidyard.com/watch/hBVHBKsxHS88jtktbyvs7Y
- Explicación de la implementación de la Capa de Lógica de Negocio (BLL): https://share.vidyard.com/watch/ViGFD4o8SiwTJtMbz3GCP4
