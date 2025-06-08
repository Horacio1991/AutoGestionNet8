using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class Factura
    {
        public int ID { get; set; }
        public Cliente Cliente { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public string FormaPago { get; set; }
        public decimal Precio { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}

