using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista.Modelos
{
    public class VehiculoDto
    {
        public string Dominio { get; set; }

        public string Marca { get; set; }

        public string Modelo { get; set; }

        public int Año { get; set; }

        public string Color { get; set; }

        public int Km { get; set; }

        // Mappea una entidad Vehiculo a VehiculoDto para la UI.
        // v = Entidad Vehiculo a mapear;
        public static VehiculoDto FromEntity(Vehiculo v)
        {
            if (v == null) return null;

            return new VehiculoDto
            {
                Dominio = v.Dominio,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Año = v.Año,
                Color = v.Color,
                Km = v.Km
            };
        }
    }
}
