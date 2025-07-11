using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    // DTO de salida para mostrar en la UI los datos de una oferta
    public class RegistrarDatosListDto
    {
        public int OfertaID { get; set; }


        // Resumen del vehículo: Marca, Modelo y Dominio.
        public string VehiculoResumen { get; set; }

        //Texto que describe los estados técnicos evaluados: motor, carrocería, interior y documentación.
        public string EvaluacionTexto { get; set; }


        // Mapea una entidad OfertaCompra y su EvaluacionTecnica asociada
        public static RegistrarDatosListDto FromEntity(OfertaCompra o, EvaluacionTecnica ev)
        {
            if (o == null || ev == null) return null;

            return new RegistrarDatosListDto
            {
                OfertaID = o.ID,
                VehiculoResumen = $"{o.Vehiculo.Marca} {o.Vehiculo.Modelo} ({o.Vehiculo.Dominio})",
                EvaluacionTexto = $"Motor: {ev.EstadoMotor}; " +
                                   $"Carrocería: {ev.EstadoCarroceria}; " +
                                   $"Interior: {ev.EstadoInterior}; " +
                                   $"Doc: {ev.EstadoDocumentacion}"
            };
        }
    }
}
