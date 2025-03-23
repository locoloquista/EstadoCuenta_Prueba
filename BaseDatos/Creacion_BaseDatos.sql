-- ============================================
-- CREACIÓN DE BASE DE DATO
-- ============================================
CREATE DATABASE estado_cuenta_tarjeta;

Use estado_cuenta_tarjeta;

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

-- ============================================
-- PROCEDIMIENTOS ALMACENADOS
-- ============================================

-- Procedimiento para registrar en la bitácora
CREATE PROCEDURE RegistrarBitacora
    @Tabla nvarchar(255),
    @Accion nvarchar(50),
    @RegistroId bigint,
    @Usuario nvarchar(255),
    @Datos nvarchar(MAX)
AS
BEGIN
    INSERT INTO bitacora (tabla, accion, registro_id, usuario, datos)
    VALUES (@Tabla, @Accion, @RegistroId, @Usuario, @Datos);
END;

-- Procedimiento para obtener el estado de cuenta
IF OBJECT_ID('ObtenerEstadoCuenta', 'P') IS NOT NULL
    DROP PROCEDURE ObtenerEstadoCuenta;
GO

-- Crear el procedimiento almacenado
CREATE PROCEDURE ObtenerEstadoCuenta
    @TarjetaId bigint
AS
BEGIN
    BEGIN TRY
        -- Usar una tabla derivada para calcular las transacciones
        WITH TransaccionesFiltradas AS (
            SELECT 
                t.tarjeta_id,
                t.monto,
                t.fecha_transaccion,
                tt.nombre_tipo AS tipo_transaccion
            FROM transacciones t
            INNER JOIN tipos_transaccion tt ON t.tipo_transaccion_id = tt.id
            WHERE t.tarjeta_id = @TarjetaId AND t.activo = 1
        )
        SELECT 
            tc.numero_tarjeta AS NumeroTarjeta,
            CONCAT(c.nombre, ' ', c.apellido_paterno, ' ', ISNULL(c.apellido_materno, '')) AS NombreCliente,
            tc.saldo_actual AS SaldoTotal,
            tc.limite_credito AS LimiteCredito,
            (tc.limite_credito - tc.saldo_actual) AS SaldoDisponible,
            -- Compras realizadas en el mes actual
            SUM(CASE 
                WHEN tf.fecha_transaccion >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) 
                     AND tf.tipo_transaccion = 'compra'
                THEN tf.monto 
                ELSE 0 
            END) AS ComprasMesActual,
            -- Compras realizadas en el mes anterior
            SUM(CASE 
                WHEN tf.fecha_transaccion >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0) 
                     AND tf.fecha_transaccion < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
                     AND tf.tipo_transaccion = 'compra'
                THEN tf.monto 
                ELSE 0 
            END) AS ComprasMesAnterior,
            -- Configuración de intereses
            ci.tasa_interes AS TasaInteres,
            ci.tasa_pago_minimo AS TasaPagoMinimo,
            -- Cálculo del interés bonificable
            (tc.saldo_actual * ci.tasa_interes / 100) AS InteresBonificable,
            -- Cálculo de la cuota mínima
            (tc.saldo_actual * ci.tasa_pago_minimo / 100) AS CuotaMinima,
            -- Cálculo del pago de contado con intereses
            (tc.saldo_actual + (tc.saldo_actual * ci.tasa_interes / 100)) AS PagoContadoConIntereses
        FROM tarjetas_credito tc
        INNER JOIN clientes c ON tc.cliente_id = c.id
        LEFT JOIN TransaccionesFiltradas tf ON tc.id = tf.tarjeta_id
        CROSS JOIN configuraciones_intereses ci
        WHERE tc.id = @TarjetaId AND tc.activo = 1 AND c.activo = 1 AND ci.activo = 1
        GROUP BY 
            tc.numero_tarjeta, 
            c.nombre, 
            c.apellido_paterno, 
            c.apellido_materno, 
            tc.saldo_actual, 
            tc.limite_credito, 
            ci.tasa_interes, 
            ci.tasa_pago_minimo;
    END TRY
    BEGIN CATCH
        -- Registrar el error en la bitácora
        DECLARE @ErrorDescripcion nvarchar(max);
        SET @ErrorDescripcion = 'Error al obtener el estado de cuenta. Detalles: ' + ERROR_MESSAGE();

        EXEC RegistrarBitacora 
            @Tabla = 'tarjetas_credito', 
            @Accion = 'error', 
            @RegistroId = @TarjetaId, 
            @Usuario = 'Sistema', 
            @Datos = @ErrorDescripcion;
    END CATCH;
