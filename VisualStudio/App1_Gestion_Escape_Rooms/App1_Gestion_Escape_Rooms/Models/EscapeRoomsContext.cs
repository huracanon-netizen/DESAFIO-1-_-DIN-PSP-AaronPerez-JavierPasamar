using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class EscapeRoomsContext : DbContext
{
    public EscapeRoomsContext(DbContextOptions<EscapeRoomsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Monitor> Monitors { get; set; }

    public virtual DbSet<MonitorHasSala> MonitorHasSalas { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<Resultado> Resultados { get; set; }

    public virtual DbSet<Sala> Salas { get; set; }

    public virtual DbSet<Tematica> Tematicas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PRIMARY");

            entity.ToTable("cliente");

            entity.HasIndex(e => e.Email, "Email").IsUnique();

            entity.HasIndex(e => e.Dni, "dni").IsUnique();

            entity.Property(e => e.IdCliente)
                .HasColumnType("int(11)")
                .HasColumnName("idCliente");
            entity.Property(e => e.Dni)
                .HasMaxLength(9)
                .HasColumnName("dni");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Monitor>(entity =>
        {
            entity.HasKey(e => e.IdMonitor).HasName("PRIMARY");

            entity.ToTable("monitor");

            entity.HasIndex(e => e.Dni, "dni").IsUnique();

            entity.Property(e => e.IdMonitor)
                .HasColumnType("int(11)")
                .HasColumnName("idMonitor");
            entity.Property(e => e.Activo)
                .HasDefaultValueSql("'1'")
                .HasColumnType("int(11)")
                .HasColumnName("activo");
            entity.Property(e => e.Dni)
                .HasMaxLength(9)
                .HasColumnName("dni");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<MonitorHasSala>(entity =>
        {
            entity.HasKey(e => new { e.MonitorIdMonitor, e.SalaIdSala })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("monitor_has_sala");

            entity.HasIndex(e => e.SalaIdSala, "Sala_idSala");

            entity.Property(e => e.MonitorIdMonitor)
                .HasColumnType("int(11)")
                .HasColumnName("Monitor_idMonitor");
            entity.Property(e => e.SalaIdSala)
                .HasColumnType("int(11)")
                .HasColumnName("Sala_idSala");
            entity.Property(e => e.FechaAlta).HasColumnName("fecha_alta");
            entity.Property(e => e.NivelCertificacion)
                .HasMaxLength(20)
                .HasColumnName("nivel_certificacion");

            entity.HasOne(d => d.MonitorIdMonitorNavigation).WithMany(p => p.MonitorHasSalas)
                .HasForeignKey(d => d.MonitorIdMonitor)
                .HasConstraintName("monitor_has_sala_ibfk_1");

            entity.HasOne(d => d.SalaIdSalaNavigation).WithMany(p => p.MonitorHasSalas)
                .HasForeignKey(d => d.SalaIdSala)
                .HasConstraintName("monitor_has_sala_ibfk_2");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PRIMARY");

            entity.ToTable("reserva");

            entity.HasIndex(e => e.ClienteIdCliente, "Cliente_idCliente");

            entity.HasIndex(e => new { e.SalaIdSala, e.FechaHora }, "Sala_idSala").IsUnique();

            entity.HasIndex(e => e.FechaHora, "idx_reserva_fecha");

            entity.Property(e => e.IdReserva)
                .HasColumnType("int(11)")
                .HasColumnName("idReserva");
            entity.Property(e => e.ClienteIdCliente)
                .HasColumnType("int(11)")
                .HasColumnName("Cliente_idCliente");
            entity.Property(e => e.FechaHora)
                .HasColumnType("datetime")
                .HasColumnName("fecha_hora");
            entity.Property(e => e.NumJugadores)
                .HasColumnType("int(11)")
                .HasColumnName("num_jugadores");
            entity.Property(e => e.Pagada)
                .HasColumnType("int(11)")
                .HasColumnName("pagada");
            entity.Property(e => e.SalaIdSala)
                .HasColumnType("int(11)")
                .HasColumnName("Sala_idSala");

            entity.HasOne(d => d.ClienteIdClienteNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.ClienteIdCliente)
                .HasConstraintName("reserva_ibfk_1");

            entity.HasOne(d => d.SalaIdSalaNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.SalaIdSala)
                .HasConstraintName("reserva_ibfk_2");
        });

        modelBuilder.Entity<Resultado>(entity =>
        {
            entity.HasKey(e => e.IdResultado).HasName("PRIMARY");

            entity.ToTable("resultado");

            entity.HasIndex(e => e.ReservaIdReserva, "Reserva_idReserva");

            entity.HasIndex(e => e.IdSolicitud, "id_solicitud").IsUnique();

            entity.Property(e => e.IdResultado)
                .HasColumnType("int(11)")
                .HasColumnName("idResultado");
            entity.Property(e => e.Clasificacion)
                .HasMaxLength(30)
                .HasColumnName("clasificacion");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .HasColumnName("estado");
            entity.Property(e => e.FechaProceso)
                .HasColumnType("datetime")
                .HasColumnName("fecha_proceso");
            entity.Property(e => e.IdSolicitud)
                .HasMaxLength(20)
                .HasColumnName("id_solicitud");
            entity.Property(e => e.PistasUsadas)
                .HasColumnType("int(11)")
                .HasColumnName("pistas_usadas");
            entity.Property(e => e.Puntuacion)
                .HasColumnType("int(11)")
                .HasColumnName("puntuacion");
            entity.Property(e => e.ReservaIdReserva)
                .HasColumnType("int(11)")
                .HasColumnName("Reserva_idReserva");
            entity.Property(e => e.TiempoSegundos)
                .HasColumnType("int(11)")
                .HasColumnName("tiempo_segundos");

            entity.HasOne(d => d.ReservaIdReservaNavigation).WithMany(p => p.Resultados)
                .HasForeignKey(d => d.ReservaIdReserva)
                .HasConstraintName("resultado_ibfk_1");
        });

        modelBuilder.Entity<Sala>(entity =>
        {
            entity.HasKey(e => e.IdSala).HasName("PRIMARY");

            entity.ToTable("sala");

            entity.HasIndex(e => e.IdTematica, "Id_Tematica");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdSala)
                .HasColumnType("int(11)")
                .HasColumnName("idSala");
            entity.Property(e => e.Dificultad)
                .HasMaxLength(20)
                .HasColumnName("dificultad");
            entity.Property(e => e.DuracionMaxMin)
                .HasColumnType("int(11)")
                .HasColumnName("duracion_max_min");
            entity.Property(e => e.IdTematica)
                .HasColumnType("int(11)")
                .HasColumnName("Id_Tematica");
            entity.Property(e => e.MaxJugadores)
                .HasColumnType("int(11)")
                .HasColumnName("max_jugadores");
            entity.Property(e => e.MinJugadores)
                .HasColumnType("int(11)")
                .HasColumnName("min_jugadores");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.Precio)
                .HasPrecision(10, 2)
                .HasColumnName("precio");

            entity.HasOne(d => d.IdTematicaNavigation).WithMany(p => p.Salas)
                .HasForeignKey(d => d.IdTematica)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("sala_ibfk_1");
        });

        modelBuilder.Entity<Tematica>(entity =>
        {
            entity.HasKey(e => e.IdTematica).HasName("PRIMARY");

            entity.ToTable("tematica");

            entity.HasIndex(e => e.Nombre, "nombre").IsUnique();

            entity.Property(e => e.IdTematica)
                .HasColumnType("int(11)")
                .HasColumnName("idTematica");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
