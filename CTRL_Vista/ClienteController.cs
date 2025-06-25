using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.CTRL_Vista
{
    public class ClienteController
    {
        private readonly ClienteBLL _clienteBLL = new();

        // Busca un cliente y devuelve DTO o null
        public ClienteDto BuscarCliente(string dni)
        {
            var c = _clienteBLL.BuscarClientePorDNI(dni);
            return c is null ? null : ClienteDto.FromEntity(c);
        }

        // Registra un nuevo cliente y devuelve su DTO
        public ClienteDto RegistrarCliente(string dni, string nombre, string apellido, string contacto)
        {
            var entidad = new Entidades.Cliente
            {
                Dni = dni,
                Nombre = nombre,
                Apellido = apellido,
                Contacto = contacto,
                FechaRegistro = DateTime.Now
            };
            var registrado = _clienteBLL.RegistrarCliente(entidad);
            return ClienteDto.FromEntity(registrado);
        }
    }
}
