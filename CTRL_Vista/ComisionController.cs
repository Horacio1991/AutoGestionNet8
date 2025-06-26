// Archivo: AutoGestion.CTRL_Vista/ComisionController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using AutoGestion.BLL;
using AutoGestion.DTOs;
using AutoGestion.Entidades;

namespace AutoGestion.CTRL_Vista
{
    public class ComisionController
    {
        private readonly ComisionBLL _comisionBll = new ComisionBLL();
        private readonly VentaBLL _ventaBll = new VentaBLL();

        /// <summary> Lista de ventas sin comisión. </summary>
        public List<VentaComisionDto> ObtenerVentasSinComision()
        {
            var ventas = _comisionBll.ObtenerVentasSinComision();
            return ventas.Select(v => new VentaComisionDto
            {
                VentaID = v.ID,
                Cliente = $"{v.Cliente.Nombre} {v.Cliente.Apellido}",
                Vendedor = v.Vendedor.Nombre,
                VehiculoResumen = $"{v.Vehiculo.Marca} {v.Vehiculo.Modelo} ({v.Vehiculo.Dominio})",
                MontoVenta = v.Total,
                ComisionSugerida = Math.Round(v.Total * 0.05m, 2),
                FechaVenta = v.Fecha.ToShortDateString()
            }).ToList();
        }

        /// <summary> Registra la comisión (aprobada o rechazada). </summary>
        public bool RegistrarComision(ComisionInputDto dto)
        {
            var venta = _ventaBll.ObtenerDetalleVenta(dto.VentaID);
            if (venta == null)
                throw new ApplicationException("Venta no encontrada.");

            var com = new Comision
            {
                Venta = venta,
                Porcentaje = dto.Monto / venta.Total,
                Monto = dto.Monto,
                Estado = dto.Estado,
                Fecha = DateTime.Now
            };

            if (dto.Estado == "Rechazada")
                com.MotivoRechazo = dto.MotivoRechazo;

            return _comisionBll.Registrar(com);
        }
    }
}