END;
GO
EXEC ObtenerEstadoCuenta 
    @TarjetaId = 2;

-- Procedimiento para obtener informacion de clientes
-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('ObtenerInformacionClientes', 'P') IS NOT NULL
    DROP PROCEDURE ObtenerInformacionClientes;
GO
-- Crear el procedimiento almacenado
CREATE PROCEDURE ObtenerInformacionClientes
AS
BEGIN
    BEGIN TRY
        -- Seleccionar información de los clientes y el número de tarjetas activas asociadas
        SELECT 
            c.id AS ClienteId,
            CONCAT(c.nombre, ' ', c.apellido_paterno, ' ', ISNULL(c.apellido_materno, '')) AS NombreCompleto,
            COUNT(tc.id) AS NumeroTarjetasActivas
        FROM clientes c
        LEFT JOIN tarjetas_credito tc ON c.id = tc.cliente_id AND tc.activo = 1
        WHERE c.activo = 1
        GROUP BY c.id, c.nombre, c.apellido_paterno, c.apellido_materno
        ORDER BY c.id;
    END TRY
    BEGIN CATCH
        -- Registrar el error en la bitácora
        DECLARE @ErrorMessage nvarchar(MAX) = ERROR_MESSAGE();
        EXEC RegistrarBitacora 'clientes/tarjetas_credito', 'error', 0, 'Sistema', @ErrorMessage;
    END CATCH;
END;
GO

-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('ObtenerInformacionClientePorId', 'P') IS NOT NULL
    DROP PROCEDURE ObtenerInformacionClientePorId;
GO
-- Crear el procedimiento para obtener información del cliente y sus tarjetas
CREATE PROCEDURE ObtenerInformacionClientePorId
    @ClienteId bigint
AS
BEGIN
    BEGIN TRY
        -- Obtener información del cliente
         SELECT 
            c.id AS ClienteId,
            CONCAT(c.nombre, ' ', c.apellido_paterno, ' ', ISNULL(c.apellido_materno, '')) AS NombreCompleto,
            COUNT(tc.id) AS NumeroTarjetasActivas
        FROM clientes c
        LEFT JOIN tarjetas_credito tc ON c.id = tc.cliente_id AND tc.activo = 1
        WHERE c.id = @ClienteId and c.activo = 1
		GROUP BY c.id, c.nombre, c.apellido_paterno, c.apellido_materno
    END TRY
    BEGIN CATCH
        -- Registrar el error en la bitácora
        DECLARE @ErrorDescripcion nvarchar(max);
        SET @ErrorDescripcion = 'Error al obtener información del cliente. Detalles: ' + ERROR_MESSAGE();

        EXEC RegistrarBitacora 
            @Tabla = 'clientes/tarjetas_credito', 
            @Accion = 'error', 
            @RegistroId = @ClienteId, 
            @Usuario = 'Sistema', 
            @Datos = @ErrorDescripcion;
    END CATCH;
END;
GO

-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('ObtenerInformacionTarjetasPorClienteId', 'P') IS NOT NULL
    DROP PROCEDURE ObtenerInformacionTarjetasPorClienteId;
GO
-- Crear el procedimiento para obtener información del cliente y sus tarjetas
CREATE PROCEDURE ObtenerInformacionTarjetasPorClienteId
    @ClienteId bigint
AS
BEGIN
    BEGIN TRY
        -- Obtener información del cliente
        SELECT 
            tc.id AS TarjetaId,
            tc.numero_tarjeta AS NumeroTarjeta,
            tc.limite_credito AS LimiteCredito,
            tc.saldo_actual AS SaldoActual,
            (tc.limite_credito - tc.saldo_actual) AS MontoDisponible,
            tc.activo AS TarjetaActiva
        FROM tarjetas_credito tc
        WHERE tc.cliente_id = @ClienteId
        ORDER BY tc.id;
    END TRY
    BEGIN CATCH
        -- Registrar el error en la bitácora
        DECLARE @ErrorDescripcion nvarchar(max);
        SET @ErrorDescripcion = 'Error al obtener información de las tarjetas de cliente. Detalles: ' + ERROR_MESSAGE();

        EXEC RegistrarBitacora 
            @Tabla = 'clientes/tarjetas_credito', 
            @Accion = 'error', 
            @RegistroId = @ClienteId, 
            @Usuario = 'Sistema', 
            @Datos = @ErrorDescripcion;
    END CATCH;
