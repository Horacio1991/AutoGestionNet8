using System;
using System.Collections.Generic;

namespace AutoGestion.Servicios.Composite

{
    [Serializable]
    public class PermisoCompuesto : IPermiso
    {
        public int ID { get; set; } // ✅ NECESARIO PARA GeneradorID

        public string Nombre { get; set; }

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
