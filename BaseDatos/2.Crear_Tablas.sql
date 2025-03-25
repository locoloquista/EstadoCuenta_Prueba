-- ============================================
-- CREACIÓN DE TABLAS
-- ============================================

-- Tabla de Clientes
CREATE TABLE clientes (
    id bigint PRIMARY KEY IDENTITY(1,1),
    nombre nvarchar(255) NOT NULL,
    apellido_paterno nvarchar(255) NOT NULL,
    apellido_materno nvarchar(255) NULL,
    activo bit NOT NULL DEFAULT 1
);

-- Tabla de Tarjetas de Crédito
CREATE TABLE tarjetas_credito (
    id bigint PRIMARY KEY IDENTITY(1,1),
    cliente_id bigint NOT NULL FOREIGN KEY REFERENCES clientes(id),
    numero_tarjeta nvarchar(255) NOT NULL UNIQUE,
    limite_credito decimal(10, 2) NOT NULL,
    saldo_actual decimal(10, 2) NOT NULL,
    activo bit NOT NULL DEFAULT 1
);

-- Tabla de Tipos de Transacción
CREATE TABLE tipos_transaccion (
    id bigint PRIMARY KEY IDENTITY(1,1),
    nombre_tipo nvarchar(255) NOT NULL UNIQUE,
    activo bit NOT NULL DEFAULT 1
);

-- Tabla de Transacciones
CREATE TABLE transacciones (
    id bigint PRIMARY KEY IDENTITY(1,1),
    tarjeta_id bigint NOT NULL FOREIGN KEY REFERENCES tarjetas_credito(id),
    fecha_transaccion date NOT NULL,
    descripcion nvarchar(255) NOT NULL,
    monto decimal(10, 2) NOT NULL,
    tipo_transaccion_id bigint NOT NULL FOREIGN KEY REFERENCES tipos_transaccion(id),
    activo bit NOT NULL DEFAULT 1
);

-- Tabla de Configuraciones de Intereses
CREATE TABLE configuraciones_intereses (
    id bigint PRIMARY KEY IDENTITY(1,1),
    tasa_interes decimal(5, 2) NOT NULL,
    tasa_pago_minimo decimal(5, 2) NOT NULL,
    fecha_vigencia date NOT NULL,
    activo bit NOT NULL DEFAULT 1
);

-- Tabla de Bitácora
CREATE TABLE bitacora (
    id bigint PRIMARY KEY IDENTITY(1,1),
    tabla nvarchar(255) NOT NULL,
    accion nvarchar(50) NOT NULL,
    registro_id bigint NOT NULL,
    fecha datetime NOT NULL DEFAULT GETDATE(),
    usuario nvarchar(255) NOT NULL,
    datos nvarchar(MAX) NOT NULL
);