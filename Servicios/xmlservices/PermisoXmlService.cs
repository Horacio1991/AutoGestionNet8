using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    /// <summary>
    /// Lee y guarda el catálogo de permisos simples.
    /// </summary>
    public static class PermisoXmlService
    {
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "permisos.xml");

        public static List<PermisoSimple> Leer()
        {
            if (!File.Exists(_ruta))
                return new List<PermisoSimple>();

            using var reader = new StreamReader(_ruta);
            var ser = new XmlSerializer(typeof(List<PermisoSimple>));
            return (List<PermisoSimple>)ser.Deserialize(reader);
        }

        public static void Guardar(List<PermisoSimple> permisos)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
            using var writer = new StreamWriter(_ruta);
            var ser = new XmlSerializer(typeof(List<PermisoSimple>));
            ser.Serialize(writer, permisos);
        }
    }
}
