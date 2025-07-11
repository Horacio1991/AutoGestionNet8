using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    // Servicio para leer y guardar roles (PermisoCompuesto) en roles.xml.
    // Serializa tanto permisos compuestos como sus hijos simples o compuestos.
    public static class RolXmlService
    {
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DatosXML",
            "roles.xml");

        // Lee todos los roles (PermisoCompuesto) desde el XML.
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
                throw new ApplicationException($"El archivo de roles está corrupto: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al leer roles: {ex.Message}", ex);
            }
        }

        // Serializa y guarda la lista de roles al XML.
        // Crea el directorio si no existe.
        public static void Guardar(List<PermisoCompuesto> roles)
        {
            try
            {
                var dir = Path.GetDirectoryName(_ruta)!;
                Directory.CreateDirectory(dir);

                using var writer = new StreamWriter(_ruta);
                var serializer = new XmlSerializer(typeof(List<PermisoCompuesto>));
                serializer.Serialize(writer, roles);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al guardar roles: {ex.Message}", ex);
            }
        }
    }
}
