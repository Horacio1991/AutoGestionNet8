using System.Xml.Serialization;
using AutoGestion.DAO.Modelos;
using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;




namespace AutoGestion.Servicios.XmlServices

{
    public static class UsuarioXmlService
    {
        private static string ruta = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML", "usuarios.xml");

        public static void Guardar(List<Usuario> usuarios)
        {
            var serializables = usuarios.Select(u =>
            {
                var permisos = ObtenerPermisosIndividuales(u.Rol);

                return new UsuarioSerializable
                {
                    ID = u.ID,
                    Nombre = u.Nombre,
                    Clave = u.Clave,
                    RolNombre = (u.Rol is PermisoCompuesto pc) ? pc.Nombre : null,
                    Permisos = permisos
                };
            }).ToList();

            using var writer = new StreamWriter(ruta);
            var serializer = new XmlSerializer(typeof(List<UsuarioSerializable>));
            serializer.Serialize(writer, serializables);
        }

        public static List<Usuario> Leer()
        {
            if (!File.Exists(ruta))
                return AgregarUsuarioAdmin(new List<Usuario>());

            using var stream = new FileStream(ruta, FileMode.Open, FileAccess.Read, FileShare.Read);
            var serializer = new XmlSerializer(typeof(List<UsuarioSerializable>));
            var serializables = (List<UsuarioSerializable>)serializer.Deserialize(stream);

            List<Usuario> usuarios = new();
            var roles = RolXmlService.Leer();
            var permisos = PermisoCompletoXmlService.Leer();

            foreach (var s in serializables)
            {
                var usuario = new Usuario
                {
                    ID = s.ID,
                    Nombre = s.Nombre,
                    Clave = s.Clave
                };

                // Rol por nombre (si existe)
                if (!string.IsNullOrEmpty(s.RolNombre))
                {
                    var rol = roles.FirstOrDefault(r => r.Nombre == s.RolNombre);
                    if (rol != null)
                        usuario.Rol = rol;
                }

                // Permisos individuales adicionales
                if (s.Permisos.Any())
                {
                    var permisoCompuesto = new PermisoCompuesto { Nombre = "Permisos Individuales" };

                    foreach (var nombre in s.Permisos)
                    {
                        var permiso = permisos.FirstOrDefault(p => p.Nombre == nombre);
                        if (permiso != null)
                            permisoCompuesto.Agregar(permiso);
                    }

                    if (usuario.Rol != null && usuario.Rol is PermisoCompuesto existente)
                    {
                        foreach (var p in permisoCompuesto.ObtenerHijos())
                            existente.Agregar(p);
                    }
                    else
                    {
                        usuario.Rol = permisoCompuesto;
                    }
                }

                usuarios.Add(usuario);
            }

            return AgregarUsuarioAdmin(usuarios);
        }

        public static void Eliminar(string nombreUsuario)
        {
            var usuarios = Leer();
            usuarios.RemoveAll(u => u.Nombre.Equals(nombreUsuario, StringComparison.OrdinalIgnoreCase));
            Guardar(usuarios);
        }

        // ✅ Extrae solo nombres de permisos individuales
        private static List<string> ObtenerPermisosIndividuales(IPermiso permiso)
        {
            List<string> permisos = new();

            if (permiso == null)
                return permisos;

            foreach (var hijo in permiso.ObtenerHijos())
            {
                if (hijo is PermisoCompleto pc)
                    permisos.Add(pc.Nombre);
            }

            return permisos;
        }

        private static List<Usuario> AgregarUsuarioAdmin(List<Usuario> lista)
        {
            if (!lista.Any(u => u.Nombre.Equals("admin", StringComparison.OrdinalIgnoreCase)))
            {
                var permisos = PermisoCompletoXmlService.Leer();

                var todos = new PermisoCompuesto
                {
                    Nombre = "SuperAdmin"
                };

                foreach (var permiso in permisos)
                    todos.Agregar(permiso);

                lista.Add(new Usuario
                {
                    ID = GeneradorID.ObtenerID<Usuario>(),
                    Nombre = "admin",
                    Clave = EncriptarPassword("admin123"), // Corrected to use the local EncriptarPassword method  
                    Rol = todos
                });

                Guardar(lista);
            }

            return lista;
        }

        private static string EncriptarPassword(string password)
        {
            // Implementación básica de encriptación de contraseña
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
