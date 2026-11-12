using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public DateTime FechaHora { get; set; }

    public int NumJugadores { get; set; }

    public int Pagada { get; set; }

    public int ClienteIdCliente { get; set; }

    public int SalaIdSala { get; set; }

    public virtual Cliente ClienteIdClienteNavigation { get; set; } = null!;

    public virtual ICollection<Resultado> Resultados { get; set; } = new List<Resultado>();

    public virtual Sala SalaIdSalaNavigation { get; set; } = null!;
}
