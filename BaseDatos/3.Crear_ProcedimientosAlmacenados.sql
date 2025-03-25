-- ============================================
-- PROCEDIMIENTOS ALMACENADOS
-- ============================================



-- ============================================
-- Procedimiento para registrar en la bitácora
-- ============================================
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







-- ============================================
-- Procedimiento para obtener el estado de cuenta
-- ============================================
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







-- ============================================
-- Procedimiento para obtener informacion de clientes
-- ============================================
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







-- ============================================
-- Procedimiento para obtener informacion de clientes por ID
-- ============================================
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







-- ============================================
--Obtener informacion de tarjetas por cliente
-- ============================================
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







-- ============================================
-- Procedimiento para obtener transacciones realizadas por tarjeta en el mes
-- ============================================
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
			t.tarjeta_id,
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
			p.tarjeta_id,
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







-- ============================================
--Agregar transaccion
-- ============================================
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

		return 1;
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

		return 0;
    END CATCH;
END;
GO







-- ============================================
-- Procedimiento para revertir una compra
-- ============================================
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







-- ============================================
-- Procedimiento para revertir un pago
-- ============================================
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