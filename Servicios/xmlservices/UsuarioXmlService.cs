using System.Xml.Serialization;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.DAO.Modelos;

namespace AutoGestion.Servicios.XmlServices
{
    public static class UsuarioXmlService
    {
        private static readonly string ruta =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "usuarios.xml");

        public static List<Usuario> Leer()
        {
            if (!File.Exists(ruta))
                return AsegurarAdmin(new List<Usuario>());

            using var fs = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read);
            var ser = new XmlSerializer(typeof(List<UsuarioSerializable>));
            var serializables = (List<UsuarioSerializable>)ser.Deserialize(fs)!;

            // Leemos roles persistidos
            var roles = RolXmlService.Leer();
            var permisos = PermisoXmlService.Leer(); // permisos simples

            var lista = new List<Usuario>();
            foreach (var s in serializables)
            {
                var u = new Usuario
                {
                    ID = s.ID,
                    Nombre = s.Nombre,
                    Clave = s.Clave  // ya viene encriptada
                };

                // Asignamos rol si existe
                if (!string.IsNullOrEmpty(s.RolNombre))
                {
                    var rol = roles.FirstOrDefault(r => r.Nombre == s.RolNombre);
                    u.Rol = rol;
                }

                lista.Add(u);
            }

            return AsegurarAdmin(lista);
        }

        public static void Guardar(List<Usuario> usuarios)
        {
            var serializables = usuarios.Select(u => new UsuarioSerializable
            {
                ID = u.ID,
                Nombre = u.Nombre,
                Clave = u.Clave,      // ya está encriptada
                RolNombre = (u.Rol as PermisoCompuesto)?.Nombre
            }).ToList();

            using var writer = new StreamWriter(ruta);
            var ser = new XmlSerializer(typeof(List<UsuarioSerializable>));
            ser.Serialize(writer, serializables);
        }

        public static void Eliminar(string nombreUsuario)
        {
            var lista = Leer();
            lista.RemoveAll(u => u.Nombre.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));
            Guardar(lista);
        }

        private static List<Usuario> AsegurarAdmin(List<Usuario> lista)
        {
            if (lista.Any(u => u.Nombre.Equals("admin", StringComparison.OrdinalIgnoreCase)))
                return lista;

            var super = new PermisoCompuesto { Nombre = "SuperAdmin" };
            foreach (var r in PermisoXmlService.Leer())
                super.Agregar(r);

            var admin = new Usuario
            {
                ID = GeneradorID.ObtenerID<Usuario>(),
                Nombre = "admin",
                Clave = Encriptacion.Encriptacion.EncriptarPassword("123"),
                Rol = super
            };
            lista.Add(admin);
            Guardar(lista);
            return lista;
        }
        
    }
}
