using System.Text.Json.Serialization;

namespace Entidades
{
    [Serializable]
    public class Bitacora
    {
        // Fecha y hora del registro
        public DateTime FechaRegistro { get; set; }
        // "backup" o "restore"
        public string Detalle { get; set; }
        //Usuario que realizó la acción
        public int UsuarioID { get; set; }
        // Nombre del usuario que realizó la acción
        public string UsuarioNombre { get; set; }
    }
}
