namespace AutoGestion.Entidades
{
    [Serializable]
    public class Venta
    {
        public int ID { get; set; }
        public Vendedor Vendedor { get; set; }
        public Cliente Cliente { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public Pago Pago { get; set; }
        public string Estado { get; set; } // "Pendiente", "Autorizada", "Rechazada"
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total => Pago?.Monto ?? 0; // Si pago es distinto de null se accede a este campo sino devuelve 0
        public string MotivoRechazo { get; set; }

    }
}
