namespace AutoGestion.Entidades
{
    [Serializable]
    public class ComprobanteEntrega
    {
        public int ID { get; set; }
        public Venta Venta { get; set; } // Venta asociada al comprobante de entrega
        public DateTime FechaEntrega { get; set; } = DateTime.Now;
    }
}
