namespace App1_Gestion_Escape_Rooms
{
    public class Solicitud
    {
        public string idSolicitud { get; set; } = "";
        public ReservaDatos reserva { get; set; } = new ReservaDatos();
        public PartidaDatos partida { get; set; } = new PartidaDatos();
    }
}