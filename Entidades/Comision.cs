namespace AutoGestion.Entidades
{
    [Serializable]
    public class Comision
    {
        public int ID { get; set; }
        // Venta asociada a la comisión
        public Venta Venta { get; set; }
        // Porcentaje de la venta que corresponde a la comisión
        public decimal Porcentaje { get; set; }
        // Monto total de la comisión calculado a partir del porcentaje y el monto de la venta
        public decimal Monto { get; set; }
        // Aprobada o Rechazada
        public string Estado { get; set; }
        // Fecha y hora en la que se registró la comisión
        public DateTime Fecha { get; set; } = DateTime.Now;

        public string MotivoRechazo { get; set; } 
    }

}
