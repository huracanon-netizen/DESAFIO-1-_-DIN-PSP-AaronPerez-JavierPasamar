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
  fecha_alta DATETIME NULL,
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
