using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;

namespace AutoGestion.BLL
{
    // Gestiona las evaluaciones técnicas de ofertas de compra.
    public class EvaluacionBLL
    {
        private readonly XmlRepository<EvaluacionTecnica> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/evaluaciones.xml".
        public EvaluacionBLL()
        {
            _repo = new XmlRepository<EvaluacionTecnica>("evaluaciones.xml");
        }

        // Guarda una evaluación técnica asociada a una oferta.
        // oferta = Oferta Compra a la que se asocia la evaluación.
        // evaluacion = Evaluación Técnica a guardar.
        public void GuardarEvaluacion(OfertaCompra oferta, EvaluacionTecnica evaluacion)
        {
            try
            {
                // 1) Asignar ID igual al de la oferta para vinculación.
                evaluacion.ID = oferta.ID;
                // 2) Agregar a la lista existente
                var lista = _repo.ObtenerTodos();
                lista.RemoveAll(e => e.ID == oferta.ID); // evitar duplicados
                lista.Add(evaluacion);
                // 3) Persistir cambios
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException(
                    $"Error al guardar evaluación técnica: {ex.Message}", ex);
            }
        }

        // Obtiene la evaluación técnica asociada a una oferta.
        public EvaluacionTecnica ObtenerEvaluacionAsociada(OfertaCompra oferta)
        {
            try
            {
                var lista = _repo.ObtenerTodos();
                return lista.FirstOrDefault(e => e.ID == oferta.ID);
            }
            catch (ApplicationException)
            {
                return null;
            }
        }
    }
}
