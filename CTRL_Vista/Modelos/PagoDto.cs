namespace AutoGestion.DTOs
{
    public class PagoDto
    {
        public string TipoPago { get; set; } // Tipo de pago: "Efectivo", "Tarjeta", "Transferencia", "Financiado", etc.
        public decimal Monto { get; set; }
        public int Cuotas { get; set; }
        public string Detalles { get; set; }
    }
}
