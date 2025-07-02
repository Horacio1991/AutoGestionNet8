using AutoGestion.DAO.Repositorios;
using AutoGestion.Entidades;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class VehiculoBLL
    {
        private readonly XmlRepository<Vehiculo> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/vehiculos.xml".
        public VehiculoBLL()
        {
            _repo = new XmlRepository<Vehiculo>("vehiculos.xml");
        }

        // Busca vehículos disponibles filtrando por modelo exacto.
        // modelo = Modelo del vehículo a buscar (ej "Corolla").

        public List<Vehiculo> BuscarVehiculosPorModelo(string modelo)
        {
            try
            {
                // 1) Leer todos los vehículos
                var todos = _repo.ObtenerTodos();
                // 2) Filtrar por modelo exacto y estado disponible
                return todos
                    .Where(v => v.Modelo.Equals(modelo, StringComparison.OrdinalIgnoreCase)
                             && v.Estado == VehiculoEstados.Disponible)
                    .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Vehiculo>();
            }
        }

        // Busca vehículos cuyo modelo o marca contenga el texto dado.
        // texto = Texto a buscar en Modelo o Marca (ej "toyota").
        public List<Vehiculo> BuscarVehiculosSimilares(string texto)
        {
            try
            {
                // 1) Leer todos los vehículos
                var todos = _repo.ObtenerTodos();
                // 2) Filtrar por coincidencia parcial en modelo o marca
                return todos
                    .Where(v =>
                        v.Modelo.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        v.Marca.Contains(texto, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Vehiculo>();
            }
        }

        // Cambia el estado de un vehículo identificado por Dominio.
        // vehiculo = Vehículo a actualizar.
        // nuevoEstado = Nuevo estado a asignar.
        public void ActualizarEstadoVehiculo(Vehiculo vehiculo, string nuevoEstado)
        {
            try
            {
                // 1) Leer lista de vehículos
                var lista = _repo.ObtenerTodos();
                // 2) Encontrar vehículo por Dominio
                var existente = lista
                    .FirstOrDefault(v => v.ID == vehiculo.ID)
                    ?? throw new ApplicationException("Vehículo no encontrado.");
                // 3) Actualizar estado
                existente.Estado = nuevoEstado;
                // 4) Guardar cambios
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ApplicationException)
            {
                throw new ApplicationException($"Error al actualizar estado de vehículo: {ex.Message}", ex);
            }
        }


        // Busca un vehículo por dominio exacto.
        public Vehiculo BuscarVehiculoPorDominio(string dominio)
        {
            try
            {
                // 1) Leer todos los vehículos
                var todos = _repo.ObtenerTodos();
                // 2) Buscar coincidencia exacta
                return todos.FirstOrDefault(v =>
                    v.Dominio.Equals(dominio, StringComparison.OrdinalIgnoreCase));
            }
            catch (ApplicationException)
            {
                return null;
            }
        }

        /// Obtiene la lista completa de vehículos.
        public List<Vehiculo> ObtenerTodos()
        {
            try
            {
                return _repo.ObtenerTodos();
            }
            catch (ApplicationException)
            {
                return new List<Vehiculo>();
            }
        }

        /// Obtiene solo los vehículos con estado "Disponible".
        public List<Vehiculo> ObtenerDisponibles()
        {
            try
            {
                return _repo.ObtenerTodos()
                            .Where(v => v.Estado == VehiculoEstados.Disponible)
                            .ToList();
            }
            catch (ApplicationException)
            {
                return new List<Vehiculo>();
            }
        }


        // Agrega un nuevo vehículo al stock, asignándole ID y estado "Disponible".
        // vehiculo = Vehículo a agregar al stock.

        public void AgregarVehiculoAlStock(Vehiculo vehiculo)
        {
            try
            {
                // 1) Asignar ID único
                vehiculo.ID = GeneradorID.ObtenerID<Vehiculo>();
                // 2) Marcar como disponible
                vehiculo.Estado = VehiculoEstados.Disponible;
                // 3) Leer lista actual
                var lista = _repo.ObtenerTodos();
                // 4) Agregar nuevo vehículo
                lista.Add(vehiculo);
                // 5) Guardar lista actualizada
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException)
            {
                throw new ApplicationException($"Error al agregar vehículo: {ex.Message}", ex);
            }
        }

        // Actualiza el estado de stock de un vehículo (p.ej. "Disponible", "En Proceso").
        public void ActualizarEstadoStock(Vehiculo vehiculo, string nuevoEstado)
        {
            try
            {
                // 1) Leer lista de vehículos
                var lista = _repo.ObtenerTodos();
                // 2) Buscar el vehículo por ID
                var existente = lista
                    .FirstOrDefault(v => v.ID == vehiculo.ID)
                    ?? throw new ApplicationException("Vehículo no encontrado.");
                // 3) Asignar nuevo estado de stock
                existente.Estado = nuevoEstado;
                // 4) Persistir cambios en XML
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ApplicationException)
            {
                throw new ApplicationException($"Error al actualizar estado de stock del vehículo: {ex.Message}", ex);
            }
        }

    }
}
