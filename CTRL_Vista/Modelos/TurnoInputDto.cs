﻿namespace AutoGestion.DTOs
{
    public class TurnoInputDto
    {
        public string DniCliente { get; set; }
        public string DominioVehiculo { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}
