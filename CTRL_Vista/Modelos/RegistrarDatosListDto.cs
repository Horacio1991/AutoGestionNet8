using AutoGestion.Entidades;

public class RegistrarDatosListDto
{
    public int OfertaID { get; set; }
    public string VehiculoResumen { get; set; }
    public string EvaluacionTexto { get; set; }

    public static RegistrarDatosListDto FromEntity(
        OfertaCompra o, EvaluacionTecnica ev)
    {
        return new()
        {
            OfertaID = o.ID,
            VehiculoResumen = $"{o.Vehiculo.Marca} {o.Vehiculo.Modelo} ({o.Vehiculo.Dominio})",
            EvaluacionTexto = $"Motor: {ev.EstadoMotor}; Carrocería: {ev.EstadoCarroceria}; Interior: {ev.EstadoInterior}; Doc: {ev.EstadoDocumentacion}"
        };
    }
}

public class RegistrarDatosInputDto
{
    public int OfertaID { get; set; }
    public string EstadoStock { get; set; }
}
