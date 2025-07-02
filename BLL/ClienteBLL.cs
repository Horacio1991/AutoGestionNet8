using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class ClienteBLL
    {
        private readonly XmlRepository<Cliente> _repo;

        // 1) Inicializa el repositorio apuntando a "DatosXML/clientes.xml".
        public ClienteBLL()
        {
            _repo = new XmlRepository<Cliente>("clientes.xml");
        }

        // Busca un cliente por su DNI. Usado para evitar duplicados al registrar.
        public Cliente BuscarClientePorDNI(string dni)
        {
            try
            {
                // 1) Leer todos los clientes
                var todos = _repo.ObtenerTodos();

                // 2) Buscar coincidencia exacta
                return todos.FirstOrDefault(c => c.Dni == dni);
            }
            catch (ApplicationException)
            {
                // 3) Retornar null para que la capa de UI no falle
                return null;
            }
        }

        // Agrega un nuevo cliente en el sistema.
        public Cliente RegistrarCliente(Cliente cliente)
        {
            try
            {
                // 1) Validar datos obligatorios
                if (string.IsNullOrWhiteSpace(cliente.Dni))
                    throw new ApplicationException("El DNI es obligatorio.");
                if (string.IsNullOrWhiteSpace(cliente.Nombre))
                    throw new ApplicationException("El nombre es obligatorio.");
                if (string.IsNullOrWhiteSpace(cliente.Apellido))
                    throw new ApplicationException("El apellido es obligatorio.");

                // 2) Asignar nuevo ID único
                cliente.ID = GeneradorID.ObtenerID<Cliente>();

                // 3) Fijar fecha de registro
                cliente.FechaRegistro = DateTime.Now;

                // 4) Persistir la entidad
                _repo.Agregar(cliente);

                return cliente;
            }
            catch (Exception ex) when (ex is IOException || ex is InvalidOperationException || ex is ApplicationException)
            {
                // 5) Re-lanzar con mensaje claro para la capa superior
                throw new ApplicationException($"Error al registrar cliente: {ex.Message}", ex);
            }
        }
    }
}
