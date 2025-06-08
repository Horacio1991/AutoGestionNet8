using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Servicios.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGestion.CTRL_Vista
{
    public class ClienteController
    {
        private readonly ClienteBLL _clienteBLL = new();

        public Cliente BuscarCliente(string dni)
        {
            try
            {
                return _clienteBLL.BuscarClientePorDNI(dni);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al buscar cliente", ex);
            }
        }

        public Cliente RegistrarCliente(string dni, string nombre, string apellido, string contacto)
        {
            try
            {
                Cliente nuevo = new Cliente
                {
                    ID = GeneradorID.ObtenerID<Cliente>(), // <-- ESTA LÍNEA ES LA CLAVE
                    Dni = dni,
                    Nombre = nombre,
                    Apellido = apellido,
                    Contacto = contacto,
                    FechaRegistro = DateTime.Now
                };

                return _clienteBLL.RegistrarCliente(nuevo);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error al registrar cliente", ex);
            }
        }
    }
}
