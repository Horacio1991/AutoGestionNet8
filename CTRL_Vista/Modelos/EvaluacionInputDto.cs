namespace AutoGestion.DTOs
{
    public class EvaluacionInputDto
    {
        // ID de la oferta a la que se asocia esta evaluación.
        public int OfertaID { get; set; }
        public string EstadoMotor { get; set; }
        public string EstadoCarroceria { get; set; }
        public string EstadoInterior { get; set; }
        public string EstadoDocumentacion { get; set; }
        public string Observaciones { get; set; }
    }
}
