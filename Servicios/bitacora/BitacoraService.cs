using AutoGestion.Entidades;
using AutoGestion.DAO;
using AutoGestion.DAO.Repositorios;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoGestion.Servicios
{
    public static class BitacoraService
    {
        private static readonly XmlRepository<Bitacora> _repo = new("bitacora.xml");

        public static void Registrar(string detalle, int usuarioID, string usuarioNombre)
        {
            var entrada = new Bitacora
            {
                FechaRegistro = DateTime.Now,
                Detalle = detalle,
                UsuarioID = usuarioID,
                UsuarioNombre = usuarioNombre
            };

            _repo.Agregar(entrada);
        }

        public static List<Bitacora> ObtenerTodo()
        {
            return _repo.ObtenerTodos().OrderByDescending(b => b.FechaRegistro).ToList();
        }
    }
}
