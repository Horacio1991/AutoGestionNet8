using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    //BLL para gestionar las ofertas de compra de vehículos.
    public class OfertaBLL
    {
        private readonly XmlRepository<OfertaCompra> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/ofertas.xml".
        public OfertaBLL()
        {
            _repo = new XmlRepository<OfertaCompra>("ofertas.xml");
        }

        public void RegistrarOferta(OfertaCompra oferta)
        {
            try
            {
                // 1) Asignar ID de oferta si no tiene
                if (oferta.ID == 0)
                    oferta.ID = GeneradorID.ObtenerID<OfertaCompra>();

                // 2) Asignar ID de vehículo si no tiene
                if (oferta.Vehiculo.ID == 0)
                    oferta.Vehiculo.ID = GeneradorID.ObtenerID<Vehiculo>();

                // 3) Cargar lista existente
                var lista = _repo.ObtenerTodos().ToList();

                // 4) Agregar nueva oferta
                lista.Add(oferta);

                // 5) Persistir lista en XML
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al registrar oferta: {ex.Message}", ex);
            }
        }

        // Obtiene todas las ofertas en evaluación.
        // Usado para mostrar ofertas pendientes de revisión.
        public List<OfertaCompra> ObtenerOfertasSinRegistrar()
        {
            try
            {
                // 1) Leer todas las ofertas
                var ofertas = _repo.ObtenerTodos();
                // 2) Filtrar por estado
                return ofertas.Where(o => o.Estado == "En evaluación").ToList();
            }
            catch (ApplicationException)
            {
                return new List<OfertaCompra>();
            }
        }


        // Obtiene todas las ofertas registradas (sin filtrar).
        public List<OfertaCompra> ObtenerTodas()
        {
            try
            {
                return _repo.ObtenerTodos();
            }
            catch (ApplicationException)
            {
                return new List<OfertaCompra>();
            }
        }

        // Obtiene ofertas que ya tienen inspección programada.
        // Usado para gestionar inspecciones pendientes.
        public List<OfertaCompra> ObtenerOfertasConInspeccion()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(o => o.FechaInspeccion != DateTime.MinValue) // Verifica si la fecha de inspección es válida
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<OfertaCompra>();
            }
        }

        // Obtiene ofertas que cuentan con evaluación técnica asociada.
        public List<OfertaCompra> ObtenerOfertasConEvaluacion()
        {
            try
            {
                // 1) Obtener IDs evaluadas
                var evaluaciones = new XmlRepository<EvaluacionTecnica>("evaluaciones.xml")
                                   .ObtenerTodos()
                                   .Select(e => e.ID)
                                   .ToHashSet(); // para busqueda rapida y evitar duplicados

                // 2) Filtrar ofertas
                return _repo.ObtenerTodos()
                            .Where(o => evaluaciones.Contains(o.ID))
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<OfertaCompra>();
            }
        }

        // nuevoEstado = Nuevo estado a asignar (ej. "Aceptada", "Rechazada").
        public void ActualizarEstado(OfertaCompra oferta, string nuevoEstado)
        {
            try
            {
                // 1) Leer lista
                var lista = _repo.ObtenerTodos().ToList();
                // 2) Encontrar oferta
                var ent = lista.FirstOrDefault(x => x.ID == oferta.ID)
                          ?? throw new ApplicationException("Oferta no encontrada.");
                // 3) Actualizar estado
                ent.Estado = nuevoEstado;
                // 4) Guardar lista
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ApplicationException)
            {
                throw new ApplicationException($"Error al actualizar estado de oferta: {ex.Message}", ex);
            }
        }

        // Marca una oferta como procesada, eliminándola del XML.
        public void MarcarOfertaProcesada(int ofertaID)
        {
            try
            {
                // 1) Leer lista
                var lista = _repo.ObtenerTodos().ToList();
                // 2) Quitar oferta
                lista.RemoveAll(o => o.ID == ofertaID);
                // 3) Guardar cambios
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al marcar oferta procesada: {ex.Message}", ex);
            }
        }

        // Obtiene ofertas cuyo estado es "EnEvaluacion".
        public List<OfertaCompra> ObtenerOfertasPendientes()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(o => o.Estado == "EnEvaluacion")
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<OfertaCompra>();
            }
        }

        // Actualiza toda la entidad oferta en el XML.
        // oferta = OfertaCompra con datos actualizados.
        public void ActualizarOferta(OfertaCompra oferta)
        {
            try
            {
                // 1) Leer lista
                var lista = _repo.ObtenerTodos().ToList();
                // 2) Reemplazar elemento existente
                var idx = lista.FindIndex(x => x.ID == oferta.ID);
                if (idx >= 0)
                {
                    lista[idx] = oferta;
                    // 3) Guardar lista actualizada
                    _repo.GuardarLista(lista);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al actualizar oferta: {ex.Message}", ex);
            }
        }
    }
}
