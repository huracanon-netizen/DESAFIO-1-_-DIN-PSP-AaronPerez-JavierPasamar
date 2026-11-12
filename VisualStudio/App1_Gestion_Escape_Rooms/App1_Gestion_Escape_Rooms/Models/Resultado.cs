using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class Resultado
{
    public int IdResultado { get; set; }

    public string IdSolicitud { get; set; } = null!;

    public int? TiempoSegundos { get; set; }

    public int? PistasUsadas { get; set; }

    public int? Puntuacion { get; set; }

    public string? Clasificacion { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaProceso { get; set; }

    public int ReservaIdReserva { get; set; }

    public virtual Reserva ReservaIdReservaNavigation { get; set; } = null!;
}
