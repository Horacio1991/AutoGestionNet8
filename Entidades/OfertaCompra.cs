using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class OfertaCompra
    {
        public int ID { get; set; }
        public Oferente Oferente { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public DateTime FechaInspeccion { get; set; }
    }
}
