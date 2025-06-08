using System;

namespace AutoGestion.Entidades
{
    [Serializable]
    public class Venta
    {
        public int ID { get; set; } // ID único para la venta
        public Vendedor Vendedor { get; set; }
        public Cliente Cliente { get; set; }
        public Vehiculo Vehiculo { get; set; }
        public Pago Pago { get; set; }
        public string Estado { get; set; } // "Pendiente", "Autorizada", "Rechazada"
        public DateTime Fecha { get; set; } = DateTime.Now;
        public decimal Total => Pago?.Monto ?? 0;
        public string MotivoRechazo { get; set; }

    }
}
