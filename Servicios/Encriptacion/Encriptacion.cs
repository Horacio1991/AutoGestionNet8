using System;
using System.Text;
using System.Security.Cryptography;

namespace AutoGestion.Servicios.Encriptacion

{
    public static class Encriptacion
    {
        public static string EncriptarPassword(string pPassword)
        {
            byte[] encriptado = Encoding.Unicode.GetBytes(pPassword);
            return Convert.ToBase64String(encriptado);
        }

        public static string DesencriptarPassword(this string pPasswordEncriptado)
        {
            byte[] desencriptado = Convert.FromBase64String(pPasswordEncriptado);
            return Encoding.Unicode.GetString(desencriptado);
        }
    }
}
