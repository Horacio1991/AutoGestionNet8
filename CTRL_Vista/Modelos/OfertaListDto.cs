namespace AutoGestion.DTOs
{
    public class OfertaListDto
    {
        public int ID { get; set; }
        public string VehiculoResumen { get; set; }
        public DateTime FechaInspeccion { get; set; }

        public static OfertaListDto FromEntity(Entidades.OfertaCompra o) => new()
        {
            ID = o.ID,
            VehiculoResumen = $"{o.Vehiculo.Marca} {o.Vehiculo.Modelo} ({o.Vehiculo.Dominio})",
            FechaInspeccion = o.FechaInspeccion
        };
    }
}
