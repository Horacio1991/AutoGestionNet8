namespace AutoGestion.Entidades
{
    [Serializable]
    public class Venta
    {
        public int ID { get; set; }
        // Vendedor que realiza la venta
        public Vendedor Vendedor { get; set; }
        // Cliente que compra el vehiculo
        public Cliente Cliente { get; set; }
        // Vehiculo vendido
        public Vehiculo Vehiculo { get; set; }
        // Pago asociado a la venta
        public Pago Pago { get; set; }
        public string Estado { get; set; } // "Pendiente", "Autorizada", "Rechazada"
        public DateTime Fecha { get; set; } = DateTime.Now;
        // Si pago es distinto de null se accede a este campo sino devuelve 0
        public decimal Total => Pago?.Monto ?? 0;
        public string MotivoRechazo { get; set; }

    }
}
