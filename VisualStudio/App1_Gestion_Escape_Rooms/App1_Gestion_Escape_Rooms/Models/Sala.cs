using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class Sala
{
    public int IdSala { get; set; }

    public string Nombre { get; set; } = null!;

    public string Dificultad { get; set; } = null!;

    public int DuracionMaxMin { get; set; }

    public int MinJugadores { get; set; }

    public int MaxJugadores { get; set; }

    public decimal Precio { get; set; }

    public int? IdTematica { get; set; }

    public virtual Tematica? IdTematicaNavigation { get; set; }

    public virtual ICollection<MonitorHasSala> MonitorHasSalas { get; set; } = new List<MonitorHasSala>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
