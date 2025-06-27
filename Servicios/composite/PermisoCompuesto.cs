using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite

{
    /// <summary>
    /// Permiso compuesto (composite). Puede contener hijos (simple o compuesto).
    /// </summary>
    [Serializable]
    public class PermisoCompuesto : IPermiso
    {
        public int ID { get; set; }
        public string Nombre { get; set; }

        // Lista de hijos (PermisoSimple o PermisoCompuesto)
        [XmlArray("Hijos")]
        [XmlArrayItem("Permiso")]
        private readonly List<IPermiso> _hijos = new();

        public void Agregar(IPermiso permiso)
        {
            if (permiso == null) throw new ArgumentNullException(nameof(permiso));
            _hijos.Add(permiso);
        }

        public void Quitar(IPermiso permiso)
        {
            if (permiso == null) throw new ArgumentNullException(nameof(permiso));
            _hijos.Remove(permiso);
        }

        public List<IPermiso> ObtenerHijos() => new List<IPermiso>(_hijos);
    }
}
