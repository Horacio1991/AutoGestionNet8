using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Vista.Modelos;

namespace AutoGestion.Vista
{
    public partial class RegistrarAsistencia : UserControl
    {
        private readonly TurnoBLL _turnoBLL = new();
        private List<Turno> _turnosCumplidos;

        public RegistrarAsistencia()
        {
            InitializeComponent();
            cmbEstado.Items.AddRange(new[] { "Asistió", "No asistió" });
            CargarTurnosCumplidos();
        }

        private void CargarTurnosCumplidos()
        {
            _turnosCumplidos = _turnoBLL.ObtenerTurnosCumplidos();

            var vista = _turnosCumplidos.Select(t => new TurnoVista
            {
                ID = t.ID,
                Cliente = $"{t.Cliente?.Nombre} {t.Cliente?.Apellido}",
                Vehiculo = $"{t.Vehiculo?.Marca} {t.Vehiculo?.Modelo} ({t.Vehiculo?.Dominio})",
                Fecha = t.Fecha.ToShortDateString(),
                Hora = t.Hora.ToString(@"hh\:mm"),
                Asistencia = t.Asistencia ?? "Pendiente"
            }).ToList();

            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = vista;
            dgvTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTurnos.ReadOnly = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un turno.");
                return;
            }

            if (cmbEstado.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el estado de asistencia.");
                return;
            }

            var seleccion = dgvTurnos.CurrentRow.DataBoundItem as TurnoVista;
            string observaciones = txtObservaciones.Text.Trim();

            _turnoBLL.RegistrarAsistencia(seleccion.ID, cmbEstado.SelectedItem.ToString(), observaciones);
            MessageBox.Show("Asistencia registrada correctamente.");

            cmbEstado.SelectedIndex = -1;
            txtObservaciones.Clear();
            CargarTurnosCumplidos();
        }
    }
}
