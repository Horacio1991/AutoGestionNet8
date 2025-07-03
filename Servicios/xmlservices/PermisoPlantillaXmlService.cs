using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    // Servicio para leer y guardar plantillas de permisos compuestos
    // en permisos_compuestos.xml.
    public static class PermisoPlantillaXmlService
    {
        // Ruta al archivo XML de plantillas de permisos compuestos
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DatosXML",
            "permisos_compuestos.xml");

        // Lee todas las plantillas de permisos compuestos desde el XML.
        // Si el archivo no existe, retorna una lista vacía.
        public static List<PermisoCompuesto> Leer()
        {
            try
            {
                if (!File.Exists(_ruta))
                    return new List<PermisoCompuesto>();

                using var reader = new StreamReader(_ruta);
                var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
                return (List<PermisoCompuesto>)serializer.Deserialize(reader)!;
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException($"El archivo de plantillas está corrupto: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al leer plantillas de permisos: {ex.Message}", ex);
            }
        }

        // Serializa y guarda la lista de plantillas de permisos compuestos al XML.
        // Crea el directorio si no existe.
        public static void Guardar(List<PermisoCompuesto> plantillas)
        {
            try
            {
                var dir = Path.GetDirectoryName(_ruta)!;
                Directory.CreateDirectory(dir);

                using var writer = new StreamWriter(_ruta);
                var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
                serializer.Serialize(writer, plantillas);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al guardar plantillas de permisos: {ex.Message}", ex);
            }
        }
    }
}
