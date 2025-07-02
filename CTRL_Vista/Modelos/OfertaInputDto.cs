using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.DTOs
{
    // DTO de entrada para crear una nueva oferta de compra,
    public class OfertaInputDto
    {
        // Datos del oferente (DTO) que realiza la oferta.
        public OferenteDto Oferente { get; set; }

        // Datos del vehículo (DTO) al que se hace la oferta.
        public VehiculoDto Vehiculo { get; set; }

        public DateTime FechaInspeccion { get; set; }
    }
}
