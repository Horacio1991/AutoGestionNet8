using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Servicios.Utilidades;
using Ent = AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class OfertaController
    {
        private readonly OferenteBLL _oferenteBll = new();
        private readonly OfertaBLL _ofertaBll = new();

        /// <summary>
        /// Busca un oferente por DNI y lo retorna como DTO, o null si no existe.
        /// </summary>
        public OferenteDto BuscarOferente(string dni)
        {
            var entidad = _oferenteBll.BuscarPorDni(dni);
            return entidad is null
                ? null
                : OferenteDto.FromEntity(entidad);
        }

        /// <summary>
        /// Registra un oferente nuevo (si no existía) y la oferta.
        /// </summary>
        public bool RegistrarOferta(OfertaInputDto dto)
        {
            // 1) Busco al oferente; si no existe, lo creo y guardo
            var entOferente = _oferenteBll.BuscarPorDni(dto.Oferente.Dni);
            if (entOferente == null)
            {
                entOferente = new Ent.Oferente
                {
                    ID = GeneradorID.ObtenerID<Ent.Oferente>(),
                    Dni = dto.Oferente.Dni,
                    Nombre = dto.Oferente.Nombre,
                    Apellido = dto.Oferente.Apellido,
                    Contacto = dto.Oferente.Contacto
                };
                _oferenteBll.GuardarOferente(entOferente);
            }

            // 2) Genero ID para el vehículo basándome en las ofertas previas
            var previas = _ofertaBll.ObtenerTodas();
            int lastVehId = previas.Select(o => o.Vehiculo?.ID ?? 0)
                                  .DefaultIfEmpty(0)
                                  .Max();

            var entVeh = new Ent.Vehiculo
            {
                ID = lastVehId + 1,
                Marca = dto.Vehiculo.Marca,
                Modelo = dto.Vehiculo.Modelo,
                Año = dto.Vehiculo.Año,
                Color = dto.Vehiculo.Color,
                Dominio = dto.Vehiculo.Dominio,
                Km = dto.Vehiculo.Km,
                Estado = "En evaluación"
            };

            // 3) Armo la oferta con su nuevo ID
            var entOferta = new Ent.OfertaCompra
            {
                ID = GeneradorID.ObtenerID<Ent.OfertaCompra>(),
                Oferente = entOferente,
                Vehiculo = entVeh,
                FechaInspeccion = dto.FechaInspeccion
            };

            // 4) Guardo la oferta
            _ofertaBll.RegistrarOferta(entOferta);

            return true;
        }
    }
}
