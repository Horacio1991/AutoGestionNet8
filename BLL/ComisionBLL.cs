// Archivo: AutoGestion.BLL/ComisionBLL.cs
using System;
using System.Collections.Generic;
using System.Linq;
using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using AutoGestion.Servicios.Utilidades;

namespace AutoGestion.BLL
{
    public class ComisionBLL
    {
        private readonly XmlRepository<Comision> _repo = new XmlRepository<Comision>("comisiones.xml");
        private readonly VentaBLL _ventaBll = new VentaBLL();

        /// <summary>
        /// Registra la comisión y devuelve true si tuvo éxito.
        /// </summary>
        public bool Registrar(Comision comision)
        {
            comision.ID = GeneradorID.ObtenerID<Comision>();
            comision.Fecha = DateTime.Now;
            _repo.Agregar(comision);
            return true;
        }

        /// <summary>
        /// Obtiene todas las ventas autorizadas que aún no tienen comisión asociada.
        /// </summary>
        public List<Venta> ObtenerVentasSinComision()
        {
            var todas = _ventaBll.ObtenerVentasAutorizadas();
            var conCom = _repo.ObtenerTodos()
                              .Select(c => c.Venta.ID)
                              .ToHashSet();
            return todas.Where(v => !conCom.Contains(v.ID)).ToList();
        }
    }
}