END;
GO


-- Procedimiento para obtener transacciones realizadas por tarjeta en el mes
-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('ObtenerTransaccionesDelMes', 'P') IS NOT NULL
    DROP PROCEDURE ObtenerTransaccionesDelMes;
GO
-- Crear el procedimiento almacenado
CREATE PROCEDURE ObtenerTransaccionesDelMes
    @TarjetaId bigint
AS
BEGIN
    BEGIN TRY
        -- Seleccionar todas las transacciones (compras y pagos) del mes actual
        SELECT 
            'Compra' AS TipoTransaccion,
            t.fecha_transaccion AS Fecha,
            t.descripcion AS Descripcion,
            t.monto AS Monto
        FROM transacciones t
        WHERE t.tarjeta_id = @TarjetaId
          AND t.activo = 1
          AND MONTH(t.fecha_transaccion) = MONTH(GETDATE())
          AND YEAR(t.fecha_transaccion) = YEAR(GETDATE())
        
        UNION ALL
        
        SELECT 
            'Pago' AS TipoTransaccion,
            p.fecha_transaccion AS Fecha,
            p.descripcion AS Descripcion,
            p.monto AS Monto
        FROM transacciones p
        WHERE p.tarjeta_id = @TarjetaId
          AND p.activo = 1
          AND MONTH(p.fecha_transaccion) = MONTH(GETDATE())
          AND YEAR(p.fecha_transaccion) = YEAR(GETDATE())
        
        ORDER BY Fecha DESC;
    END TRY
    BEGIN CATCH
        -- Registrar el error en la bitácora
        DECLARE @ErrorMessage nvarchar(MAX) = ERROR_MESSAGE();
        EXEC RegistrarBitacora 'transacciones/pagos', 'error', @TarjetaId, 'Sistema', @ErrorMessage;
    END CATCH;
END;


-- Procedimiento para agregar un cliente
-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('AgregarCliente', 'P') IS NOT NULL
    DROP PROCEDURE AgregarCliente;
GO
-- Crear el procedimiento almacenado
CREATE PROCEDURE AgregarCliente
    @Nombre nvarchar(255),
    @ApellidoPaterno nvarchar(255),
    @ApellidoMaterno nvarchar(255),
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO clientes (nombre, apellido_paterno, apellido_materno, activo)
        VALUES (@Nombre, @ApellidoPaterno, @ApellidoMaterno, 1);

        DECLARE @ClienteId bigint = SCOPE_IDENTITY();

         -- Registrar en la bitácora
        DECLARE @Descripcion nvarchar(max);
        SET @Descripcion = 'Nombre: ' + ISNULL(@Nombre, '') + 
                           ', ApellidoPaterno: ' + ISNULL(@ApellidoPaterno, '') + 
                           ', ApellidoMaterno: ' + ISNULL(@ApellidoMaterno, '');

        EXEC RegistrarBitacora 'clientes', 'insertar', @ClienteId, @Usuario, @Descripcion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

       -- Registrar el error en la bitácora
        EXEC RegistrarBitacora 
            @Tabla = 'clientes', 
            @Accion = 'error', 
            @RegistroId = 0, 
            @Usuario = @Usuario, 
            @Datos = ERROR_MESSAGE;
    END CATCH;
END;

-- Procedimiento para agregar una tarjeta de crédito
-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('AgregarTarjetaCredito', 'P') IS NOT NULL
    DROP PROCEDURE AgregarTarjetaCredito;
GO
-- Crear el procedimiento almacenado
CREATE PROCEDURE AgregarTarjetaCredito
    @ClienteId bigint,
    @NumeroTarjeta nvarchar(255),
    @LimiteCredito decimal(10, 2),
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO tarjetas_credito (cliente_id, numero_tarjeta, limite_credito, saldo_actual, activo)
        VALUES (@ClienteId, @NumeroTarjeta, @LimiteCredito, 0, 1);

        DECLARE @TarjetaId bigint = SCOPE_IDENTITY();

        -- Registrar en la bitácora
        DECLARE @Descripcion nvarchar(max);
        SET @Descripcion = 'ClienteId: ' + CAST(@ClienteId AS nvarchar) + 
                           ', NumeroTarjeta: ' + ISNULL(@NumeroTarjeta, '') + 
                           ', LimiteCredito: ' + CAST(@LimiteCredito AS nvarchar);

        EXEC RegistrarBitacora 'tarjetas_credito', 'insertar', @TarjetaId, @Usuario, @Descripcion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        EXEC RegistrarBitacora 'tarjetas_credito', 'error', 0, @Usuario, ERROR_MESSAGE;

    END CATCH;
