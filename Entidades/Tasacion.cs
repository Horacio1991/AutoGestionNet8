namespace AutoGestion.Entidades
{
    [Serializable]
    public class Tasacion
    {
        public int ID { get; set; }
        // Datos del oferente y del vehículo tasado
        public OfertaCompra Oferta { get; set; }
        // Valor final de la tasación, despues de los ajustes manuales o automaticos
        public decimal ValorFinal { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
