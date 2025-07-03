using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    // Servicio para leer y guardar el catálogo de permisos simples en permisos.xml.
    public static class PermisoXmlService
    {
        // Ruta al archivo XML de permisos simples
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DatosXML",
            "permisos.xml");

        // Lee todos los permisos simples desde el XML.
        public static List<PermisoSimple> Leer()
        {
            try
            {
                if (!File.Exists(_ruta))
                    return new List<PermisoSimple>();

                using var reader = new StreamReader(_ruta);
                var serializer = new XmlSerializer(typeof(List<PermisoSimple>));
                return (List<PermisoSimple>)serializer.Deserialize(reader)!;
            }
            catch (InvalidOperationException ex)
            {
                // Error de deserialización: XML mal formado
                throw new ApplicationException($"El archivo de permisos está corrupto: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al leer permisos: {ex.Message}", ex);
            }
        }

        // Serializa y guarda la lista de permisos simples al XML.
        // Crea el directorio si no existe.
        public static void Guardar(List<PermisoSimple> permisos)
        {
            try
            {
                var dir = Path.GetDirectoryName(_ruta)!;
                Directory.CreateDirectory(dir);

                using var writer = new StreamWriter(_ruta);
                var serializer = new XmlSerializer(typeof(List<PermisoSimple>));
                serializer.Serialize(writer, permisos);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al guardar permisos: {ex.Message}", ex);
            }
        }
    }
}
