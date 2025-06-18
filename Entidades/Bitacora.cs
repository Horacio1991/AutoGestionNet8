using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class Bitacora
    {
        public DateTime FechaRegistro { get; set; }
        public string Detalle { get; set; } // "backup" o "restore"
        public int UsuarioID { get; set; }
        public string UsuarioNombre { get; set; }
    }
}
