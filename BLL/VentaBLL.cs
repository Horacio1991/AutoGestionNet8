﻿using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;


namespace AutoGestion.BLL
{
    public class VentaBLL
    {
        // Repositorio XML donde se guardan todas las ventas
        private readonly XmlRepository<Venta> _repo = new("ventas.xml");
        private readonly VehiculoBLL _vehiculoBLL = new();

        public List<Venta> ObtenerTodas()
        {
            return _repo.ObtenerTodos();
        }

        // obtiene todas las ventas pendientes (no facturadas)
        public List<Venta> ObtenerVentasPendientes()
        {
            return _repo.ObtenerTodos()
                        .Where(v => v.Estado != "Facturada")
                        .ToList();
        }

        public List<Venta> ObtenerVentasConEstadoPendiente()
        {
            return _repo.ObtenerTodos()
                        .Where(v => v.Estado == "Pendiente")
                        .ToList();
        }

        public Venta ObtenerDetalleVenta(int id)
        {
            return _repo.ObtenerTodos()
                        .FirstOrDefault(v => v.ID == id);
        }

        // Cambia estado de venta a Autorizada o la rechaza si ya hay una venta autorizada para el mismo vehículo
        public bool AutorizarVenta(int ventaId)
        {
            var lista = _repo.ObtenerTodos();
            var venta = lista.FirstOrDefault(v => v.ID == ventaId);

            if (venta == null) return false;

            // Verifica si ya hay una venta autorizada para el mismo vehículo
            bool yaVendida = lista.Any(v =>
                v.Estado == "Autorizada" &&
                v.Vehiculo?.Dominio == venta.Vehiculo?.Dominio
            );

            if (yaVendida)
            {
                venta.Estado = "Rechazada";
                venta.MotivoRechazo = "Vehículo ya vendido en otra operación.";
                _repo.GuardarLista(lista);
                return false;
            }

            venta.Estado = "Autorizada";
            _repo.GuardarLista(lista);
            return true;
        }

        public bool RechazarVenta(int ventaId, string motivo)
        {
            var lista = _repo.ObtenerTodos();
            var venta = lista.FirstOrDefault(v => v.ID == ventaId);

            if (venta == null) return false;

            venta.Estado = "Rechazada";
            venta.MotivoRechazo = motivo;

            _vehiculoBLL.ActualizarEstadoVehiculo(venta.Vehiculo, "Disponible");

            _repo.GuardarLista(lista);
            return true;
        }

        public void RegistrarVenta(Venta venta)
        {
            _repo.Agregar(venta);
        }

        public void MarcarComoFacturada(int ventaId)
        {
            var lista = _repo.ObtenerTodos();
            var venta = lista.FirstOrDefault(v => v.ID == ventaId);

            if (venta != null)
            {
                venta.Estado = "Facturada";
                _repo.GuardarLista(lista);
            }
        }

        public void MarcarComoEntregada(int id)
        {
            var lista = _repo.ObtenerTodos();
            var venta = lista.FirstOrDefault(v => v.ID == id);
            if (venta != null)
            {
                venta.Estado = "Entregada";
                _repo.GuardarLista(lista);
            }
        }

        public List<Venta> ObtenerVentasSinComisionAsignada()
        {
            var comisiones = new XmlRepository<Comision>("comisiones.xml").ObtenerTodos();
            var idsConComision = comisiones.Select(c => c.Venta.ID).ToList();

            return _repo.ObtenerTodos()
                        .Where(v => v.Estado == "Entregada" && !idsConComision.Contains(v.ID))
                        .ToList();
        }


        public void FinalizarVenta(Venta venta)
        {
            var lista = _repo.ObtenerTodos();
            lista.Add(venta);
            _repo.GuardarLista(lista);
        }

        public List<Venta> ObtenerVentasFacturadas()
        {
            return _repo.ObtenerTodos()
                        .Where(v => v.Estado == "Facturada")
                        .ToList();
        }

        public List<Venta> ObtenerVentasPorFecha(DateTime desde, DateTime hasta)
        {
            return _repo.ObtenerTodos()
                        .Where(v => v.Fecha.Date >= desde.Date && v.Fecha.Date <= hasta.Date)
                        .ToList();
        }

        public List<Venta> ObtenerVentasAutorizadas()
        {
            return _repo.ObtenerTodos()
                        .Where(v => string.Equals(v.Estado, "Autorizada", StringComparison.OrdinalIgnoreCase))
                        .ToList();
        }

        public List<Venta> ObtenerVentasEntregadas()
        {
            return _repo.ObtenerTodos()
                        .Where(v => v.Estado == "Entregada")
                        .ToList();
        }


    }
}
