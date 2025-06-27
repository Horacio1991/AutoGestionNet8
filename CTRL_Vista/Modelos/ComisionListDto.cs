namespace AutoGestion.DTOs
{
    public class ComisionListDto
    {
        public int ID { get; set; }
        public string Fecha { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string MotivoRechazo { get; set; }

        public static ComisionListDto FromEntity(AutoGestion.Entidades.Comision c)
        {
            return new ComisionListDto
            {
                ID = c.ID,
                Fecha = c.Fecha.ToShortDateString(),
                Cliente = $"{c.Venta.Cliente.Nombre} {c.Venta.Cliente.Apellido}",
                Vehiculo = $"{c.Venta.Vehiculo.Marca} {c.Venta.Vehiculo.Modelo} ({c.Venta.Vehiculo.Dominio})",
                Monto = c.Monto,
                Estado = c.Estado,
                MotivoRechazo = c.MotivoRechazo
            };
        }
    }
}
