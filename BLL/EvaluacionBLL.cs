using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

public class EvaluacionBLL
{
    // Repositorio que persiste las evaluaciones técnicas en un archivo XML 
    private readonly XmlRepository<EvaluacionTecnica> _repo = new("evaluaciones.xml");

    public void GuardarEvaluacion(OfertaCompra oferta, EvaluacionTecnica evaluacion)
    {
        evaluacion.ID = GeneradorID.ObtenerID<EvaluacionTecnica>();
        //OfertaCompra a la que se asocia la evaluación, se usa el ID de la oferta       
        evaluacion.ID = oferta.ID;

        var lista = _repo.ObtenerTodos();
        lista.Add(evaluacion);
        _repo.GuardarLista(lista);
    }

    public EvaluacionTecnica ObtenerEvaluacionAsociada(OfertaCompra oferta)
    {
        var lista = _repo.ObtenerTodos();
        return lista.FirstOrDefault(e => e.ID == oferta.ID);
    }

}
