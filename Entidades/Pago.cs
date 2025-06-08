using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class Pago
    {
        public int ID { get; set; } // ID único para el pago
        public string TipoPago { get; set; } // Contado o Financiado
        public decimal Monto { get; set; }
        public int Cuotas { get; set; } // Solo si es financiado
        public string Detalles { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
    }
}

