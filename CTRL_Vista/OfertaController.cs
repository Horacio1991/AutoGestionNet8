using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Entidades;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.CTRL_Vista
{
    // Se usa para gestionar ofertas de compra de vehículos:
    // - Búsqueda de oferentes
    // - Registro de nuevas ofertas (y creación de oferente si es necesario)
    public class OfertaController
    {
        private readonly OferenteBLL _oferenteBll = new();
        private readonly OfertaBLL _ofertaBll = new();

        // Busca un oferente por DNI y lo retorna como DTO
        public OferenteDto BuscarOferente(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni))
                    throw new ArgumentException("El DNI no puede estar vacío.", nameof(dni));

                var entidad = _oferenteBll.BuscarPorDni(dni);
                return entidad == null ? null : OferenteDto.FromEntity(entidad);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar oferente: {ex.Message}", ex);
            }
        }

        // Registra un oferente (si no existe) y luego la oferta.
        public bool RegistrarOferta(OfertaInputDto dto)
        {
            try
            {
                // validaciones 
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto));
                if (dto.Oferente == null)
                    throw new ArgumentException("Datos de oferente requeridos.", nameof(dto.Oferente));
                if (dto.Vehiculo == null)
                    throw new ArgumentException("Datos de vehículo requeridos.", nameof(dto.Vehiculo));
                if (dto.FechaInspeccion == default)
                    throw new ArgumentException("Fecha de inspección inválida.", nameof(dto.FechaInspeccion));

                // Buscar o crear oferente
                var entOferente = _oferenteBll.BuscarPorDni(dto.Oferente.Dni);
                if (entOferente == null)
                {
                    entOferente = new Oferente
                    {
                        ID = GeneradorID.ObtenerID<Oferente>(),
                        Dni = dto.Oferente.Dni,
                        Nombre = dto.Oferente.Nombre,
                        Apellido = dto.Oferente.Apellido,
                        Contacto = dto.Oferente.Contacto
                    };
                    _oferenteBll.GuardarOferente(entOferente);
                }

                // Crear entidad Vehiculo para la oferta
                var entVehiculo = new Vehiculo
                {
                    ID = GeneradorID.ObtenerID<Vehiculo>(),
                    Marca = dto.Vehiculo.Marca,
                    Modelo = dto.Vehiculo.Modelo,
                    Año = dto.Vehiculo.Año,
                    Color = dto.Vehiculo.Color,
                    Dominio = dto.Vehiculo.Dominio,
                    Km = dto.Vehiculo.Km,
                    Estado = "En evaluación"
                };

                // Crear entidad OfertaCompra
                var entOferta = new OfertaCompra
                {
                    ID = GeneradorID.ObtenerID<OfertaCompra>(),
                    Oferente = entOferente,
                    Vehiculo = entVehiculo,
                    FechaInspeccion = dto.FechaInspeccion,
                    Estado = "En evaluación"
                };

                // Persistir oferta
                _ofertaBll.RegistrarOferta(entOferta);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al registrar oferta: {ex.Message}", ex);
            }
        }
    }
}
