using AutoGestion.Entidades;
using System;

namespace AutoGestion.Vista.Modelos
{
    // Para mostrar información de ventas en una vista
    public class VentaVista
    {
        public int ID { get; set; }
        public string Cliente { get; set; }
        public string Vehiculo { get; set; }
        public string TipoPago { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; }
        public string Fecha { get; set; }

        // Método de utilidad para convertir desde Venta real
        public static VentaVista DesdeVenta(Venta venta)
        {
            return new VentaVista
            {
                ID = venta.ID,
                Cliente = $"{venta.Cliente?.Nombre} {venta.Cliente?.Apellido}",
                Vehiculo = $"{venta.Vehiculo?.Marca} {venta.Vehiculo?.Modelo} ({venta.Vehiculo?.Dominio})",
                TipoPago = venta.Pago?.TipoPago ?? "N/A",
                Monto = venta.Total,
                Estado = venta.Estado,
                Fecha = venta.Fecha.ToShortDateString()
            };
        }
    }
}
