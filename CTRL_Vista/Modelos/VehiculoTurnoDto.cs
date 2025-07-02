using AutoGestion.Entidades;

namespace AutoGestion.DTOs
{
    /// DTO para listar vehículos en el formulario de registro de turnos,
    /// con los campos: ID, dominio, marca y modelo.
    public class VehiculoTurnoDto
    {
        //ID del vehículo 
        public int ID { get; set; }

        public string Dominio { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        /// Mapea una entidad Vehiculo a este DTO.
        // v = Entidad Vehiculo a mapear;
        public static VehiculoTurnoDto FromEntity(Vehiculo v)
        {
            if (v == null) return null;

            return new VehiculoTurnoDto
            {
                ID = v.ID,
                Dominio = v.Dominio,
                Marca = v.Marca,
                Modelo = v.Modelo
            };
        }
    }
}
