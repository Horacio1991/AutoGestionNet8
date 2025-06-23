using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.BLL
{
    public class VehiculoBLL
    {
        // Repositorio genérico que persiste List<Vehiculo> en DatosXML/vehiculos.xml
        private readonly XmlRepository<Vehiculo> _repo = new("vehiculos.xml");

        // Busca vehiculos disponibles por modelo y disponibilidad
        public List<Vehiculo> BuscarVehiculosPorModelo(string modelo)
        {
            return _repo.ObtenerTodos()
                        .Where(v => v.Modelo.ToLower() == modelo.ToLower() && v.Estado == "Disponible")
                        .ToList();
        }

        // Devuelve vehiculos similares por modelo o marca (Segun la cadena de texto ingresada)
        public List<Vehiculo> BuscarVehiculosSimilares(string modelo)
        {
            var lista = _repo.ObtenerTodos();

            return lista.Where(v =>
                v.Modelo.Contains(modelo, StringComparison.OrdinalIgnoreCase) ||
                v.Marca.Contains(modelo, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        // Cambia el estado de un vehiculo en stock (Ej: "Vendido", "Disponible", etc.)
        public void ActualizarEstadoStock(Vehiculo vehiculo, string estado)
        {
            var lista = _repo.ObtenerTodos();
            var v = lista.FirstOrDefault(x => x.Dominio == vehiculo.Dominio);
            if (v != null)
            {
                v.Estado = estado;
                _repo.GuardarLista(lista);
            }
        }

        public Vehiculo BuscarVehiculoPorDominio(string dominio)
        {
            return _repo.ObtenerTodos()
                        .FirstOrDefault(v => v.Dominio.Equals(dominio, StringComparison.OrdinalIgnoreCase));
        }

        public List<Vehiculo> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }

        public List<Vehiculo> ObtenerDisponibles()
        {
            return _repo.ObtenerTodos().Where(v => v.Estado == "Disponible").ToList();
        }

        public void ActualizarEstadoVehiculo(Vehiculo vehiculo, string nuevoEstado)
        {
            var lista = _repo.ObtenerTodos();
            var existente = lista.FirstOrDefault(v => v.ID == vehiculo.ID);

            if (existente != null)
            {
                existente.Estado = nuevoEstado;
                _repo.GuardarLista(lista);
            }
        }

        public void AgregarVehiculoAlStock(Vehiculo vehiculo)
        {
            var lista = _repo.ObtenerTodos();
            vehiculo.ID = GeneradorID.ObtenerID<Vehiculo>();
            vehiculo.Estado = "Disponible"; // lo dejamos listo para la venta
            lista.Add(vehiculo);
            _repo.GuardarLista(lista);
        }

    }
}
