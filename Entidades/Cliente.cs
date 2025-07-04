namespace AutoGestion.Entidades
{
    [Serializable]
    public class Cliente
    {
        public int ID { get; set; }
        public string Dni { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Contacto { get; set; }
        // Fecha y hora de registro del cliente. Se inicializa automaticamente cuando se instanacia
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
