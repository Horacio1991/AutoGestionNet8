﻿namespace AutoGestion.DTOs
{
    public class EvaluacionInputDto
    {
        public int OfertaID { get; set; }  // Para saber a qué oferta asociar
        public string EstadoMotor { get; set; }
        public string EstadoCarroceria { get; set; }
        public string EstadoInterior { get; set; }
        public string EstadoDocumentacion { get; set; }
        public string Observaciones { get; set; }
    }
}
