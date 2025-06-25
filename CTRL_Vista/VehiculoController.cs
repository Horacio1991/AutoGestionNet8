using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;

namespace AutoGestion.CTRL_Vista
{
    public class VehiculoController
    {
        private readonly VehiculoBLL _bll = new();

        public List<VehiculoDto> ObtenerTodos()
            => _bll.ObtenerTodos()
                   .Select(VehiculoDto.FromEntity)
                   .ToList();

        public List<VehiculoDto> ObtenerDisponibles()
            => _bll.ObtenerDisponibles()
                   .Select(VehiculoDto.FromEntity)
                   .ToList();

        public List<VehiculoDto> BuscarPorModelo(string modelo)
            => _bll.BuscarVehiculosPorModelo(modelo)
                   .Select(VehiculoDto.FromEntity)
                   .ToList();

        public List<VehiculoDto> BuscarPorMarca(string marca)
            => _bll.ObtenerTodos()
                   .Where(v => v.Marca.Equals(marca, StringComparison.OrdinalIgnoreCase))
                   .Select(VehiculoDto.FromEntity)
                   .ToList();
    }
}
