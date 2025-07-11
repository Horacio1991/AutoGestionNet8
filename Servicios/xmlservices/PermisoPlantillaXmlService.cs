using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    // Servicio para leer y guardar plantillas de permisos compuestos
    public static class PermisoPlantillaXmlService
    {
        // donde voy a guardar las plantillas de permisos compuestos
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

                using var reader = new StreamReader(_ruta); //Abro archivo
                var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>)); // Serializador xml y la lista
                return (List<PermisoCompuesto>)serializer.Deserialize(reader)!; // paso de xml a objeto en memoria 
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

                using var writer = new StreamWriter(_ruta); // abro el archivo para escribir
                var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>)); // Serializador xml y la lista
                serializer.Serialize(writer, plantillas); // Lo serializo y guardo en el archivo
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al guardar plantillas de permisos: {ex.Message}", ex);
            }
        }
    }
}
