namespace AutoGestion.Entidades
{
    [Serializable]
    public class Tasacion
    {
        public int ID { get; set; }
        public OfertaCompra Oferta { get; set; } // Datos del oferente y del vehículo tasado
        public decimal ValorFinal { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
