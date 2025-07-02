using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    // DTO para listar las ofertas con evaluación y rango de tasación sugerido.
    public class TasacionListDto
    {
        // Oferta de compra que se está tasando.
        public int OfertaID { get; set; }

        //Resumen del vehículo: Marca, Modelo y Dominio
        public string VehiculoResumen { get; set; }
        public string EstadoMotor { get; set; }
        public string EstadoCarroceria { get; set; }
        public string EstadoInterior { get; set; }
        public string EstadoDocumentacion { get; set; }
        public int Kilometraje { get; set; }

        //Valor mínimo sugerido
        public decimal? RangoMin { get; set; }

        //Valor máximo sugerido
        public decimal? RangoMax { get; set; }

        // Mapea la entidad de oferta y su evaluación a este DTO,
        // incorporando opcionalmente el rango calculado.
        // o = Entidad OfertaCompra a mapear;
        // e = EvaluacionTecnica asociada a la oferta;

        public static TasacionListDto FromEntity(
            OfertaCompra o,
            EvaluacionTecnica e,
            (decimal Min, decimal Max)? rango)
        {
            if (o == null || e == null)
                return null;

            return new TasacionListDto
            {
                OfertaID = o.ID,
                VehiculoResumen = $"{o.Vehiculo.Marca} {o.Vehiculo.Modelo} ({o.Vehiculo.Dominio})",
                EstadoMotor = e.EstadoMotor,
                EstadoCarroceria = e.EstadoCarroceria,
                EstadoInterior = e.EstadoInterior,
                EstadoDocumentacion = e.EstadoDocumentacion,
                Kilometraje = o.Vehiculo.Km,
                RangoMin = rango?.Min,
                RangoMax = rango?.Max
            };
        }
    }
}
