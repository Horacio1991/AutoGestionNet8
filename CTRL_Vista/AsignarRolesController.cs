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
            RecargarTodo();
        }

        // Fuerza la recarga desde los XML de plantillas, roles y usuarios.
        public void RecargarTodo()
        {
            _plantillas = PermisoPlantillaXmlService.Leer();
            _roles = RolXmlService.Leer();
            _usuarios = UsuarioXmlService.Leer();
        }

        #region Plantillas

        /// <summary>Devuelve todas las plantillas raíz.</summary>
        public IEnumerable<PermisoCompuesto> GetPlantillas() => _plantillas;

        /// <summary>
        /// Crea una nueva plantilla raíz con un ID único en todo el conjunto.
        /// </summary>
        public void CrearPlantilla(string nombre)
        {
            if (_plantillas.Any(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("La plantilla ya existe.");

            int nextId = FlattenCompuestos().Any()
                ? FlattenCompuestos().Max(c => c.ID) + 1
                : 1;

            _plantillas.Add(new PermisoCompuesto { ID = nextId, Nombre = nombre });
            PersistirPlantillas();
        }

        /// <summary>
        /// Elimina la plantilla raíz, desasocia de roles y limpia usuarios afectados.
        /// </summary>
        public void EliminarPlantilla(int plantillaId)
        {
            // 1) Quitar de plantillas
            _plantillas.RemoveAll(p => p.ID == plantillaId);
            PersistirPlantillas();

            // 2) Desasociar de roles
            bool rolesMod = false;
            foreach (var rol in _roles)
                if (EliminarEnProfundidad(rol, plantillaId))
                    rolesMod = true;
            if (rolesMod) RolXmlService.Guardar(_roles);

            // 3) Desasignar rol en usuarios afectados
            bool usrMod = false;
            foreach (var u in _usuarios)
            {
                if (u.Rol is PermisoCompuesto r && r.HijosCompuestos.Any(pc => pc.ID == plantillaId))
                {
                    u.Rol = null;
                    usrMod = true;
                }
            }
            if (usrMod) UsuarioXmlService.Guardar(_usuarios);
        }

        /// <summary>
        /// Agrega un submenú (y opcionalmente un ítem) a la plantilla dada.
        /// IDs globales para evitar colisiones.
        /// </summary>
        public void AgregarItemAPlantilla(int plantillaId, string nombreSubMenu, string nombreItem)
        {
            var root = _plantillas.FirstOrDefault(p => p.ID == plantillaId)
                       ?? throw new ApplicationException("Plantilla no encontrada.");

            // 1) Submenú
            var sub = root.HijosCompuestos.FirstOrDefault(m => m.Nombre == nombreSubMenu);
            if (sub == null)
            {
                int nextComp = FlattenCompuestos().Max(c => c.ID) + 1;
                sub = new PermisoCompuesto { ID = nextComp, Nombre = nombreSubMenu };
                root.Agregar(sub);
            }

            // 2) Ítem simple (si existe nombre)
            if (!string.IsNullOrWhiteSpace(nombreItem))
            {
                if (sub.HijosSimples.Any(s => s.Nombre == nombreItem))
                    throw new ApplicationException($"El ítem '{nombreItem}' ya existe en '{nombreSubMenu}'.");

                int nextSimple = FlattenSimples().Max(s => s.ID) + 1;
                sub.Agregar(new PermisoSimple { ID = nextSimple, Nombre = nombreItem });
            }

            PersistirPlantillas();
        }

        /// <summary>
        /// Guarda y recarga plantillas desde XML.
        /// </summary>
        private void PersistirPlantillas()
        {
            PermisoPlantillaXmlService.Guardar(_plantillas);
            _plantillas = PermisoPlantillaXmlService.Leer();
        }

        /// <summary>
        /// Elimina recursivamente nodos compuestos con el ID dado.
        /// </summary>
        private bool EliminarEnProfundidad(PermisoCompuesto padre, int idAEliminar)
        {
            int removed = padre.HijosCompuestos.RemoveAll(c => c.ID == idAEliminar);
            foreach (var hijo in padre.HijosCompuestos)
                if (EliminarEnProfundidad(hijo, idAEliminar))
                    removed++;
            return removed > 0;
        }

        /// <summary>Aplana todo el árbol de compuestos.</summary>
        private IEnumerable<PermisoCompuesto> FlattenCompuestos()
        {
            foreach (var root in _plantillas)
                foreach (var pc in FlattenCompuestos(root))
                    yield return pc;
        }

        private IEnumerable<PermisoCompuesto> FlattenCompuestos(PermisoCompuesto p)
        {
            yield return p;
            foreach (var child in p.HijosCompuestos)
                foreach (var desc in FlattenCompuestos(child))
                    yield return desc;
        }

        /// <summary>Aplana todo el árbol de simples.</summary>
        private IEnumerable<PermisoSimple> FlattenSimples()
        {
            foreach (var root in _plantillas)
                foreach (var ps in FlattenSimples(root))
                    yield return ps;
        }

        private IEnumerable<PermisoSimple> FlattenSimples(PermisoCompuesto p)
        {
            foreach (var s in p.HijosSimples)
                yield return s;
            foreach (var child in p.HijosCompuestos)
                foreach (var desc in FlattenSimples(child))
                    yield return desc;
        }

        #endregion

        #region Roles

        public IEnumerable<PermisoCompuesto> GetRoles() => _roles;

        public void CrearRol(string nombre)
        {
            if (_roles.Any(r => r.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("El rol ya existe.");

            int next = _roles.Any() ? _roles.Max(r => r.ID) + 1 : 1;
            _roles.Add(new PermisoCompuesto { ID = next, Nombre = nombre });
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

        /// <summary>
        /// Elimina un rol y lo desasigna de los usuarios que lo tuvieran.
        /// </summary>
        public void EliminarRol(int rolId)
        {
            _roles.RemoveAll(r => r.ID == rolId);
            RolXmlService.Guardar(_roles);

            bool mod = false;
            foreach (var u in _usuarios)
                if (u.Rol?.ID == rolId)
                {
                    u.Rol = null;
                    mod = true;
                }
            if (mod) UsuarioXmlService.Guardar(_usuarios);
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

        public void QuitarPlantillaDeRol(int rolId, int plantillaId)
        {
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");
            rol.Quitar(new PermisoCompuesto { ID = plantillaId });
            RolXmlService.Guardar(_roles);
        }

        #endregion

        #region Usuarios

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

        #endregion
    }
}
