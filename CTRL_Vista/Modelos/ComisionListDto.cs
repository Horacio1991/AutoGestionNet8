using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    // Presenta los datos para las grid, con campos ya formateados
    public class ComisionListDto
    {
        //ID de la comisión
        public int ID { get; set; }
        public string Fecha { get; set; }

        //Nombre completo del cliente asociado a la venta.
        public string Cliente { get; set; }

        //Resumen del vehículo: marca, modelo y dominio.
        public string Vehiculo { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } // Aprobada o rechazada
        public string MotivoRechazo { get; set; }

        // Mapea Comision a ComisionListDto para la UI.
        // c = Entidad Comision a mapear;
        public static ComisionListDto FromEntity(Comision c)
        {
            // 1) Validar null para evitar excepciones en la UI
            if (c == null) return null;

            // 2) Construir el DTO extrayendo y formateando solo lo necesario
            return new ComisionListDto
            {
                ID = c.ID,
                Fecha = c.Fecha.ToShortDateString(),
                Cliente = $"{c.Venta?.Cliente?.Nombre} {c.Venta?.Cliente?.Apellido}".Trim(),
                Vehiculo = $"{c.Venta?.Vehiculo?.Marca} {c.Venta?.Vehiculo?.Modelo} ({c.Venta?.Vehiculo?.Dominio})",
                Monto = c.Monto,
                Estado = c.Estado,
                MotivoRechazo = c.MotivoRechazo
            };
        }
    }
}
