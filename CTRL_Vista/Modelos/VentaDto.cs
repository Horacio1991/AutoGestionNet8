namespace AutoGestion.CTRL_Vista.Modelos
{
    public class VentaDto
    {
        public int ID { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }
        public string TipoPago { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string Fecha { get; set; }

        public static VentaDto FromEntity(Entidades.Venta v)
        {
            return new VentaDto
            {
                ID = v.ID,
                Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}",
                Vehiculo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo} ({v.Vehiculo?.Dominio})",
                TipoPago = v.Pago?.TipoPago ?? "N/A",
                Monto = v.Total,
                Estado = v.Estado,
                Fecha = v.Fecha.ToShortDateString()
            };
        }
    }
}
