using System;
using System.Collections.Generic;

namespace AutoGestion.Servicios.Composite

{
    [Serializable]
    public class PermisoCompuesto : IPermiso
    {
        // Identificador único para el permiso compuesto.
        public int ID { get; set; } 
        
        public string Nombre { get; set; }

        // Lista interna de hijos: pueden ser PermisoSimple o más PermisoCompuesto
        private List<IPermiso> _hijos = new();

        public void Agregar(IPermiso permiso)
        {
            _hijos.Add(permiso);
        }

        public void Quitar(IPermiso permiso)
        {
            _hijos.Remove(permiso);
        }

        public List<IPermiso> ObtenerHijos()
        {
            return _hijos;
        }
    }
}
