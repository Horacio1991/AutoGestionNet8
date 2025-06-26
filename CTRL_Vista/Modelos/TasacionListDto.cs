namespace AutoGestion.DTOs
{
    public class TasacionListDto
    {
        public int OfertaID { get; set; }
        public string VehiculoResumen { get; set; }

        // Componentes de la evaluación técnica
        public string EstadoMotor { get; set; }
        public string EstadoCarroceria { get; set; }
        public string EstadoInterior { get; set; }
        public string EstadoDocumentacion { get; set; }

        public int Kilometraje { get; set; }

        // Rango opcional
        public decimal? RangoMin { get; set; }
        public decimal? RangoMax { get; set; }

        public static TasacionListDto FromEntity(
            Entidades.OfertaCompra o,
            Entidades.EvaluacionTecnica e,
            (decimal Min, decimal Max)? rango)
        {
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
