namespace AutoGestion.DTOs
{
    public class ComisionInputDto
    {
        public int VentaID { get; set; }
        public decimal Monto { get; set; }              // aquí va Monto
        public string Estado { get; set; }              // "Aprobada" o "Rechazada"
        public string MotivoRechazo { get; set; }       // si Estado == "Rechazada"
    }
}
