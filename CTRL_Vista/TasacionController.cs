using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Entidades;


namespace AutoGestion.CTRL_Vista
{
    public class TasacionController
    {
        private readonly OfertaBLL _ofertaBll = new();
        private readonly EvaluacionBLL _evaluacionBll = new();
        private readonly TasaBLL _tasaBll = new();
        private readonly VehiculoBLL _vehiculoBll = new();

        /// <summary>
        /// Obtiene todas las ofertas con evaluación para tasar,
        /// y retorna DTOs con rango sugerido (si existe).
        /// </summary>
        public List<TasacionListDto> ObtenerOfertasParaTasar()
        {
            var ofertas = _ofertaBll.ObtenerOfertasConEvaluacion();
            var resultado = new List<TasacionListDto>();

            foreach (var o in ofertas)
            {
                // 1) Recupero la evaluación asociada (o null)
                var eval = _evaluacionBll.ObtenerEvaluacionAsociada(o);

                // 2) Llamo al BLL de tasación, que me devuelve un RangoTasacion o null
                RangoTasacion rangoEntity = eval != null
                    ? _tasaBll.CalcularRangoTasacion(o.Vehiculo.Modelo,
                                                    eval.EstadoMotor,
                                                    o.Vehiculo.Km)
                    : null;

                // 3) Convierto ese objeto en un tuple opcional (decimal Min, decimal Max)?
                (decimal Min, decimal Max)? rangoNullable = null;
                if (rangoEntity != null)
                {
                    rangoNullable = (rangoEntity.Min, rangoEntity.Max);
                }

                // 4) Construyo el DTO pasando el tuple
                resultado.Add(TasacionListDto.FromEntity(o, eval!, rangoNullable));
            }

            return resultado;
        }

        /// <summary>
        /// Registra la tasación y actualiza el estado del vehículo.
        /// </summary>
        public void RegistrarTasacion(TasacionInputDto dto)
        {
            // 1) Buscar la oferta
            var oferta = _ofertaBll.ObtenerOfertasConEvaluacion()
                                  .FirstOrDefault(o => o.ID == dto.OfertaID);
            if (oferta == null)
                throw new ApplicationException("Oferta no encontrada.");

            // 2) Registrar la tasación
            _tasaBll.RegistrarTasacion(oferta, dto.ValorFinal);

            // 3) Actualizar stock del vehículo
            _vehiculoBll.ActualizarEstadoStock(oferta.Vehiculo, dto.EstadoStock);
        }
    }
}
