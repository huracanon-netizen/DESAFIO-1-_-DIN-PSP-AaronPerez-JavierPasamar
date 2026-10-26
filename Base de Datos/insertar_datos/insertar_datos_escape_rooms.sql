USE escape_rooms;

INSERT INTO Tematica (nombre, descripcion) VALUES
('Terror', 'Ambientes oscuros y sustos'),
('Misterio', 'Casos detectivescos y enigmas'),
('Aventura', 'Exploración y búsqueda de tesoros'),
('Ciencia ficción', 'Naves, laboratorios y futuro');

INSERT INTO Sala (nombre, dificultad, duracion_max_min, min_jugadores, max_jugadores, precio, Id_Tematica) VALUES
('La Mansión Maldita', 'Dificil', 60, 3, 6, 18.00, 1),
('El Sótano', 'Media', 60, 2, 5, 16.00, 1),
('Asesinato en el Orient', 'Media', 60, 2, 6, 17.50, 2),
('El Detective', 'Facil', 45, 2, 4, 14.00, 2),
('El Templo Perdido', 'Media', 60, 2, 6, 16.50, 3),
('Estación Orbital', 'Dificil', 75, 3, 6, 20.00, 4);

INSERT INTO Cliente (dni, nombre, Email, Telefono) VALUES
('11111111A', 'Laura Gómez', 'laura.gomez@mail.com', '600111222'),
('22222222B', 'Carlos Pérez', 'carlos.perez@mail.com', '600222333'),
('33333333C', 'Marta Sanz', 'marta.sanz@mail.com', '600333444'),
('44444444D', 'Javier Ruiz', 'javier.ruiz@mail.com', '600444555');

INSERT INTO Monitor (dni, nombre, activo) VALUES
('77777777G', 'Sergio Blanco', 1),
('88888888H', 'Nuria Campos', 1),
('99999999J', 'Iván Molina', 0);

INSERT INTO Monitor_has_Sala (Monitor_idMonitor, Sala_idSala, nivel_certificacion, fecha_alta) VALUES
(1, 1, 'Experto', '2025-01-15'),
(1, 2, 'Intermedio', '2025-03-01'),
(2, 3, 'Experto', '2025-02-10'),
(2, 4, 'Basico', '2025-06-20'),
(3, 5, 'Intermedio', '2025-04-05'),
(2, 6, 'Experto', '2025-09-12');

INSERT INTO Reserva (fecha_hora, num_jugadores, pagada, Cliente_idCliente, Sala_idSala) VALUES
('2026-09-12 18:00:00', 4, 1, 1, 1),
('2026-09-13 17:00:00', 3, 1, 2, 3),
('2026-09-14 20:00:00', 5, 0, 3, 5),
('2026-09-19 19:00:00', 4, 1, 4, 6),
('2026-10-24 18:30:00', 6, 0, 1, 6);

-- Resultados de partidas ya procesadas por la Aplicación 2
INSERT INTO Resultado (id_solicitud, tiempo_segundos, pistas_usadas, puntuacion, clasificacion, estado, fecha_proceso, Reserva_idReserva) VALUES
('SOL-2026-001', 3240, 2, 780, 'Excelente', 'Procesada correctamente', '2026-09-12 19:05:00', 1),
('SOL-2026-002', 3590, 4, 520, 'Normal', 'Procesada correctamente', '2026-09-13 18:02:00', 2),
('SOL-2026-003', 3900, 1, 910, 'Récord', 'Procesada correctamente', '2026-09-19 20:20:00', 4);