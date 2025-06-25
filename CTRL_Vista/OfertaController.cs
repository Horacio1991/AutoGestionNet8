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

        /// <summary> Busca un oferente por DNI y lo retorna como DTO, o null. </summary>
        public OferenteDto BuscarOferente(string dni)
        {
            var ent = _oferenteBll.BuscarPorDni(dni);
            return ent is null
                ? null
                : OferenteDto.FromEntity(ent);
        }

        /// <summary> Registra un nuevo oferente y devuelve su DTO. </summary>
        public OferenteDto RegistrarOferente(OferenteDto dto)
        {
            var ent = new Ent.Oferente
            {
                ID = GeneradorID.ObtenerID<Ent.Oferente>(),
                Dni = dto.Dni,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Contacto = dto.Contacto
            };
            _oferenteBll.GuardarOferente(ent);
            return OferenteDto.FromEntity(ent);
        }

        /// <summary> Crea y guarda una nueva oferta (y su vehículo). </summary>
        public bool RegistrarOferta(OfertaInputDto dto)
        {
            // 1) Asegura que el oferente exista
            var entOferente = _oferenteBll.BuscarPorDni(dto.Oferente.Dni)
                              ?? new Ent.Oferente
                              {
                                  ID = GeneradorID.ObtenerID<Ent.Oferente>(),
                                  Dni = dto.Oferente.Dni,
                                  Nombre = dto.Oferente.Nombre,
                                  Apellido = dto.Oferente.Apellido,
                                  Contacto = dto.Oferente.Contacto
                              };
            if (entOferente.ID == 0)
                _oferenteBll.GuardarOferente(entOferente);

            // 2) Genera ID para el vehículo
            var previas = _ofertaBll.ObtenerTodas();
            int lastVehId = previas
                .Select(o => o.Vehiculo?.ID ?? 0)
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

            // 3) Arma la oferta
            var entOferta = new Ent.OfertaCompra
            {
                ID = GeneradorID.ObtenerID<Ent.OfertaCompra>(),
                Oferente = entOferente,
                Vehiculo = entVeh,
                FechaInspeccion = dto.FechaInspeccion
            };

            // 4) Guarda
            _ofertaBll.RegistrarOferta(entOferta);
            return true;
        }
    }
}
