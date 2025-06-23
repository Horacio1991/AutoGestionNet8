namespace AutoGestion.Entidades
{
    [Serializable]
    public class Factura
    {
        public int ID { get; set; }
        // Cliente al que se le emite la factura
        public Cliente Cliente { get; set; }
        // Vehiculo asociado a la factura
        public Vehiculo Vehiculo { get; set; }
        public string FormaPago { get; set; }
        public decimal Precio { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}

