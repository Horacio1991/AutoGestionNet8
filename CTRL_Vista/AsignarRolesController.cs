using System;
using System.Collections.Generic;
using System.Linq;
using AutoGestion.Servicios.Composite;
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
            // Carga inicial desde los XML
            _plantillas = PermisoPlantillaXmlService.Leer();
            _roles = RolXmlService.Leer();
            _usuarios = UsuarioXmlService.Leer();
        }

        // --- PLANTILLAS ---

        public IEnumerable<PermisoCompuesto> GetPlantillas() => _plantillas;

        /// <summary>
        /// Crea una nueva plantilla raíz con un ID único dentro de _plantillas.
        /// </summary>
        public void CrearPlantilla(string nombre)
        {
            if (_plantillas.Any(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("La plantilla ya existe.");

            int nuevoId = (_plantillas.Any() ? _plantillas.Max(p => p.ID) : 0) + 1;

            _plantillas.Add(new PermisoCompuesto
            {
                ID = nuevoId,
                Nombre = nombre
            });
            PersistirPlantillas();
        }

        /// <summary>
        /// Elimina la plantilla completa (nodo raíz).
        /// </summary>
        public void EliminarPlantilla(int plantillaId)
        {
            _plantillas.RemoveAll(p => p.ID == plantillaId);
            PersistirPlantillas();
        }

        /// <summary>
        /// Agrega a la plantilla indicada un submenú (PermisoCompuesto) y, opcionalmente, un ítem (PermisoSimple).
        /// Genera IDs únicos para submenú e ítem dentro de esta colección de plantillas.
        /// </summary>
        public void AgregarItemAPlantilla(
            int plantillaId,
            string nombreSubMenu,
            string nombreItem)
        {
            var root = _plantillas
                .FirstOrDefault(p => p.ID == plantillaId)
                ?? throw new ApplicationException("Plantilla no encontrada.");

            // 1) Obtener todos los nodos compuestos existentes para generar un nuevo ID
            var todosCompuestos = _plantillas.SelectMany(p => FlattenCompuestos(p)).ToList();
            int nextCompId = (todosCompuestos.Any() ? todosCompuestos.Max(c => c.ID) : 0) + 1;

            // 2) Buscar o crear el submenú
            var sub = root.HijosCompuestos.FirstOrDefault(m => m.Nombre == nombreSubMenu);
            if (sub == null)
            {
                sub = new PermisoCompuesto
                {
                    ID = nextCompId,
                    Nombre = nombreSubMenu
                };
                root.Agregar(sub);
            }

            // 3) Si se indicó ítem, agregarlo
            if (!string.IsNullOrWhiteSpace(nombreItem))
            {
                if (sub.HijosSimples.Any(s => s.Nombre == nombreItem))
                    throw new ApplicationException($"El ítem '{nombreItem}' ya existe en '{nombreSubMenu}'.");

                // IDs de simples únicos dentro de esta plantilla
                var todosSimples = _plantillas.SelectMany(p => FlattenSimples(p)).ToList();
                int nextSimpleId = (todosSimples.Any() ? todosSimples.Max(s => s.ID) : 0) + 1;

                sub.Agregar(new PermisoSimple
                {
                    ID = nextSimpleId,
                    Nombre = nombreItem
                });
            }

            PersistirPlantillas();
        }

        private void PersistirPlantillas()
        {
            PermisoPlantillaXmlService.Guardar(_plantillas);
            _plantillas = PermisoPlantillaXmlService.Leer();
        }

        // Helpers para aplanar el árbol
        private IEnumerable<PermisoCompuesto> FlattenCompuestos(PermisoCompuesto p)
        {
            yield return p;
            foreach (var child in p.HijosCompuestos)
                foreach (var desc in FlattenCompuestos(child))
                    yield return desc;
        }

        private IEnumerable<PermisoSimple> FlattenSimples(PermisoCompuesto p)
        {
            foreach (var s in p.HijosSimples) yield return s;
            foreach (var child in p.HijosCompuestos)
                foreach (var desc in FlattenSimples(child))
                    yield return desc;
        }

        // --- ROLES ---

        public IEnumerable<PermisoCompuesto> GetRoles() => _roles;

        public void CrearRol(string nombre)
        {
            if (_roles.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("El rol ya existe.");

            int nuevoId = (_roles.Any() ? _roles.Max(r => r.ID) : 0) + 1;
            _roles.Add(new PermisoCompuesto
            {
                ID = nuevoId,
                Nombre = nombre
            });
            RolXmlService.Guardar(_roles);
        }

        public void EliminarRol(int rolId)
        {
            _roles.RemoveAll(r => r.ID == rolId);
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

        // --- USUARIOS ---

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
}
