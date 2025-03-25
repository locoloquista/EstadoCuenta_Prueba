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
-- INSERTAR intereses
-- ============================================
INSERT INTO configuraciones_intereses (tasa_interes, tasa_pago_minimo, fecha_vigencia, activo)
VALUES (25, 5, '2999-12-12', 1);