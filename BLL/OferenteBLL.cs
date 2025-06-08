using AutoGestion.Entidades;
using AutoGestion.DAO.Repositorios;
using System.Collections.Generic;
using System.Linq;

namespace AutoGestion.BLL
{
    public class OferenteBLL
    {
        private readonly XmlRepository<Oferente> _repo = new("oferentes.xml");

        public Oferente BuscarPorDni(string dni)
        {
            return _repo.ObtenerTodos().FirstOrDefault(o => o.Dni == dni);
        }

        public void GuardarOferente(Oferente oferente)
        {
            oferente.ID = ObtenerNuevoID();
            _repo.Agregar(oferente);
        }

        private int ObtenerNuevoID()
        {
            var lista = _repo.ObtenerTodos();
            return lista.Any() ? lista.Max(o => o.ID) + 1 : 1;
        }
    }
}
