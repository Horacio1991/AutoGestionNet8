using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;

namespace AutoGestion.BLL
{
    public class OfertaBLL
    {
        // Repositorio genérico que lee/escribe List<OfertaCompra> en "DatosXML/ofertas.xml"
        private readonly XmlRepository<OfertaCompra> _repo = new("ofertas.xml");

        public void RegistrarOferta(OfertaCompra oferta)
        {
            oferta.ID = ObtenerNuevoID();
            _repo.Agregar(oferta);
        }

        private int ObtenerNuevoID()
        {
            var lista = _repo.ObtenerTodos();
            return lista.Any() ? lista.Max(o => o.ID) + 1 : 1;
        }

        public List<OfertaCompra> ObtenerTodas()
        {
            return _repo.ObtenerTodos();
        }

        // Obtiene solo ofertas que tienen fecha de inspección registrada
        public List<OfertaCompra> ObtenerOfertasConInspeccion()
        {
            return _repo.ObtenerTodos()
                        .Where(o => o.FechaInspeccion != DateTime.MinValue)
                        .ToList();
        }

        // Obtiene solo ofertas que tienen fecha de evaluación técnica registrada
        public List<OfertaCompra> ObtenerOfertasConEvaluacion()
        {
            var evaluaciones = new XmlRepository<EvaluacionTecnica>("evaluaciones.xml").ObtenerTodos();
            var ofertas = _repo.ObtenerTodos();

            // Filtra solo las oferta.ID que aparece en alguna evaluacion
            return ofertas
                .Where(o => evaluaciones.Any(e => e.ID == o.ID)) 
                .ToList();
        }

        // obtiene las ofertas de los vehiculos que no estan registrados en el stock
        public List<OfertaCompra> ObtenerOfertasSinRegistrar()
        {
            var ofertas = ObtenerTodas();
            var vehiculosRegistrados = new VehiculoBLL().ObtenerTodos().Select(v => v.Dominio).ToList();

            return ofertas.Where(o => !vehiculosRegistrados.Contains(o.Vehiculo.Dominio)).ToList();
        }

    }
}
