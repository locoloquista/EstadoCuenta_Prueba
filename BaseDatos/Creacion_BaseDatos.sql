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

-- Tabla de Pagos
CREATE TABLE pagos (
    id bigint PRIMARY KEY IDENTITY(1,1),
    tarjeta_id bigint NOT NULL FOREIGN KEY REFERENCES tarjetas_credito(id),
    fecha_pago date NOT NULL,
    monto decimal(10, 2) NOT NULL,
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
CREATE PROCEDURE ObtenerEstadoCuenta
    @TarjetaId bigint
AS
BEGIN
    SELECT 
        tc.numero_tarjeta,
        CONCAT(c.nombre, ' ', c.apellido_paterno, ' ', ISNULL(c.apellido_materno, '')) AS nombre_cliente,
        tc.saldo_actual AS saldo_total,
        tc.limite_credito,
        (tc.limite_credito - tc.saldo_actual) AS saldo_disponible,
        SUM(CASE WHEN t.fecha_transaccion >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) THEN t.monto ELSE 0 END) AS compras_mes_actual,
        SUM(CASE WHEN t.fecha_transaccion >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0) AND t.fecha_transaccion < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0) THEN t.monto ELSE 0 END) AS compras_mes_anterior,
        ci.tasa_interes,
        (tc.saldo_actual * ci.tasa_interes / 100) AS interes_bonificable,
        (tc.saldo_actual * ci.tasa_pago_minimo / 100) AS cuota_minima,
        (tc.saldo_actual + (tc.saldo_actual * ci.tasa_interes / 100)) AS pago_contado_con_intereses
    FROM tarjetas_credito tc
    INNER JOIN clientes c ON tc.cliente_id = c.id
    LEFT JOIN transacciones t ON tc.id = t.tarjeta_id AND t.activo = 1
    CROSS JOIN configuraciones_intereses ci
    WHERE tc.id = @TarjetaId AND tc.activo = 1 AND c.activo = 1 AND ci.activo = 1
    GROUP BY tc.numero_tarjeta, c.nombre, c.apellido_paterno, c.apellido_materno, tc.saldo_actual, tc.limite_credito, ci.tasa_interes, ci.tasa_pago_minimo;
END;

-- Procedimiento para obtener transacciones realizadas por tarjeta en el mes
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
            p.fecha_pago AS Fecha,
            'Pago realizado' AS Descripcion,
            p.monto AS Monto
        FROM pagos p
        WHERE p.tarjeta_id = @TarjetaId
          AND p.activo = 1
          AND MONTH(p.fecha_pago) = MONTH(GETDATE())
          AND YEAR(p.fecha_pago) = YEAR(GETDATE())
        
        ORDER BY Fecha DESC;
    END TRY
    BEGIN CATCH
        -- Registrar el error en la bitácora
        DECLARE @ErrorMessage nvarchar(MAX) = ERROR_MESSAGE();
        EXEC RegistrarBitacora 'transacciones/pagos', 'error', @TarjetaId, 'Sistema', @ErrorMessage;
    END CATCH;
END;


-- Procedimiento para agregar un cliente
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

-- Procedimiento para agregar una compra
CREATE PROCEDURE AgregarCompra
    @TarjetaId bigint,
    @FechaTransaccion date,
    @Descripcion nvarchar(255),
    @Monto decimal(10, 2),
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insertar la transacción
        INSERT INTO transacciones (tarjeta_id, fecha_transaccion, descripcion, monto, tipo_transaccion_id, activo)
        VALUES (@TarjetaId, @FechaTransaccion, @Descripcion, @Monto, 
                (SELECT id FROM tipos_transaccion WHERE nombre_tipo = 'compra' AND activo = 1), 1);

        -- Actualizar el saldo de la tarjeta (restar el monto)
        UPDATE tarjetas_credito
        SET saldo_actual = saldo_actual + @Monto
        WHERE id = @TarjetaId;

        DECLARE @TransaccionId bigint = SCOPE_IDENTITY();

        -- Registrar en la bitácora
        DECLARE @BitacoraDescripcion nvarchar(max);
        SET @BitacoraDescripcion = 'TarjetaId: ' + CAST(@TarjetaId AS nvarchar) + 
                                   ', Fecha: ' + CAST(@FechaTransaccion AS nvarchar) + 
                                   ', Monto: ' + CAST(@Monto AS nvarchar);

        EXEC RegistrarBitacora 'transacciones', 'insertar', @TransaccionId, @Usuario, @BitacoraDescripcion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        DECLARE @ErrorMensaje nvarchar(max);
        SET @ErrorMensaje = ERROR_MESSAGE();

        EXEC RegistrarBitacora 'transacciones', 'error', 0, @Usuario, @ErrorMensaje;
    END CATCH;
END;

