CREATE DATABASE SistemaGG;
GO

USE SistemaGG;
GO

-- 1. Tabla Monedas 
CREATE TABLE Monedas (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Codigo CHAR(3) NOT NULL UNIQUE, 
    Nombre NVARCHAR(50) NOT NULL,
    Simbolo NVARCHAR(5) NULL,
    Activa BIT NOT NULL DEFAULT 1
);

-- 2. Tabla Usuarios 
CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash VARBINARY(256) NOT NULL,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    MonedaPredeterminada INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Usuarios_Monedas FOREIGN KEY (MonedaPredeterminada) REFERENCES Monedas(Id)
);

-- 3. Tabla Categorias 
CREATE TABLE Categorias (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    Nombre NVARCHAR(50) NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Activa BIT NOT NULL DEFAULT 1,
    CONSTRAINT FK_Categorias_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT UQ_CategoriaUsuario UNIQUE (UsuarioId, Nombre)
);

-- 4. Tabla Gastos 
CREATE TABLE Gastos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    CategoriaId INT NOT NULL,
    MonedaId INT NOT NULL,
    Monto DECIMAL(18,2) NOT NULL,
    Fecha DATETIME NOT NULL,
    Descripcion NVARCHAR(255) NULL,
    Lugar NVARCHAR(100) NULL,
    EsRecurrente BIT NOT NULL DEFAULT 0,
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Gastos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Gastos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    CONSTRAINT FK_Gastos_Monedas FOREIGN KEY (MonedaId) REFERENCES Monedas(Id),
    CONSTRAINT CHK_Gastos_Monto CHECK (Monto > 0)
);

-- 5. Tabla Presupuestos
CREATE TABLE Presupuestos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UsuarioId INT NOT NULL,
    CategoriaId INT NULL,
    MonedaId INT NOT NULL,
    Limite DECIMAL(18,2) NOT NULL,
    FechaInicio DATE NOT NULL,
    FechaFin DATE NOT NULL,
    NotificarAl INT NULL, 
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Presupuestos_Usuarios FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id),
    CONSTRAINT FK_Presupuestos_Categorias FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id),
    CONSTRAINT FK_Presupuestos_Monedas FOREIGN KEY (MonedaId) REFERENCES Monedas(Id),
    CONSTRAINT CHK_Presupuestos_Fechas CHECK (FechaFin > FechaInicio),
    CONSTRAINT CHK_Presupuestos_Limite CHECK (Limite > 0)
);

-- 6. Tabla tipo de cambio
CREATE TABLE TiposCambio (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MonedaOrigen INT NOT NULL,
    MonedaDestino INT NOT NULL,
    Tasa DECIMAL(18,6) NOT NULL,
    FechaActualizacion DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_TiposCambio_MonedaOrigen FOREIGN KEY (MonedaOrigen) REFERENCES Monedas(Id),
    CONSTRAINT FK_TiposCambio_MonedaDestino FOREIGN KEY (MonedaDestino) REFERENCES Monedas(Id),
    CONSTRAINT UQ_TiposCambio UNIQUE (MonedaOrigen, MonedaDestino)
);

-- Creación de índices
CREATE INDEX IX_Categorias_UsuarioId ON Categorias(UsuarioId);
CREATE INDEX IX_Gastos_UsuarioId ON Gastos(UsuarioId);
CREATE INDEX IX_Gastos_CategoriaId ON Gastos(CategoriaId);
CREATE INDEX IX_Gastos_Fecha ON Gastos(Fecha);
CREATE INDEX IX_Gastos_UsuarioId_Fecha ON Gastos(UsuarioId, Fecha);
CREATE INDEX IX_Presupuestos_UsuarioId ON Presupuestos(UsuarioId);
CREATE INDEX IX_Presupuestos_CategoriaId ON Presupuestos(CategoriaId);
CREATE INDEX IX_Presupuestos_Fechas ON Presupuestos(FechaInicio, FechaFin);
