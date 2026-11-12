using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class VistaResultadosCompleto
{
    public string IdSolicitud { get; set; } = null!;

    public string Cliente { get; set; } = null!;

    public string Sala { get; set; } = null!;

    public DateTime FechaHora { get; set; }

    public int? TiempoSegundos { get; set; }

    public int? PistasUsadas { get; set; }

    public int? Puntuacion { get; set; }

    public string? Clasificacion { get; set; }

    public string? Estado { get; set; }

    public DateTime? FechaProceso { get; set; }
}