-- Procedimiento para agregar un pago
CREATE PROCEDURE AgregarPago
    @TarjetaId bigint,
    @FechaPago date,
    @Monto decimal(10, 2),
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Insertar el pago
        INSERT INTO pagos (tarjeta_id, fecha_pago, monto, activo)
        VALUES (@TarjetaId, @FechaPago, @Monto, 1);

        -- Actualizar el saldo de la tarjeta (sumar el monto)
        UPDATE tarjetas_credito
        SET saldo_actual = saldo_actual - @Monto
        WHERE id = @TarjetaId;

        DECLARE @PagoId bigint = SCOPE_IDENTITY();

        -- Registrar en la bitácora
        DECLARE @BitacoraDescripcion nvarchar(max);
        SET @BitacoraDescripcion = 'TarjetaId: ' + CAST(@TarjetaId AS nvarchar) + 
                                   ', Fecha: ' + CAST(@FechaPago AS nvarchar) + 
                                   ', Monto: ' + CAST(@Monto AS nvarchar);

        EXEC RegistrarBitacora 'pagos', 'insertar', @PagoId, @Usuario, @BitacoraDescripcion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        DECLARE @ErrorMensaje nvarchar(max);
        SET @ErrorMensaje = ERROR_MESSAGE();

        EXEC RegistrarBitacora 'pagos', 'error', 0, @Usuario, @ErrorMensaje;
    END CATCH;
END;

-- Procedimiento para revertir una compra
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
CREATE PROCEDURE RevertirPago
    @PagoId bigint,
    @Usuario nvarchar(255)
AS
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- Obtener el monto y la tarjeta asociada al pago
        DECLARE @Monto decimal(10, 2);
        DECLARE @TarjetaId bigint;

        SELECT @Monto = monto, @TarjetaId = tarjeta_id
        FROM pagos
        WHERE id = @PagoId AND activo = 1;

        -- Marcar el pago como inactivo
        UPDATE pagos
        SET activo = 0
        WHERE id = @PagoId;

        -- Actualizar el saldo de la tarjeta (sumar el monto)
        UPDATE tarjetas_credito
        SET saldo_actual = saldo_actual + @Monto
        WHERE id = @TarjetaId;

        -- Registrar en la bitácora
        DECLARE @BitacoraDescripcion nvarchar(max);
        SET @BitacoraDescripcion = 'PagoId: ' + CAST(@PagoId AS nvarchar) + 
                                   ', Monto: ' + CAST(@Monto AS nvarchar);

        EXEC RegistrarBitacora 'pagos', 'revertir', @PagoId, @Usuario, @BitacoraDescripcion;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        -- Registrar el error en la bitácora
        DECLARE @ErrorMensaje nvarchar(max);
        SET @ErrorMensaje = ERROR_MESSAGE();

        EXEC RegistrarBitacora 'pagos', 'error', @PagoId, @Usuario, @ErrorMensaje;
    END CATCH;
END;


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
-- INSERTAR TRANSACCIONES (COMPRAS Y PAGOS)
-- ============================================
DECLARE @TarjetaId bigint;
DECLARE @Fecha date;
DECLARE @Monto decimal(10, 2);
DECLARE @Descripcion nvarchar(255);

-- Generar transacciones para cada tarjeta
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
        -- Generar entre 10 y 12 compras por mes
        DECLARE @Compras int = 0;
        WHILE @Compras < 12
        BEGIN
            SET @Fecha = DATEADD(MONTH, -@Mes, GETDATE()) - ABS(CHECKSUM(NEWID())) % 28; -- Fecha aleatoria del mes
            SET @Monto = ABS(CHECKSUM(NEWID())) % 500 + 50; -- Monto aleatorio entre 50 y 550
            SET @Descripcion = 'Compra en tienda ' + CAST(ABS(CHECKSUM(NEWID())) % 100 AS nvarchar);

            INSERT INTO transacciones (tarjeta_id, fecha_transaccion, descripcion, monto, tipo_transaccion_id, activo)
            VALUES (@TarjetaId, @Fecha, @Descripcion, @Monto, 
                    (SELECT id FROM tipos_transaccion WHERE nombre_tipo = 'compra'), 1);

            SET @Compras = @Compras + 1;
        END;

        -- Generar 1 o 2 pagos por mes
        DECLARE @Pagos int = 0;
        WHILE @Pagos < 2
        BEGIN
            SET @Fecha = DATEADD(MONTH, -@Mes, GETDATE()) - ABS(CHECKSUM(NEWID())) % 28; -- Fecha aleatoria del mes
            SET @Monto = ABS(CHECKSUM(NEWID())) % 300 + 100; -- Monto aleatorio entre 100 y 400

            INSERT INTO pagos (tarjeta_id, fecha_pago, monto, activo)
            VALUES (@TarjetaId, @Fecha, @Monto, 1);

            SET @Pagos = @Pagos + 1;
        END;

        SET @Mes = @Mes + 1;
    END;

    FETCH NEXT FROM tarjeta_cursor INTO @TarjetaId;
END;

CLOSE tarjeta_cursor;
DEALLOCATE tarjeta_cursor;