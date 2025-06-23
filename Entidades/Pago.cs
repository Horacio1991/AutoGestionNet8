namespace AutoGestion.Entidades
{
    [Serializable]
    public class Pago
    {
        public int ID { get; set; }
        // Contado o Financiado
        public string TipoPago { get; set; } 
        public decimal Monto { get; set; }
        // Solo si es financiado
        public int Cuotas { get; set; } 
        public string Detalles { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
    }
}

