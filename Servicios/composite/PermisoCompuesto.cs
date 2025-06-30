using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.Composite
{
    [XmlInclude(typeof(PermisoSimple))]
    [XmlInclude(typeof(PermisoCompuesto))]
    [Serializable]
    public class PermisoCompuesto : IPermiso
    {
        public PermisoCompuesto() { }

        [XmlElement]
        public int ID { get; set; }

        [XmlElement]
        public string Nombre { get; set; }

        // Hijos compuestos (sub-menús, plantillas)
        [XmlArray("HijosCompuestos")]
        [XmlArrayItem("PermisoCompuesto", typeof(PermisoCompuesto))]
        public List<PermisoCompuesto> HijosCompuestos { get; set; } = new();

        // Hijos simples (acciones)
        [XmlArray("HijosSimples")]
        [XmlArrayItem("PermisoSimple", typeof(PermisoSimple))]
        public List<PermisoSimple> HijosSimples { get; set; } = new();

        // Esta propiedad no se serializa, sirve para tu lógica de UI
        [XmlIgnore]
        public List<IPermiso> Hijos
        {
            get
            {
                var all = new List<IPermiso>();
                all.AddRange(HijosCompuestos);
                all.AddRange(HijosSimples);
                return all;
            }
        }

        public void Agregar(IPermiso permiso)
        {
            switch (permiso)
            {
                case PermisoCompuesto pc: HijosCompuestos.Add(pc); break;
                case PermisoSimple ps: HijosSimples.Add(ps); break;
                default: throw new ArgumentException("Tipo de permiso no soportado", nameof(permiso));
            }
        }

        public void Quitar(IPermiso permiso)
        {
            switch (permiso)
            {
                case PermisoCompuesto pc: HijosCompuestos.RemoveAll(x => x.ID == pc.ID); break;
                case PermisoSimple ps: HijosSimples.RemoveAll(x => x.ID == ps.ID); break;
            }
        }

        public List<IPermiso> ObtenerHijos() => new(Hijos);
    }
}
