using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class Monitor
{
    public int IdMonitor { get; set; }

    public string Dni { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public int Activo { get; set; }

    public virtual ICollection<MonitorHasSala> MonitorHasSalas { get; set; } = new List<MonitorHasSala>();
}
