namespace AutoGestion.DTOs
{
    // Los datos que se muestran en el grid de comisiones pendientes
    public class VentaComisionDto
    {
        public int VentaID { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public string VehiculoResumen { get; set; }
        public decimal MontoVenta { get; set; }
        public decimal ComisionSugerida { get; set; }
        public string FechaVenta { get; set; }
    }
}
