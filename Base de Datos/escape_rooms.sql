DROP DATABASE IF EXISTS escape_rooms;
CREATE DATABASE escape_rooms;
USE escape_rooms;



CREATE TABLE Tematica (
  idTematica INT PRIMARY KEY AUTO_INCREMENT NOT NULL UNIQUE,
  nombre VARCHAR(50) NOT NULL UNIQUE,
  descripcion VARCHAR(200) NULL
);

CREATE TABLE Cliente (
  idCliente INT PRIMARY KEY AUTO_INCREMENT NOT NULL UNIQUE,
  dni VARCHAR(9) NOT NULL UNIQUE,
  nombre VARCHAR(100) NOT NULL,
  Email VARCHAR(100) NOT NULL UNIQUE,
  Telefono VARCHAR(20) NULL
);

CREATE TABLE Monitor (
  idMonitor INT PRIMARY KEY AUTO_INCREMENT NOT NULL UNIQUE,
  dni VARCHAR(9) NOT NULL UNIQUE,
  nombre VARCHAR(100) NOT NULL,
  activo INT NOT NULL DEFAULT 1
);


CREATE TABLE Sala (
  idSala INT PRIMARY KEY AUTO_INCREMENT NOT NULL UNIQUE,
  nombre VARCHAR(100) NOT NULL UNIQUE,
  dificultad VARCHAR(20) NOT NULL,
  duracion_max_min INT NOT NULL,
  min_jugadores INT NOT NULL,
  max_jugadores INT NOT NULL,
  precio DECIMAL(10,2) NOT NULL,
  Id_Tematica INT NULL,
  FOREIGN KEY (Id_Tematica) REFERENCES Tematica (idTematica)
    ON DELETE SET NULL ON UPDATE CASCADE
);


CREATE TABLE Monitor_has_Sala (
  Monitor_idMonitor INT NOT NULL,
  Sala_idSala INT NOT NULL,
  nivel_certificacion VARCHAR(20) NULL,
  fecha_alta DATE NULL,
  PRIMARY KEY (Monitor_idMonitor, Sala_idSala),
  FOREIGN KEY (Monitor_idMonitor) REFERENCES Monitor (idMonitor)
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (Sala_idSala) REFERENCES Sala (idSala)
    ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE Reserva (
  idReserva INT PRIMARY KEY AUTO_INCREMENT NOT NULL UNIQUE,
  fecha_hora DATETIME NOT NULL,
  num_jugadores INT NOT NULL,
  pagada INT NOT NULL DEFAULT 0,
  Cliente_idCliente INT NOT NULL,
  Sala_idSala INT NOT NULL,
  UNIQUE (Sala_idSala, fecha_hora),
  FOREIGN KEY (Cliente_idCliente) REFERENCES Cliente (idCliente)
    ON DELETE CASCADE ON UPDATE CASCADE,
  FOREIGN KEY (Sala_idSala) REFERENCES Sala (idSala)
    ON DELETE CASCADE ON UPDATE CASCADE
);


CREATE TABLE Resultado (
  idResultado INT PRIMARY KEY AUTO_INCREMENT NOT NULL UNIQUE,
  id_solicitud VARCHAR(20) NOT NULL UNIQUE,
  tiempo_segundos INT NULL,
  pistas_usadas INT NULL,
  puntuacion INT NULL,
  clasificacion VARCHAR(30) NULL,
  estado VARCHAR(50) NULL,
  fecha_proceso DATETIME NULL,
  Reserva_idReserva INT NOT NULL,
  FOREIGN KEY (Reserva_idReserva) REFERENCES Reserva (idReserva)
    ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE INDEX idx_reserva_fecha ON Reserva(fecha_hora);

CREATE VIEW vista_reservas_completas AS
SELECT r.idReserva, r.fecha_hora, r.num_jugadores, r.pagada,
       c.nombre AS cliente, s.nombre AS sala
FROM Reserva r
JOIN Cliente c ON r.Cliente_idCliente = c.idCliente
JOIN Sala    s ON r.Sala_idSala       = s.idSala;

CREATE VIEW vista_resultados_completos AS
SELECT res.id_solicitud, c.nombre AS cliente, s.nombre AS sala, r.fecha_hora,
       res.tiempo_segundos, res.pistas_usadas, res.puntuacion,
       res.clasificacion, res.estado, res.fecha_proceso
FROM Resultado res
JOIN Reserva r ON res.Reserva_idReserva = r.idReserva
JOIN Cliente c ON r.Cliente_idCliente   = c.idCliente
JOIN Sala    s ON r.Sala_idSala         = s.idSala;

DELIMITER //

CREATE TRIGGER trg_before_insert_sala
BEFORE INSERT ON Sala
FOR EACH ROW
BEGIN
    IF NEW.dificultad NOT IN ('Facil', 'Media', 'Dificil') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Dificultad no válida. Use: Facil, Media o Dificil';
    END IF;

    IF NEW.max_jugadores < NEW.min_jugadores THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El máximo de jugadores no puede ser menor que el mínimo';
    END IF;
END //

CREATE TRIGGER trg_before_update_sala
BEFORE UPDATE ON Sala
FOR EACH ROW
BEGIN
    IF NEW.dificultad NOT IN ('Facil', 'Media', 'Dificil') THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Dificultad no válida. Use: Facil, Media o Dificil';
    END IF;

    IF NEW.max_jugadores < NEW.min_jugadores THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El máximo de jugadores no puede ser menor que el mínimo';
    END IF;
END //

CREATE TRIGGER trg_before_insert_reserva
BEFORE INSERT ON Reserva
FOR EACH ROW
BEGIN
    DECLARE v_min INT;
    DECLARE v_max INT;

    SELECT min_jugadores, max_jugadores INTO v_min, v_max
    FROM Sala WHERE idSala = NEW.Sala_idSala;

    IF NEW.num_jugadores < v_min OR NEW.num_jugadores > v_max THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Número de jugadores fuera del rango de la sala';
    END IF;
END //

CREATE TRIGGER trg_before_update_reserva
BEFORE UPDATE ON Reserva
FOR EACH ROW
BEGIN
    DECLARE v_min INT;
    DECLARE v_max INT;

    SELECT min_jugadores, max_jugadores INTO v_min, v_max
    FROM Sala WHERE idSala = NEW.Sala_idSala;

    IF NEW.num_jugadores < v_min OR NEW.num_jugadores > v_max THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Número de jugadores fuera del rango de la sala';
    END IF;
END //

CREATE TRIGGER trg_before_insert_resultado
BEFORE INSERT ON Resultado
FOR EACH ROW
BEGIN
    DECLARE v_duracion INT;

    SELECT s.duracion_max_min INTO v_duracion
    FROM Reserva r
    JOIN Sala s ON r.Sala_idSala = s.idSala
    WHERE r.idReserva = NEW.Reserva_idReserva;

    IF NEW.tiempo_segundos <= 0 OR NEW.pistas_usadas < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Tiempo o pistas no válidos';
    END IF;

    IF NEW.tiempo_segundos > v_duracion * 60 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El tiempo supera la duración máxima de la sala';
    END IF;

    IF NEW.fecha_proceso IS NULL THEN
        SET NEW.fecha_proceso = NOW();
    END IF;
END //

DELIMITER ;