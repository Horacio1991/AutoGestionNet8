using AutoGestion.BLL;
using AutoGestion.DTOs;

public class RegistrarDatosController
{
    private readonly OfertaBLL _ofertaBll = new();
    private readonly EvaluacionBLL _evaluacionBll = new();
    private readonly VehiculoBLL _vehiculoBll = new();

    public OfertaRegistroDto ObtenerOfertaPorDominio(string dominio)
    {
        var oferta = _ofertaBll.ObtenerOfertasSinRegistrar()
                              .FirstOrDefault(o =>
                                o.Vehiculo.Dominio.Equals(dominio, StringComparison.OrdinalIgnoreCase));
        if (oferta == null) return null;

        var ev = _evaluacionBll.ObtenerEvaluacionAsociada(oferta);
        if (ev == null) return null;

        return new OfertaRegistroDto
        {
            OfertaID = oferta.ID,
            EvaluacionTexto =
            $"Motor: {ev.EstadoMotor}\r\n" +
            $"Carrocería: {ev.EstadoCarroceria}\r\n" +
            $"Interior: {ev.EstadoInterior}\r\n" +
            $"Documentación: {ev.EstadoDocumentacion}"
        };
    }

    public void RegistrarDatos(RegistrarDatosInputDto dto)
    {
        // 1) recuperar oferta
        var oferta = _ofertaBll.ObtenerOfertasSinRegistrar()
                              .FirstOrDefault(o => o.ID == dto.OfertaID);
        if (oferta == null)
            throw new ApplicationException("Oferta no encontrada.");

        // 2) actualizar estado del vehículo
        _vehiculoBll.ActualizarEstadoVehiculo(oferta.Vehiculo, dto.EstadoStock);

        // 3) si quedó "Disponible", ingresarlo a stock
        if (dto.EstadoStock == "Disponible")
            _vehiculoBll.AgregarVehiculoAlStock(oferta.Vehiculo);

        // 4) marcar la oferta como ya procesada
        oferta.Estado = "Registrada";
        _ofertaBll.ActualizarOferta(oferta);
    }
}
