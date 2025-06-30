// PermisoPlantillaXmlService.cs
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios.XmlServices
{
    public static class PermisoPlantillaXmlService
    {
        private static readonly string _ruta = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "DatosXML", "permisos_compuestos.xml");  // <--- nuevo nombre

        public static List<PermisoCompuesto> Leer()
        {
            if (!File.Exists(_ruta))
                return new List<PermisoCompuesto>();
            using var reader = new StreamReader(_ruta);
            var ser = new XmlSerializer(typeof(List<PermisoCompuesto>));
            return (List<PermisoCompuesto>)ser.Deserialize(reader)!;
        }

        public static void Guardar(List<PermisoCompuesto> plantillas)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
            using var writer = new StreamWriter(_ruta);
            var ser = new XmlSerializer(typeof(List<PermisoCompuesto>));
            ser.Serialize(writer, plantillas);
        }
    }
}
