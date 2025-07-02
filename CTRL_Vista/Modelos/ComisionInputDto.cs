namespace AutoGestion.DTOs
{
    /// DTO de entrada para registrar o rechazar una comisión.
    public class ComisionInputDto
    {
        /// ID de la venta a la que se le va a asociar la comisión.
        public int VentaID { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } //Aprbada o Rechazada
        public string MotivoRechazo { get; set; }
    }
}
