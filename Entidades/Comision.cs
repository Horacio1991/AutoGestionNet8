using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class Comision
    {
        public int ID { get; set; }
        public Venta Venta { get; set; }
        public decimal Porcentaje { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } // Aprobada o Rechazada
        public DateTime Fecha { get; set; } = DateTime.Now;

        public string MotivoRechazo { get; set; } // <-- NUEVO
    }

}
