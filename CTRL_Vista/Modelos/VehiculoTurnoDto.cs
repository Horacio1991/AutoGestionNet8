namespace AutoGestion.DTOs
{
    public class VehiculoTurnoDto
    {
        public int ID { get; set; }
        public string Dominio { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }

        public static VehiculoTurnoDto FromEntity(Entidades.Vehiculo v) => new()
        {
            ID = v.ID,
            Dominio = v.Dominio,
            Marca = v.Marca,
            Modelo = v.Modelo
        };
    }
}
