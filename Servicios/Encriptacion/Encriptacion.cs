using System;
using System.Text;
using System.Security.Cryptography;

namespace AutoGestion.Servicios.Encriptacion

{
    public static class Encriptacion
    {
        public static string EncriptarPassword(string pPassword)
        {
            //Convierte la contraseña a un arreglo de bytes usando Unicode
            byte[] encriptado = Encoding.Unicode.GetBytes(pPassword);
            // Codifica el arreglo de bytes a una cadena Base64
            return Convert.ToBase64String(encriptado);
        }

        public static string DesencriptarPassword(this string pPasswordEncriptado)
        {
            // Decodifica la cadena Base64 a un arreglo de bytes
            byte[] desencriptado = Convert.FromBase64String(pPasswordEncriptado);
            // Reconstruye la cadena original a partir del arreglo de bytes usando Unicode
            return Encoding.Unicode.GetString(desencriptado);
        }
    }
}
