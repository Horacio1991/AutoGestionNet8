﻿using System;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class Vendedor
    {
        public int ID { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contacto { get; set; }
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        public override string ToString()
        {
            return $"{Nombre} {Apellido} ({DNI})";
        }
    }
}
