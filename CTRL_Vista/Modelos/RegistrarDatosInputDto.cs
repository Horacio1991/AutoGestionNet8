namespace AutoGestion.DTOs
{
    public class RegistrarDatosInputDto
    {
        public int OfertaID { get; set; }
        public string EstadoStock { get; set; } // Disponible, requiere reacondiconamiento
    }
}
