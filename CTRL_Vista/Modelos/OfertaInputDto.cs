using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.DTOs
{
    public class OfertaInputDto
    {
        public OferenteDto Oferente { get; set; }
        public VehiculoDto Vehiculo { get; set; }
        public DateTime FechaInspeccion { get; set; }
    }
}
