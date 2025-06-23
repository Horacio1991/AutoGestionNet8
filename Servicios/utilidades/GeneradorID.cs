using AutoGestion.DAO.Modelos;
using System.Xml.Serialization;

namespace AutoGestion.Servicios.Utilidades

{
    public static class GeneradorID
    {
        // Diccionario para mapear tipos a nombres de archivos XML
        // Asocia el nombre de la clase con el nombre del archivo XML correspondiente
        private static readonly Dictionary<string, string> NombresArchivos = new()
        {
            { "Cliente", "clientes.xml" },
            { "Vehiculo", "vehiculos.xml" },
            { "Venta", "ventas.xml" },
            { "Pago", "pagos.xml" },
            { "Factura", "facturas.xml" },
            { "EvaluacionTecnica", "evaluaciones.xml" },
            { "OfertaCompra", "ofertas.xml" },
            { "Tasacion", "tasaciones.xml" },
            { "Turno", "turnos.xml" },
            { "Comision", "comisiones.xml" },
            { "ComprobanteEntrega", "comprobantes.xml" },
            { "Oferente", "oferentes.xml" },
            { "PermisoCompleto", "permisos.xml" },
            { "Usuario", "usuarios.xml" },
            { "PermisoCompuesto" , "roles.xml" }

        };

        public static int ObtenerID<T>()
        {
            //Nombre de la clase (por ejemplo, "Cliente", "Vehiculo", etc.)
            string tipo = typeof(T).Name;

            // Determina el nombre del archivo XML basado en el tipo
            string archivo = NombresArchivos.ContainsKey(tipo)
                ? NombresArchivos[tipo]
                : tipo.ToLower() + "s.xml";

            // Ruta completa al archivo XML
            string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", archivo);

            // Si el archivo no existe, retorna 1 (primer ID disponible)
            if (!File.Exists(ruta))
                return 1;

            // Si es Usuario, usamos UsuarioSerializable
            if (typeof(T).Name == "Usuario")
            {
                var serializer = new XmlSerializer(typeof(List<UsuarioSerializable>));
                using var stream = new FileStream(ruta, FileMode.Open);
                var lista = (List<UsuarioSerializable>)serializer.Deserialize(stream);
                return lista.Any() ? lista.Max(u => u.ID) + 1 : 1;
            }

            // Para otros tipos, usamos el tipo genérico T
            var serializerT = new XmlSerializer(typeof(List<T>));
            using var streamT = new FileStream(ruta, FileMode.Open);
            var listaT = (List<T>)serializerT.Deserialize(streamT);

            var prop = typeof(T).GetProperty("ID");
            if (prop == null) return 1;

            var ids = listaT
                .Select(item => (int?)prop.GetValue(item))
                .Where(id => id.HasValue)
                .Select(id => id.Value);

            return ids.Any() ? ids.Max() + 1 : 1;
        }


    }
}
