using System.Xml.Serialization;
using AutoGestion.DAO.Modelos;

namespace AutoGestion.Servicios.Utilidades
{
    public static class GeneradorID
    {
        // Mapa de tipo de entidad al nombre de archivo XML
        private static readonly Dictionary<string, string> NombresArchivos = new()
        {
            { "Cliente",             "clientes.xml" },
            { "Vehiculo",            "vehiculos.xml" },
            { "Venta",               "ventas.xml" },
            { "Pago",                "pagos.xml" },
            { "Factura",             "facturas.xml" },
            { "EvaluacionTecnica",   "evaluaciones.xml" },
            { "OfertaCompra",        "ofertas.xml" },
            { "Tasacion",            "tasaciones.xml" },
            { "Turno",               "turnos.xml" },
            { "Comision",            "comisiones.xml" },
            { "ComprobanteEntrega",  "comprobantes.xml" },
            { "Oferente",            "oferentes.xml" },
            { "PermisoCompleto",     "permisos_compuestos.xml" },
            { "Usuario",             "usuarios.xml" },
            { "PermisoCompuesto",    "roles.xml" }
        };

        // Obtiene el próximo ID para el tipo T, buscando en el XML respectivo.
        public static int ObtenerID<T>()
        {
            var tipo = typeof(T).Name;
            var archivo = NombresArchivos.ContainsKey(tipo)
                ? NombresArchivos[tipo]
                : $"{tipo.ToLower()}s.xml";

            var ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", archivo);

            try
            {
                if (!File.Exists(ruta))
                    return 1; // primer ID

                // Caso especial: UsuarioSerializable
                if (tipo == "Usuario")
                {
                    var serU = new XmlSerializer(typeof(List<UsuarioSerializable>));
                    using var streamU = new FileStream(ruta, FileMode.Open, FileAccess.Read);
                    var listU = (List<UsuarioSerializable>)serU.Deserialize(streamU);
                    return listU.Any() ? listU.Max(u => u.ID) + 1 : 1;
                }

                // Serializar lista genérica T
                var ser = new XmlSerializer(typeof(List<T>));
                using var stream = new FileStream(ruta, FileMode.Open, FileAccess.Read);
                var list = (List<T>)ser.Deserialize(stream);

                // Obtener propiedad ID vía reflexión
                var prop = typeof(T).GetProperty("ID")
                    ?? throw new InvalidOperationException($"Tipo {tipo} no tiene propiedad ID.");

                var ids = list
                    .Select(item => (int?)prop.GetValue(item))
                    .Where(id => id.HasValue)
                    .Select(id => id.Value);

                return ids.Any() ? ids.Max() + 1 : 1;
            }
            catch (InvalidOperationException ex)
            {
                // XML mal formado u otro problema de deserialización
                throw new ApplicationException($"Error al leer IDs de {archivo}: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al generar ID para {tipo}: {ex.Message}", ex);
            }
        }
    }
}