END;

-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('AgregarTransaccion', 'P') IS NOT NULL
    DROP PROCEDURE AgregarTransaccion;
GO
-- Crear el procedimiento para registrar compras y pagos
CREATE PROCEDURE AgregarTransaccion
    @TarjetaId bigint,
    @FechaTransaccion date,
    @Descripcion nvarchar(255),
    @Monto decimal(10, 2),
    @TipoTransaccionId bigint, -- ID del tipo de transacción ('compra' o 'pago')
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que el tipo de transacción exista y esté activo
        IF NOT EXISTS (SELECT 1 FROM tipos_transaccion WHERE id = @TipoTransaccionId AND activo = 1)
        BEGIN
            THROW 50000, 'El ID del tipo de transacción no es válido o está inactivo.', 1;
        END

        -- Insertar la transacción
        INSERT INTO transacciones (tarjeta_id, fecha_transaccion, descripcion, monto, tipo_transaccion_id, activo)
        VALUES (
            @TarjetaId, 
            @FechaTransaccion, 
            @Descripcion, 
            @Monto, 
            @TipoTransaccionId, 
            1
        );

        -- Actualizar el saldo de la tarjeta
        DECLARE @TipoTransaccion nvarchar(50);
        SELECT @TipoTransaccion = nombre_tipo 
        FROM tipos_transaccion 
        WHERE id = @TipoTransaccionId;

        IF @TipoTransaccion = 'compra'
        BEGIN
            -- Incrementar el saldo actual para una compra
            UPDATE tarjetas_credito
            SET saldo_actual = saldo_actual + @Monto
            WHERE id = @TarjetaId;
        END
        ELSE IF @TipoTransaccion = 'pago'
        BEGIN
            -- Reducir el saldo actual para un pago
            UPDATE tarjetas_credito
            SET saldo_actual = saldo_actual - @Monto
            WHERE id = @TarjetaId;
        END

        -- Registrar en la bitácora
        DECLARE @TransaccionId bigint = SCOPE_IDENTITY();
        DECLARE @BitacoraDescripcion nvarchar(max);
        SET @BitacoraDescripcion = 'TarjetaId: ' + CAST(@TarjetaId AS nvarchar) + 
                                   ', Fecha: ' + CAST(@FechaTransaccion AS nvarchar) + 
                                   ', Monto: ' + CAST(@Monto AS nvarchar) + 
                                   ', TipoTransaccionId: ' + CAST(@TipoTransaccionId AS nvarchar);

        EXEC RegistrarBitacora 
            @Tabla = 'transacciones', 
            @Accion = 'insertar', 
            @RegistroId = @TransaccionId, 
            @Usuario = @Usuario, 
            @Datos = @BitacoraDescripcion;

        -- Confirmar la transacción
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Revertir la transacción en caso de error
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        DECLARE @ErrorMensaje nvarchar(max);
        SET @ErrorMensaje = ERROR_MESSAGE();
        EXEC RegistrarBitacora 
            @Tabla = 'transacciones', 
            @Accion = 'error', 
            @RegistroId = 0, 
            @Usuario = @Usuario, 
            @Datos = @ErrorMensaje;
    END CATCH;
END;
GO

-- Procedimiento para revertir una compra
-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('RevertirCompra', 'P') IS NOT NULL
    DROP PROCEDURE RevertirCompra;
GO
-- Crear el procedimiento almacenado
CREATE PROCEDURE RevertirCompra
    @TransaccionId bigint,
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obtener el monto y la tarjeta asociada a la transacción
        DECLARE @Monto decimal(10, 2);
        DECLARE @TarjetaId bigint;

        SELECT @Monto = monto, @TarjetaId = tarjeta_id
        FROM transacciones
        WHERE id = @TransaccionId AND activo = 1;

        -- Marcar la transacción como inactiva
        UPDATE transacciones
        SET activo = 0
        WHERE id = @TransaccionId;

        -- Actualizar el saldo de la tarjeta (restar el monto)
        UPDATE tarjetas_credito
        SET saldo_actual = saldo_actual - @Monto
        WHERE id = @TarjetaId;

        -- Registrar en la bitácora
        DECLARE @BitacoraDescripcion nvarchar(max);
        SET @BitacoraDescripcion = 'TransaccionId: ' + CAST(@TransaccionId AS nvarchar) + 
                                   ', Monto: ' + CAST(@Monto AS nvarchar);

        EXEC RegistrarBitacora 'transacciones', 'revertir', @TransaccionId, @Usuario, @BitacoraDescripcion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        DECLARE @ErrorMensaje nvarchar(max);
        SET @ErrorMensaje = ERROR_MESSAGE();

        EXEC RegistrarBitacora 'transacciones', 'error', @TransaccionId, @Usuario, @ErrorMensaje;
    END CATCH;
