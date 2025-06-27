namespace AutoGestion.DTOs
{
    /// <summary> Datos para enviar al controller al registrar o rechazar una comisión. </summary>
    public class ComisionInputDto
    {
        public int VentaID { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }        // "Aprobada" o "Rechazada"
        public string MotivoRechazo { get; set; } // Sólo para rechazos
    }
}
