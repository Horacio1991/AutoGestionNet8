using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using System.Collections.Generic;
using System.Linq;

namespace AutoGestion.BLL
{
    public class OfertaBLL
    {
        private readonly XmlRepository<OfertaCompra> _repo = new("ofertas.xml");

        public bool ValidarOfertaDuplicada(Vehiculo vehiculo)
        {
            return _repo.ObtenerTodos()
                        .Any(o => o.Vehiculo.Dominio == vehiculo.Dominio);
        }

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

        public List<OfertaCompra> ObtenerOfertasConInspeccion()
        {
            return _repo.ObtenerTodos()
                        .Where(o => o.FechaInspeccion != DateTime.MinValue)
                        .ToList();
        }

        public List<OfertaCompra> ObtenerOfertasConEvaluacion()
        {
            var evaluaciones = new XmlRepository<EvaluacionTecnica>("evaluaciones.xml").ObtenerTodos();
            var ofertas = _repo.ObtenerTodos();

            return ofertas
                .Where(o => evaluaciones.Any(e => e.ID == o.ID)) // se asume que comparten ID
                .ToList();
        }

        public OfertaCompra BuscarOfertaPorDominio(string dominio)
        {
            return _repo.ObtenerTodos()
                        .FirstOrDefault(o => o.Vehiculo.Dominio.Equals(dominio, StringComparison.OrdinalIgnoreCase));
        }

        public List<OfertaCompra> ObtenerOfertasSinRegistrar()
        {
            var ofertas = ObtenerTodas();
            var vehiculosRegistrados = new VehiculoBLL().ObtenerTodos().Select(v => v.Dominio).ToList();

            return ofertas.Where(o => !vehiculosRegistrados.Contains(o.Vehiculo.Dominio)).ToList();
        }




    }
}
