namespace AutoGestion.Entidades
{
    [Serializable]
    public class Turno
    {
        public int ID { get; set; }
        // Cliuene que solicita el turno
        public Cliente Cliente { get; set; }
        // Vehiculo que se atiende en el turno
        public Vehiculo Vehiculo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Asistencia { get; set; } // Pendiente / Asistió / No asistió
        public string Observaciones { get; set; }


    }
}
