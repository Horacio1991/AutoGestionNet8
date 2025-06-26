using AutoGestion.DAO.Repositorios;
using AutoGestion.Entidades;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class OfertaBLL
    {
        // Repositorio genérico que lee/escribe List<OfertaCompra> en "DatosXML/ofertas.xml"
        private readonly XmlRepository<OfertaCompra> _repo = new("ofertas.xml");

        public void RegistrarOferta(OfertaCompra oferta)
        {
            // Le asignamos un ID único si aún no lo tiene
            if (oferta.ID == 0)
                oferta.ID = GeneradorID.ObtenerID<OfertaCompra>();

            // También aseguramos que el vehículo tenga su propio ID
            if (oferta.Vehiculo.ID == 0)
                oferta.Vehiculo.ID = GeneradorID.ObtenerID<Vehiculo>();

            var lista = _repo.ObtenerTodos();
            lista.Add(oferta);
            _repo.GuardarLista(lista);
        }
        public List<OfertaCompra> ObtenerOfertasSinRegistrar()
          => _repo.ObtenerTodos()
                  .Where(o => o.Estado == "En evaluación")
                  .ToList();

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

        public void ActualizarEstado(OfertaCompra oferta, string nuevoEstado)
        {
            var all = _repo.ObtenerTodos().ToList();
            var ent = all.FirstOrDefault(x => x.ID == oferta.ID);
            if (ent == null) throw new ApplicationException("Oferta no encontrada");
            ent.Estado = nuevoEstado;
            _repo.GuardarLista(all);
        }

        /// <summary>
        /// Quita la oferta de la lista (para que no vuelva a aparecer en ObtenerOfertasSinRegistrar).
        /// </summary>
        public void MarcarOfertaProcesada(int ofertaID)
        {
            // 1) Carga todas las ofertas
            var lista = _repo.ObtenerTodos();

            // 2) Busca la que corresponde
            var oferta = lista.FirstOrDefault(o => o.ID == ofertaID);
            if (oferta == null) return;

            // 3) La quita de la lista y guarda
            lista.Remove(oferta);
            _repo.GuardarLista(lista);
        }

        public List<OfertaCompra> ObtenerOfertasPendientes()
            => _repo.ObtenerTodos()
            .Where(o => o.Estado == "EnEvaluacion")
            .ToList();

        public void ActualizarOferta(OfertaCompra oferta)
        {
            var lista = _repo.ObtenerTodos();
            var idx = lista.FindIndex(x => x.ID == oferta.ID);
            if (idx >= 0)
            {
                lista[idx] = oferta;
                _repo.GuardarLista(lista);
            }
        }


    }
}
