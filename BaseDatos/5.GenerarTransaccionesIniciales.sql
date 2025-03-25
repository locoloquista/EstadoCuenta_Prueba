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
