-- Insertar monedas 
INSERT INTO Monedas (Codigo, Nombre, Simbolo, Activa)
VALUES
    ('USD', 'Dólar Estadounidense', '$', 1),
    ('EUR', 'Euro', '€', 1),
    ('MXN', 'Peso Mexicano', '$', 1),
    ('JPY', 'Yen Japonés', '¥', 1),
    ('CAD', 'Dólar Canadiense', '$', 1),
    ('COP', 'Peso Colombiano', '$', 1),
    ('BRL', 'Real Brasileño', 'R$', 1),
    ('ARS', 'Peso Argentino', '$', 1);


-- Insertar Usuarios
INSERT INTO Usuarios (Nombre, Email, PasswordHash, MonedaPredeterminada)
VALUES (
    'Juan Pérez',
    'juan.perez@example.com',
    HASHBYTES('SHA2_256', '1234'),
    1
);

-- Insertar Categorías
INSERT INTO Categorias (UsuarioId, Nombre, Descripcion, Activa)
VALUES
    (2, 'Alimentación', 'Compras de supermercado y comida', 1),
    (2, 'Transporte', 'Gasolina, transporte público, taxis', 1),
    (2, 'Vivienda', 'Renta, hipoteca, servicios públicos', 1),
    (2, 'Entretenimiento', 'Cine, conciertos, salidas', 1),
    (2, 'Salud', 'Medicinas, consultas médicas, seguro', 1),
    (2, 'Educación', 'Libros, cursos, colegiaturas', 1),
    (2, 'Ropa', 'Prendas de vestir y accesorios', 1),
    (2, 'Tecnología', 'Dispositivos electrónicos y software', 1),
    (2, 'Ahorros', 'Depósitos a cuentas de ahorro', 1),
    (2, 'Regalos', 'Obsequios para familiares y amigos', 1),
    (2, 'Viajes', 'Hoteles, vuelos, vacaciones', 1),
    (2, 'Seguros', 'Pagos de pólizas de seguro', 1),
    (2, 'Deudas', 'Pagos de préstamos y créditos', 1),
    (2, 'Donaciones', 'Ayuda a organizaciones benéficas', 1),
    (2, 'Otros', 'Gastos varios no categorizados', 1);

-- Consulta de datos
SELECT * FROM Monedas;
SELECT * FROM Usuarios;
SELECT * FROM Categorias WHERE UsuarioId = 2;
