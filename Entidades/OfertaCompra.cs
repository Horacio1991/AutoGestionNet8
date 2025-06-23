namespace AutoGestion.Entidades
{
    [Serializable]
    // Representa una oferta de compra de un vehículo por parte de un oferente
    public class OfertaCompra
    {
        public int ID { get; set; }
        // Datos del oferente que realiza la oferta
        public Oferente Oferente { get; set; }
        // Vehiculo que se ofrece comprar
        public Vehiculo Vehiculo { get; set; }
        public DateTime FechaInspeccion { get; set; }
    }
}
