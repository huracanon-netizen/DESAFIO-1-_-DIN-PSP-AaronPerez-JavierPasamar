namespace App2_Procesamiento
{
    public class PeticionRegistro
    {
        public string IdPeticion { get; set; } = "";
        public DateTime FechaHoraRecepcion { get; set; }
        public string TipoOperacion { get; set; } = "";
        public string EstadoFinal { get; set; } = "";
        public string? ErrorTecnico { get; set; }
        public string? CampoError { get; set; }
        public string? Detalle { get; set; }
        public int? Puntuacion { get; set; }
        public string? Clasificacion { get; set; }
        public string? JsonRecibido { get; set; }
    }
}