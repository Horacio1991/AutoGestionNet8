using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class ComprobanteEntrega
    {
        public int ID { get; set; }
        public Venta Venta { get; set; }
        public DateTime FechaEntrega { get; set; } = DateTime.Now;
    }
}
