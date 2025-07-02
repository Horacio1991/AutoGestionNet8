namespace AutoGestion.DTOs
{
    public class DashboardVentaDto
    {
        public string Fecha { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }

        // Monto total facturado en esa venta.
        public decimal Total { get; set; }
    }
}
