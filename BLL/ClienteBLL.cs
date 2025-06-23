using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;

namespace AutoGestion.BLL
{
    public class ClienteBLL
    {
        //Repositorio para manejar la persistencia de clientes en "DatosXML/clientes.xml"
        private readonly XmlRepository<Cliente> _repo;

        // Constructor que inicializa el repositorio con el archivo XML de clientes
        public ClienteBLL()
        {
            _repo = new XmlRepository<Cliente>("clientes.xml");
        }

        public Cliente BuscarClientePorDNI(string dni)
        {
            // Carga todos los clientes y busca el que coincida con el DNI proporcionado sino devuelve null
            return _repo.ObtenerTodos().FirstOrDefault(c => c.Dni == dni);
        }

        public Cliente RegistrarCliente(Cliente cliente)
        {
            cliente.FechaRegistro = DateTime.Now;
            // Agrega el cliente a l lista y lo guarda en el repositorio XML
            _repo.Agregar(cliente);
            return cliente;
        }
    }
}
