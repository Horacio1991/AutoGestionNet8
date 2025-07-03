using AutoGestion.Servicios.Composite;

namespace AutoGestion.Servicios

{
    // Mantiene informacion de la sesion actual del usuario.
    public static class Sesion
    {
        public static Usuario UsuarioActual { get; set; }
    }
}
