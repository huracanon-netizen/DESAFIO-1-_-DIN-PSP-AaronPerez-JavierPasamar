namespace App1_Gestion_Escape_Rooms
{
    public class Respuesta
    {
        public string idSolicitud { get; set; } = "";
        public string estado { get; set; } = "";
        public string? campoError { get; set; }
        public string? detalle { get; set; }
        public int? puntuacion { get; set; }
        public string? clasificacion { get; set; }
    }
}