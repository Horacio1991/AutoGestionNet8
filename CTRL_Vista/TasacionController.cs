using AutoGestion.BLL;
using AutoGestion.DTOs;

namespace AutoGestion.CTRL_Vista
{
    // Gestiona la tasación de ofertas:
    // - Listar ofertas con evaluación
    // - Registrar la tasación 
    public class TasacionController
    {
        private readonly OfertaBLL _ofertaBll = new();
        private readonly EvaluacionBLL _evaluacionBll = new();
        private readonly TasaBLL _tasaBll = new();


        // Obtiene todas las ofertas que ya cuentan con evaluación técnica,
        public List<TasacionListDto> ObtenerOfertasParaTasar()
        {
            try
            {
                var ofertas = _ofertaBll.ObtenerOfertasConEvaluacion();
                var resultado = new List<TasacionListDto>();

                foreach (var o in ofertas)
                {
                    var eval = _evaluacionBll.ObtenerEvaluacionAsociada(o);

                    (decimal Min, decimal Max)? rangoNullable = null;
                    if (eval != null)
                    {
                        var rangoEntity = _tasaBll.CalcularRangoTasacion(
                            o.Vehiculo.Modelo,
                            eval.EstadoMotor,
                            o.Vehiculo.Km);
                        rangoNullable = (rangoEntity.Min, rangoEntity.Max);
                    }

                    resultado.Add(TasacionListDto.FromEntity(o, eval, rangoNullable));
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al obtener ofertas para tasar: {ex.Message}", ex);
            }
        }

        // Registra la tasación de una oferta, guardando únicamente el valor final
        // dto = TasacionInputDto con OfertaID y ValorFinal.
        public void RegistrarTasacion(TasacionInputDto dto)
        {
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));
                if (dto.OfertaID <= 0)
                    throw new ArgumentException("OfertaID inválido.", nameof(dto.OfertaID));

                var oferta = _ofertaBll.ObtenerOfertasConEvaluacion()
                                      .Find(o => o.ID == dto.OfertaID);
                if (oferta == null)
                    throw new ApplicationException("Oferta no encontrada.");

                _tasaBll.RegistrarTasacion(oferta, dto.ValorFinal);

                // Nota: no modificamos aquí el stock del vehículo.
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Error al registrar tasación: {ex.Message}", ex);
            }
        }
    }
}
