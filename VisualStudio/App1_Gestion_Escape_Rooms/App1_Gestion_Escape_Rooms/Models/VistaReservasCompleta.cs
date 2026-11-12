using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class VistaReservasCompleta
{
    public int IdReserva { get; set; }

    public DateTime FechaHora { get; set; }

    public int NumJugadores { get; set; }

    public int Pagada { get; set; }

    public string Cliente { get; set; } = null!;

    public string Sala { get; set; } = null!;
}
