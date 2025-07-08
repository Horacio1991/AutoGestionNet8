using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;

namespace AutoGestion.CTRL_Vista
{
    public class AsignarRolesController
    {
        private List<PermisoCompuesto> _plantillas;
        private List<PermisoCompuesto> _roles;
        private List<Usuario> _usuarios;

        public AsignarRolesController()
        {
            _plantillas = PermisoPlantillaXmlService.Leer();
            _roles = RolXmlService.Leer();
            _usuarios = UsuarioXmlService.Leer();
        }

        // --- Helpers internos ---

        // Recalcula el próximo ID disponible para PermisoCompuesto,
        // recorriendo todo el árbol de plantillas.
        private int _ObtenerIDGlobal()
        {
            var todas = PermisoPlantillaXmlService.Leer();
            var ids = new List<int>();
            void Recolectar(PermisoCompuesto pc)
            {
                ids.Add(pc.ID);
                foreach (var hijo in pc.HijosCompuestos)
                    Recolectar(hijo);
            }
            foreach (var root in todas) Recolectar(root);
            return ids.Any() ? ids.Max() + 1 : 1;
        }

        // --- Plantillas (menús + submenús + items) ---

        public IEnumerable<PermisoCompuesto> GetPlantillas() => _plantillas;

        public void CrearPlantilla(string nombre)
        {
            if (_plantillas.Any(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("La plantilla ya existe.");

            var nueva = new PermisoCompuesto
            {
                ID = _ObtenerIDGlobal(),
                Nombre = nombre
            };
            _plantillas.Add(nueva);
            PersistirPlantillas();
        }

        public void EliminarPlantilla(int plantillaId)
        {
            _plantillas.RemoveAll(p => p.ID == plantillaId);
            PersistirPlantillas();
        }

        public void AgregarItemAPlantilla(
            int plantillaId,
            string nombreSubMenu,
            string nombreItem)
        {
            var root = _plantillas
                .FirstOrDefault(p => p.ID == plantillaId)
                ?? throw new ApplicationException("Plantilla no encontrada.");

            // 1) Rama de menú (PermisoCompuesto)
            var rama = root.HijosCompuestos
                        .FirstOrDefault(m => m.Nombre.Equals(nombreSubMenu, StringComparison.OrdinalIgnoreCase))
                     ?? new PermisoCompuesto
                     {
                         ID = _ObtenerIDGlobal(),
                         Nombre = nombreSubMenu
                     }.Also(m => root.Agregar(m));

            // 2) Permiso simple
            if (rama.HijosSimples.Any(s => s.Nombre.Equals(nombreItem, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException($"El ítem '{nombreItem}' ya existe en '{nombreSubMenu}'.");

            rama.Agregar(new PermisoSimple
            {
                ID = GeneradorID.ObtenerID<PermisoSimple>(), // sigue centralizado en GeneradorID
                Nombre = nombreItem
            });

            PersistirPlantillas();
        }

        private void PersistirPlantillas()
        {
            PermisoPlantillaXmlService.Guardar(_plantillas);
            // Recargamos para mantener sync los IDs recién asignados
            _plantillas = PermisoPlantillaXmlService.Leer();
        }

        // --- Roles ---

        public IEnumerable<PermisoCompuesto> GetRoles() => _roles;

        public void CrearRol(string nombre)
        {
            if (_roles.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("El rol ya existe.");

            _roles.Add(new PermisoCompuesto
            {
                ID = _ObtenerIDGlobal(),
                Nombre = nombre
            });
            RolXmlService.Guardar(_roles);
        }

        public void ModificarRol(int rolId, string nuevoNombre)
        {
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");
            if (_roles.Any(r => r.Nombre.Equals(nuevoNombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("Ya existe un rol con ese nombre.");
            rol.Nombre = nuevoNombre;
            RolXmlService.Guardar(_roles);
        }

        public void EliminarRol(int rolId)
        {
            _roles.RemoveAll(r => r.ID == rolId);
            RolXmlService.Guardar(_roles);
        }

        public void AsociarPlantillaARol(int rolId, int plantillaId)
        {
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                        ?? throw new ApplicationException("Rol no encontrado.");
            var plant = _plantillas.FirstOrDefault(p => p.ID == plantillaId)
                        ?? throw new ApplicationException("Plantilla no encontrada.");

            if (rol.HijosCompuestos.Any(p => p.ID == plant.ID))
                throw new ApplicationException("Plantilla ya asociada a ese rol.");

            rol.Agregar(plant);
            RolXmlService.Guardar(_roles);
        }

        // --- Usuarios ---

        public IEnumerable<Usuario> GetUsuarios() => _usuarios;

        public void AsignarRolAUsuario(int usuarioId, int rolId)
        {
            var usr = _usuarios.FirstOrDefault(u => u.ID == usuarioId)
                      ?? throw new ApplicationException("Usuario no encontrado.");
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");

            usr.Rol = rol;
            UsuarioXmlService.Guardar(_usuarios);
        }

        public void QuitarRolDeUsuario(int usuarioId)
        {
            var usr = _usuarios.FirstOrDefault(u => u.ID == usuarioId)
                      ?? throw new ApplicationException("Usuario no encontrado.");

            usr.Rol = null;
            UsuarioXmlService.Guardar(_usuarios);
        }
    }

    // Helper inline
    public static class Extensions
    {
        public static T Also<T>(this T self, Action<T> act)
        {
            act(self);
            return self;
        }
    }
}
