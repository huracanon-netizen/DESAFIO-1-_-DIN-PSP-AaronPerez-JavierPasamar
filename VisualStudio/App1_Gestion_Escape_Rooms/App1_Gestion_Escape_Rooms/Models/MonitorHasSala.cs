using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class MonitorHasSala
{
    public int MonitorIdMonitor { get; set; }

    public int SalaIdSala { get; set; }

    public string? NivelCertificacion { get; set; }

    public DateTime? FechaAlta { get; set; }

    public virtual Monitor MonitorIdMonitorNavigation { get; set; } = null!;

    public virtual Sala SalaIdSalaNavigation { get; set; } = null!;
}