END;


-- Procedimiento para revertir un pago
-- Verificar si el procedimiento ya existe y eliminarlo
IF OBJECT_ID('RevertirPago', 'P') IS NOT NULL
    DROP PROCEDURE RevertirPago;
GO

-- Crear el procedimiento para revertir un pago
CREATE PROCEDURE RevertirPago
    @TransaccionId bigint, -- ID de la transacción a revertir
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Validar que la transacción exista, sea un pago y esté activa
        DECLARE @Monto decimal(10, 2);
        DECLARE @TarjetaId bigint;
        DECLARE @TipoTransaccion nvarchar(50);

        SELECT 
            @Monto = monto, 
            @TarjetaId = tarjeta_id,
            @TipoTransaccion = (SELECT nombre_tipo 
                                FROM tipos_transaccion 
                                WHERE id = tipo_transaccion_id)
        FROM transacciones
        WHERE id = @TransaccionId AND activo = 1;

        IF @TipoTransaccion IS NULL OR @TipoTransaccion != 'pago'
        BEGIN
            THROW 50000, 'La transacción no es un pago válido o ya ha sido revertida.', 1;
        END

        -- Marcar la transacción como inactiva
        UPDATE transacciones
        SET activo = 0
        WHERE id = @TransaccionId;

        -- Actualizar el saldo de la tarjeta (sumar el monto del pago revertido)
        UPDATE tarjetas_credito
        SET saldo_actual = saldo_actual + @Monto
        WHERE id = @TarjetaId;

        -- Registrar en la bitácora
        DECLARE @BitacoraDescripcion nvarchar(max);
        SET @BitacoraDescripcion = 'Reversión de pago. TransaccionId: ' + CAST(@TransaccionId AS nvarchar) + 
                                   ', TarjetaId: ' + CAST(@TarjetaId AS nvarchar) + 
                                   ', Monto: ' + CAST(@Monto AS nvarchar);

        EXEC RegistrarBitacora 
            @Tabla = 'transacciones', 
            @Accion = 'revertir', 
            @RegistroId = @TransaccionId, 
            @Usuario = @Usuario, 
            @Datos = @BitacoraDescripcion;

        -- Confirmar la transacción
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        -- Revertir la transacción en caso de error
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        DECLARE @ErrorDescripcion nvarchar(max);
        SET @ErrorDescripcion = 'Error al revertir el pago. Detalles: ' + ERROR_MESSAGE();

        EXEC RegistrarBitacora 
            @Tabla = 'transacciones', 
            @Accion = 'error', 
            @RegistroId = @TransaccionId, 
            @Usuario = @Usuario, 
            @Datos = @ErrorDescripcion;
    END CATCH;
END;
GO


-- ============================================
-- INSERTAR CLIENTES
-- ============================================
INSERT INTO clientes (nombre, apellido_paterno, apellido_materno, activo)
VALUES 
('Juan', 'Pérez', 'Gómez', 1),
('María', 'López', 'Hernández', 1),
('Carlos', 'Ramírez', 'Martínez', 1),
('Ana', 'García', 'Sánchez', 1),
('Luis', 'Fernández', 'Torres', 1);

-- ============================================
-- INSERTAR TARJETAS DE CRÉDITO
-- ============================================
-- Cliente 1
INSERT INTO tarjetas_credito (cliente_id, numero_tarjeta, limite_credito, saldo_actual, activo)
VALUES 
(1, '1111222233334444', 5000, 0, 1),
(1, '1111222233335555', 7000, 0, 1);

-- Cliente 2
INSERT INTO tarjetas_credito (cliente_id, numero_tarjeta, limite_credito, saldo_actual, activo)
VALUES 
(2, '2222333344445555', 6000, 0, 1),
(2, '2222333344446666', 8000, 0, 1),
(2, '2222333344447777', 10000, 0, 1);

