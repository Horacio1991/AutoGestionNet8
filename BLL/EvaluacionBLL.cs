using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

public class EvaluacionBLL
{
    private readonly XmlRepository<EvaluacionTecnica> _repo = new("evaluaciones.xml");

    public void GuardarEvaluacion(OfertaCompra oferta, EvaluacionTecnica evaluacion)
    {
        evaluacion.ID = GeneradorID.ObtenerID<EvaluacionTecnica>();
        // Importante: vinculamos evaluación con la oferta por ID
        evaluacion.ID = oferta.ID;

        var lista = _repo.ObtenerTodos();
        lista.Add(evaluacion);
        _repo.GuardarLista(lista);
    }

    public List<EvaluacionTecnica> ObtenerTodas()
    {
        return _repo.ObtenerTodos();
    }

    public EvaluacionTecnica ObtenerEvaluacionAsociada(OfertaCompra oferta)
    {
        var lista = _repo.ObtenerTodos();
        return lista.FirstOrDefault(e => e.ID == oferta.ID);
    }

    public void IngresarVehiculoAlStock(Vehiculo vehiculo, string estado)
    {
        vehiculo.Estado = estado;
        new VehiculoBLL().ActualizarVehiculo(vehiculo);
    }

}
