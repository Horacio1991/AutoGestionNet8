using AutoGestion.Servicios.Composite;
using AutoGestion.Servicios.Utilidades;
using AutoGestion.Servicios.XmlServices;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // --- Plantillas ---

        public IEnumerable<PermisoCompuesto> GetPlantillas() => _plantillas;

        public void CrearPlantilla(string nombre)
        {
            if (_plantillas.Any(p => p.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase)))
                throw new ApplicationException("La plantilla ya existe.");

            _plantillas.Add(new PermisoCompuesto
            {
                ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                Nombre = nombre
            });
            PersistirPlantillas();
        }

        public void EliminarPermiso(int permisoId)
        {
            // Elimina raíz o cualquier subnodo
            bool removed = _plantillas.RemoveAll(p => p.ID == permisoId) > 0;
            if (!removed)
            {
                // buscar dentro de las plantillas
                foreach (var root in _plantillas)
                {
                    QuitarRecursivo(root, permisoId);
                }
            }
            PersistirPlantillas();
        }

        private bool QuitarRecursivo(PermisoCompuesto padre, int permisoId)
        {
            // Quita de hijos compuestos
            if (padre.HijosCompuestos.RemoveAll(p => p.ID == permisoId) > 0)
                return true;

            // Quita de hijos simples
            if (padre.HijosSimples.RemoveAll(p => p.ID == permisoId) > 0)
                return true;

            // Recurse
            foreach (var child in padre.HijosCompuestos)
                if (QuitarRecursivo(child, permisoId))
                    return true;

            return false;
        }

        public void AgregarItemAPlantilla(int plantillaId, string nombreSubMenu, string nombreItem)
        {
            var root = _plantillas.FirstOrDefault(p => p.ID == plantillaId)
                       ?? throw new ApplicationException("Plantilla no encontrada.");

            // busca o crea submenú
            var sub = root.HijosCompuestos.FirstOrDefault(m => m.Nombre == nombreSubMenu)
                   ?? new PermisoCompuesto
                   {
                       ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                       Nombre = nombreSubMenu
                   }.Also(m => root.Agregar(m));

            // valida duplicado
            if (sub.HijosSimples.Any(s => s.Nombre == nombreItem))
                throw new ApplicationException($"El ítem '{nombreItem}' ya existe en '{nombreSubMenu}'.");

            // agrega
            sub.Agregar(new PermisoSimple
            {
                ID = GeneradorID.ObtenerID<PermisoSimple>(),
                Nombre = nombreItem
            });

            PersistirPlantillas();
        }

        private void PersistirPlantillas()
        {
            PermisoPlantillaXmlService.Guardar(_plantillas);
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
                ID = GeneradorID.ObtenerID<PermisoCompuesto>(),
                Nombre = nombre
            });
            RolXmlService.Guardar(_roles);
        }

        public void ModificarRol(int rolId, string nuevoNombre)
        {
            var rol = _roles.FirstOrDefault(r => r.ID == rolId)
                      ?? throw new ApplicationException("Rol no encontrado.");
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