-- Cliente 3
INSERT INTO tarjetas_credito (cliente_id, numero_tarjeta, limite_credito, saldo_actual, activo)
VALUES 
(3, '3333444455556666', 4000, 0, 1),
(3, '3333444455557777', 6000, 0, 1);

-- Cliente 4
INSERT INTO tarjetas_credito (cliente_id, numero_tarjeta, limite_credito, saldo_actual, activo)
VALUES 
(4, '4444555566667777', 5000, 0, 1),
(4, '4444555566668888', 7000, 0, 1);

-- Cliente 5
INSERT INTO tarjetas_credito (cliente_id, numero_tarjeta, limite_credito, saldo_actual, activo)
VALUES 
(5, '5555666677778888', 9000, 0, 1),
(5, '5555666677779999', 11000, 0, 1),
(5, '5555666677770000', 13000, 0, 1);

-- ============================================
-- INSERTAR TIPOS DE TRANSACCIÓN
-- ============================================
INSERT INTO tipos_transaccion (nombre_tipo, activo)
VALUES 
('compra', 1),
('pago', 1);

-- ============================================
-- INSERTAR TIPOS DE TRANSACCIÓN
-- ============================================
INSERT INTO configuraciones_intereses (tasa_interes, tasa_pago_minimo, fecha_vigencia, activo)
VALUES (25, 5, '2999-12-12', 1);

-- ============================================
-- INSERTAR TRANSACCIONES (COMPRAS Y PAGOS)
-- ============================================
-- Generar transacciones para cada tarjeta
DECLARE @TarjetaId bigint;
DECLARE @Fecha date;
DECLARE @Monto decimal(10, 2);
DECLARE @Descripcion nvarchar(255);
DECLARE @TipoTransaccionId bigint;

DECLARE tarjeta_cursor CURSOR FOR
SELECT id FROM tarjetas_credito;

OPEN tarjeta_cursor;

FETCH NEXT FROM tarjeta_cursor INTO @TarjetaId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Generar entre 12 y 15 transacciones para los últimos 3 meses
    DECLARE @Mes int = 0;
    WHILE @Mes < 3
    BEGIN
        -- Generar entre 4 y 5 compras por mes
        DECLARE @Compras int = 0;
        WHILE @Compras < 14
        BEGIN
            SET @Fecha = DATEADD(MONTH, -@Mes, GETDATE()) - ABS(CHECKSUM(NEWID())) % 28; -- Fecha aleatoria del mes
            SET @Monto = ABS(CHECKSUM(NEWID())) % 500 + 50; -- Monto aleatorio entre 50 y 550
            SET @Descripcion = 'Compra en tienda ' + CAST(ABS(CHECKSUM(NEWID())) % 100 AS nvarchar);
            SET @TipoTransaccionId = (SELECT id FROM tipos_transaccion WHERE nombre_tipo = 'compra' AND activo = 1);

            EXEC AgregarTransaccion 
                @TarjetaId = @TarjetaId, 
                @FechaTransaccion = @Fecha, 
                @Descripcion = @Descripcion, 
                @Monto = @Monto, 
                @TipoTransaccionId = @TipoTransaccionId, 
                @Usuario = 'Sistema';

            SET @Compras = @Compras + 1;
        END;

        -- Generar 1 o 2 pagos por mes
        DECLARE @Pagos int = 0;
        WHILE @Pagos < 2
        BEGIN
             SET @Fecha = DATEADD(MONTH, -@Mes, GETDATE()) - ABS(CHECKSUM(NEWID())) % 28; -- Fecha aleatoria del mes
            SET @Monto = ABS(CHECKSUM(NEWID())) % 300 + 100; -- Monto aleatorio entre 100 y 400
            SET @Descripcion = 'Pago realizado';
            SET @TipoTransaccionId = (SELECT id FROM tipos_transaccion WHERE nombre_tipo = 'pago' AND activo = 1);

            EXEC AgregarTransaccion 
                @TarjetaId = @TarjetaId, 
                @FechaTransaccion = @Fecha, 
                @Descripcion = @Descripcion, 
                @Monto = @Monto, 
                @TipoTransaccionId = @TipoTransaccionId, 
                @Usuario = 'Sistema';

            SET @Pagos = @Pagos + 1;
        END;

        SET @Mes = @Mes + 1;
    END;

    FETCH NEXT FROM tarjeta_cursor INTO @TarjetaId;
END;

CLOSE tarjeta_cursor;
DEALLOCATE tarjeta_cursor;


