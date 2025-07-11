using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.CTRL_Vista
{
    public class VehiculoController
    {
        private readonly VehiculoBLL _bll = new();

        // Retorna todos los vehículos en stock.
        public List<VehiculoDto> ObtenerTodos()
        {
            try
            {
                return _bll.ObtenerTodos()
                           .Select(VehiculoDto.FromEntity)
                           .Where(dto => dto != null)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener todos los vehículos: {ex.Message}", ex);
            }
        }

        // Retorna solo los vehículos con estado "Disponible".
        public List<VehiculoDto> ObtenerDisponibles()
        {
            try
            {
                return _bll.ObtenerDisponibles()
                           .Select(VehiculoDto.FromEntity)
                           .Where(dto => dto != null)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener vehículos disponibles: {ex.Message}", ex);
            }
        }

        // Busca vehículos cuyo modelo coincida exactament
        public List<VehiculoDto> BuscarPorModelo(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new ArgumentException("Modelo requerido.", nameof(modelo));

            try
            {
                return _bll.BuscarVehiculosPorModelo(modelo)
                           .Select(VehiculoDto.FromEntity)
                           .Where(dto => dto != null)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar por modelo: {ex.Message}", ex);
            }
        }

        public List<VehiculoDto> BuscarPorMarca(string marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
                throw new ArgumentException("Marca requerida.", nameof(marca));

            try
            {
                return _bll.ObtenerTodos()
                           .Where(v => v.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase))
                           .Select(VehiculoDto.FromEntity)
                           .Where(dto => dto != null)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al buscar por marca: {ex.Message}", ex);
            }
        }
    }
}
