namespace AutoGestion.Vista.Modelos
{
    // Para presentar en la interfaz la información de las ventas y sus comisiones sugeridas
    public class VentaComisionVista
    {
        public int ID { get; set; }
        public string Cliente { get; set; }
        public string Vendedor { get; set; }
        public string Vehiculo { get; set; }
        public decimal MontoVenta { get; set; }
        public decimal ComisionSugerida { get; set; }
        public string Fecha { get; set; }
    }
}
