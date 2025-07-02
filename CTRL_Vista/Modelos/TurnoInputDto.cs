namespace AutoGestion.DTOs
{
    // DTO para entrada de datos de un turno de asistencia
    public class TurnoInputDto
    {
        public string DniCliente { get; set; }
        public string DominioVehiculo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}
