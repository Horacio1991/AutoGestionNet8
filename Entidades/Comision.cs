namespace AutoGestion.Entidades
{
    [Serializable]
    public class Comision
    {
        public int ID { get; set; }   
        public Venta Venta { get; set; } // Venta asociada a la comisión
        public decimal Porcentaje { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } // Aprobada o Rechazada
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string MotivoRechazo { get; set; } // Fecha y hora en la que se registró la comisión

    }

}
