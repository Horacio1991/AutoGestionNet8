namespace AutoGestion.DAO.Modelos
{
    //Representa la información de un usuario para serializar

    [Serializable]
    public class UsuarioSerializable
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
       
        //Contraseña encriptada (Base64) que se almacena en el XML.
        public string Clave { get; set; }


        // Nombre del rol asignado al usuario. 
        // Se usa para despues enlazar con un PermisoCompuesto al deserializar.
        public string RolNombre { get; set; }
        public List<string> Permisos { get; set; } = new List<string>();
    }
}
