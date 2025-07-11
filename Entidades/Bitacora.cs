namespace Entidades
{
    [Serializable]
    public class Bitacora
    {
        public DateTime FechaRegistro { get; set; }
        public string Detalle { get; set; } // "backup" o "restore"
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; }
    }
}
