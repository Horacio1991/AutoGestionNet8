﻿namespace AutoGestion.Entidades
{
    [Serializable]
    public class EvaluacionTecnica
    {
        public int ID { get; set; }
        public string EstadoMotor { get; set; }
        public string EstadoCarroceria { get; set; }
        public string EstadoInterior { get; set; }
        public string EstadoDocumentacion { get; set; }
        public string Observaciones { get; set; }
    }
}

