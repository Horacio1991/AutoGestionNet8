using System;
using System.Linq;
using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;

namespace AutoGestion.BLL
{
    public class ClienteBLL
    {
        private readonly XmlRepository<Cliente> _repo;

        public ClienteBLL()
        {
            _repo = new XmlRepository<Cliente>("clientes.xml");
        }

        private int ObtenerNuevoID()
        {
            var lista = _repo.ObtenerTodos();
            return lista.Any() ? lista.Max(c => c.ID) + 1 : 1;
        }

        public Cliente BuscarClientePorDNI(string dni)
        {
            return _repo.ObtenerTodos().FirstOrDefault(c => c.Dni == dni);
        }

        public Cliente RegistrarCliente(Cliente cliente)
        {
            cliente.FechaRegistro = DateTime.Now;
            _repo.Agregar(cliente);
            return cliente;
        }
    }
}
