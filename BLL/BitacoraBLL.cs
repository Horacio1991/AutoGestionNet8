using Entidades;
using AutoGestion.DAO.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL
{
    public class BitacoraBLL
    {
        private readonly XmlRepository<Bitacora> _repo;

        public BitacoraBLL()
        {
            _repo = new XmlRepository<Bitacora>("bitacora.xml");
        }

        public void Registrar(string tipo, int usuarioID, string usuarioNombre)
        {
            var registro = new Bitacora
            {
                FechaRegistro = DateTime.Now,
                Detalle = tipo.ToLower(), // "backup" o "restore"
                UsuarioID = usuarioID,
                UsuarioNombre = usuarioNombre
            };

            _repo.Agregar(registro);
        }

        public List<Bitacora> ObtenerTodos()
        {
            return _repo.ObtenerTodos();
        }

        public List<Bitacora> FiltrarPorTipo(string tipo)
        {
            return _repo.ObtenerTodos()
                        .Where(b => b.Detalle.Equals(tipo, StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }

        public List<Bitacora> FiltrarPorRango(DateTime desde, DateTime hasta)
        {
            return _repo.ObtenerTodos()
                        .Where(b => b.FechaRegistro >= desde && b.FechaRegistro <= hasta)
                        .ToList();
        }

        public List<Bitacora> FiltrarPorUsuario(string nombre)
        {
            return _repo.ObtenerTodos()
                        .Where(b => b.UsuarioNombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }
    }
}
