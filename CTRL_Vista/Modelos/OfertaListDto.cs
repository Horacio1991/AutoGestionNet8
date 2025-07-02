using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    public class OfertaListDto
    {
        public int ID { get; set; }

        // Resumen del vehículo: Marca, Modelo y Dominio.
        public string VehiculoResumen { get; set; }

        public DateTime FechaInspeccion { get; set; }

        // Mapea una entidad OfertaCompra a OfertaListDto para la UI.
        // o = Entidad OfertaCompra a mapear;
        public static OfertaListDto FromEntity(OfertaCompra o)
        {
            if (o == null) return null;

            return new OfertaListDto
            {
                ID = o.ID,
                VehiculoResumen = $"{o.Vehiculo.Marca} {o.Vehiculo.Modelo} ({o.Vehiculo.Dominio})",
                FechaInspeccion = o.FechaInspeccion
            };
        }
    }
}
