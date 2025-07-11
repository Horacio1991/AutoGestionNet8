using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista.Modelos
{

    // DTO para exponer datos de una venta en la pantalla
    
    public class VentaDto
    {
        public int ID { get; set; }

        public string Cliente { get; set; }

        public string Vehiculo { get; set; }

        public string TipoPago { get; set; }

        public decimal Monto { get; set; }
        public string Estado { get; set; } // Pendiente | Autorizada | Facturada | Rechazada
        public string Fecha { get; set; }

        // Mapea una entidad Venta a este DTO.
        public static VentaDto FromEntity(Venta v)
        {
            if (v == null) return null;

            return new VentaDto
            {
                ID = v.ID,
                Cliente = $"{v.Cliente?.Nombre} {v.Cliente?.Apellido}".Trim(),
                Vehiculo = $"{v.Vehiculo?.Marca} {v.Vehiculo?.Modelo} ({v.Vehiculo?.Dominio})",
                TipoPago = v.Pago?.TipoPago ?? "N/A",
                Monto = v.Total,
                Estado = v.Estado,
                Fecha = v.Fecha.ToShortDateString()
            };
        }
    }
}
