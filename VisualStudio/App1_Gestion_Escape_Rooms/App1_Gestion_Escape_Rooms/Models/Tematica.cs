using System;
using System.Collections.Generic;

namespace App1_Gestion_Escape_Rooms.Models;

public partial class Tematica
{
    public int IdTematica { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<Sala> Salas { get; set; } = new List<Sala>();
}
