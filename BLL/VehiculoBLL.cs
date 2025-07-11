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
                var todos = _repo.ObtenerTodos();
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

        // Obtiene la lista completa de vehículos.
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

        // Obtiene solo los vehículos con estado "Disponible".
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

        public void AgregarVehiculoAlStock(Vehiculo vehiculo)
        {
            try
            {
                vehiculo.ID = GeneradorID.ObtenerID<Vehiculo>();
                vehiculo.Estado = VehiculoEstados.Disponible;
                var lista = _repo.ObtenerTodos();
                lista.Add(vehiculo);
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
                var lista = _repo.ObtenerTodos();
                var existente = lista
                    .FirstOrDefault(v => v.ID == vehiculo.ID)
                    ?? throw new ApplicationException("Vehículo no encontrado.");
                existente.Estado = nuevoEstado;
                _repo.GuardarLista(lista);
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ApplicationException)
            {
                throw new ApplicationException($"Error al actualizar estado de stock del vehículo: {ex.Message}", ex);
            }
        }

    }
}
