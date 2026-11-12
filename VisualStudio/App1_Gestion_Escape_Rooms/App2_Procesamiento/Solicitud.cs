namespace App2_Procesamiento
{
    public class Solicitud
    {
        public string idSolicitud { get; set; } = "";
        public ReservaDatos reserva { get; set; } = new ReservaDatos();
        public PartidaDatos partida { get; set; } = new PartidaDatos();
    }
}