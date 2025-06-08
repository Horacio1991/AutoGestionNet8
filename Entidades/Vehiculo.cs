using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.Entidades

{
    [Serializable]
    public class Vehiculo
    {
        public int ID { get; set; } // ID único para el vehículo
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public string Color { get; set; }
        public int Km { get; set; }
        public string Dominio { get; set; }
        public string Estado { get; set; } // Ej: Disponible, Vendido
    }
}
