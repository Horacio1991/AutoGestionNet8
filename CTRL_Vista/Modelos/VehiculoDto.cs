namespace AutoGestion.CTRL_Vista.Modelos
{
    // DTO que expone sólo lo que la vista necesita
    public class VehiculoDto
    {
        public string Dominio { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public string Color { get; set; }

        public int Km { get; set; }

        // Mapea de la entidad a este DTO
        public static VehiculoDto FromEntity(Entidades.Vehiculo v) => new VehiculoDto
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
