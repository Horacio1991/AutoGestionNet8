using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    /// <summary>
    /// Lee y guarda la lista de roles (PermisoCompuesto),
    /// serializando también sus hijos (PermisoSimple o PermisoCompuesto).
    /// </summary>
    public static class RolXmlService
    {
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "roles.xml");

        public static List<PermisoCompuesto> Leer()
        {
            if (!File.Exists(_ruta))
                return new List<PermisoCompuesto>();

            using var reader = new StreamReader(_ruta);
            var ser = new XmlSerializer(typeof(List<PermisoCompuesto>));
            return (List<PermisoCompuesto>)ser.Deserialize(reader);
        }

        public static void Guardar(List<PermisoCompuesto> roles)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
            using var writer = new StreamWriter(_ruta);
            var ser = new XmlSerializer(typeof(List<PermisoCompuesto>));
            ser.Serialize(writer, roles);
        }
    }
}
