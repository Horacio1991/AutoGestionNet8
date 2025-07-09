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
            // Carga inicial desde los XML
            _plantillas = PermisoPlantillaXmlService.Leer();
            _roles = RolXmlService.Leer();
            _usuarios = UsuarioXmlService.Leer();
        }

        #region Plantillas (menús + submenús + items)

        /// <summary>
        /// Obtiene todas las plantillas raíz.
        /// </summary>
        public IEnumerable<PermisoCompuesto> GetPlantillas() => _plantillas;

        /// <summary>
        /// Crea una nueva plantilla raíz con nombre único.
        /// </summary>
        public void CrearPlantilla(string nombre)
        {
            if (_plantillas.Any(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("La plantilla ya existe.");

            var nueva = new PermisoCompuesto
            {
                ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                Nombre = nombre
            };
            _plantillas.Add(nueva);
            PersistirPlantillas();
        }

        /// <summary>
        /// Elimina la plantilla raíz y la desasocia de todos los roles que la tuvieran.
        /// </summary>
        public void EliminarPlantilla(int plantillaId)
        {
            // 1) Quitar de plantillas
            _plantillas.RemoveAll(p => p.ID == plantillaId);
            PersistirPlantillas();

            // 2) Quitar de los roles
            bool cambios = false;
            foreach (var rol in _roles)
            {
                if (EliminarEnProfundidad(rol, plantillaId))
                    cambios = true;
            }
            if (cambios)
                RolXmlService.Guardar(_roles);
        }

        /// <summary>
        /// Agrega a la plantilla indicada un submenú y, opcionalmente, un ítem.
        /// </summary>
        public void AgregarItemAPlantilla(int plantillaId, string nombreSubMenu, string nombreItem)
        {
            var root = _plantillas.FirstOrDefault(p => p.ID == plantillaId)
                       ?? throw new ApplicationException("Plantilla no encontrada.");

            // 1) Submenú (PermisoCompuesto)
            var sub = root.HijosCompuestos.FirstOrDefault(m => m.Nombre == nombreSubMenu);
            if (sub == null)
            {
                sub = new PermisoCompuesto
                {
                    ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                    Nombre = nombreSubMenu
                };
                root.Agregar(sub);
            }

            // 2) Ítem (PermisoSimple)
            if (!string.IsNullOrWhiteSpace(nombreItem))
            {
                if (sub.HijosSimples.Any(s => s.Nombre == nombreItem))
                    throw new ApplicationException($"El ítem '{nombreItem}' ya existe en '{nombreSubMenu}'.");

                sub.Agregar(new PermisoSimple
                {
                    ID = GeneradorID.ObtenerID<PermisoSimple>(),
                    Nombre = nombreItem
                });
            }

            PersistirPlantillas();
        }

        /// <summary>
        /// Guarda las plantillas en disco y recarga la lista interna.
        /// </summary>
        private void PersistirPlantillas()
        {
            PermisoPlantillaXmlService.Guardar(_plantillas);
            _plantillas = PermisoPlantillaXmlService.Leer();
        }

        /// <summary>
        /// Elimina de forma recursiva cualquier PermisoCompuesto con ID == idAEliminar.
        /// </summary>
        private bool EliminarEnProfundidad(PermisoCompuesto padre, int idAEliminar)
        {
            int removed = padre.HijosCompuestos.RemoveAll(c => c.ID == idAEliminar);
            foreach (var hijo in padre.HijosCompuestos)
                if (EliminarEnProfundidad(hijo, idAEliminar))
                    removed++;
            return removed > 0;
        }

        #endregion

        #region Roles

        /// <summary>
        /// Obtiene todos los roles.
        /// </summary>
        public IEnumerable<PermisoCompuesto> GetRoles() => _roles;

        /// <summary>
        /// Crea un nuevo rol vacío.
        /// </summary>
        public void CrearRol(string nombre)
        {
            if (_roles.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("El rol ya existe.");

            _roles.Add(new PermisoCompuesto
            {
                ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                Nombre = nombre
            });
            RolXmlService.Guardar(_roles);
        }

        /// <summary>
        /// Modifica el nombre de un rol existente.
        /// </summary>
        public void ModificarRol(int rolId, string nuevoNombre)
        {
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");

            if (_roles.Any(r => r.Nombre.Equals(nuevoNombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("Ya existe un rol con ese nombre.");

            rol.Nombre = nuevoNombre;
            RolXmlService.Guardar(_roles);
        }

        /// <summary>
        /// Elimina un rol.
        /// </summary>
        public void EliminarRol(int rolId)
        {
            _roles.RemoveAll(r => r.ID == rolId);
            RolXmlService.Guardar(_roles);
        }

        /// <summary>
        /// Asocia toda la plantilla (árbol completo) como hijo de un rol.
        /// </summary>
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

        /// <summary>
        /// Desasocia (quita) una plantilla de un rol.
        /// </summary>
        public void QuitarPlantillaDeRol(int rolId, int plantillaId)
        {
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");

            // Se quita por ID, usando el mecanismo interno de PermisoCompuesto
            rol.Quitar(new PermisoCompuesto { ID = plantillaId });
            RolXmlService.Guardar(_roles);
        }

        #endregion

        #region Usuarios

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        public IEnumerable<Usuario> GetUsuarios() => _usuarios;

        /// <summary>
        /// Asigna un rol a un usuario.
        /// </summary>
        public void AsignarRolAUsuario(int usuarioId, int rolId)
        {
            var usr = _usuarios.FirstOrDefault(u => u.ID == usuarioId)
                      ?? throw new ApplicationException("Usuario no encontrado.");
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");

            usr.Rol = rol;
            UsuarioXmlService.Guardar(_usuarios);
        }

        /// <summary>
        /// Quita el rol asignado a un usuario.
        /// </summary>
        public void QuitarRolDeUsuario(int usuarioId)
        {
            var usr = _usuarios.FirstOrDefault(u => u.ID == usuarioId)
                      ?? throw new ApplicationException("Usuario no encontrado.");

            usr.Rol = null;
            UsuarioXmlService.Guardar(_usuarios);
        }

        #endregion
    }
}
