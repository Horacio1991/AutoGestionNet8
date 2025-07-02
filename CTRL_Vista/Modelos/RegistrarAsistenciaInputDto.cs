namespace AutoGestion.DTOs
{
    // DTO de entrada para registrar la asistencia de un turno.
    public class RegistrarAsistenciaInputDto
    {
        /// ID del turno al que se le registra asistencia.
        public int TurnoID { get; set; }
        public string Estado { get; set; } // Asistió, pendiente o, no asistió

        //Observaciones opcionales sobre la asistencia.
        public string Observaciones { get; set; }
    }
}
