using System;

namespace AutoGestion.Vista.Modelos
{
    public class TurnoVista
    {
        public int ID { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Asistencia { get; set; }
    }
}
