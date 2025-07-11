using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.CTRL_Vista
{
    public class ClienteController
    {
        private readonly ClienteBLL _clienteBll = new();

        public ClienteDto BuscarCliente(string dni)
        {
            try
            {
                var entidad = _clienteBll.BuscarClientePorDNI(dni);
                
                // Mapear a DTO o devolver null
                return entidad == null ? null : ClienteDto.FromEntity(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar cliente: {ex.Message}", ex);
            }
        }

        // Registra un nuevo cliente y retorna su DTO.
        public ClienteDto RegistrarCliente(string dni, string nombre, string apellido, string contacto)
        {
            try
            {
                // 1) Construir entidad
                var entidad = new Entidades.Cliente
                {
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    Contacto = contacto,
                    FechaRegistro = DateTime.Now
                };
                // 2) Invocar BLL para persistir
                var registrado = _clienteBll.RegistrarCliente(entidad);
                // 3) Mapear a DTO y retornar
                return ClienteDto.FromEntity(registrado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar cliente: {ex.Message}", ex);
            }
        }
    }
}
