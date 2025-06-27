namespace AutoGestion.DTOs
{
    public class RegistrarAsistenciaInputDto
    {
        public int TurnoID { get; set; }
        public string Estado { get; set; } // "Asistió" | "No asistió" | "Pendiente"
        public string Observaciones { get; set; }
    }
}
