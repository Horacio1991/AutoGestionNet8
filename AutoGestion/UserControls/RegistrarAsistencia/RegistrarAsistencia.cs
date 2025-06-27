using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;

namespace AutoGestion.Vista
{
    public partial class RegistrarAsistencia : UserControl
    {
        private readonly RegistrarAsistenciaController _ctrl = new();
        private List<TurnoAsistenciaListDto> _turnos;

        public RegistrarAsistencia()
        {
            InitializeComponent();

            // Poblamos el combo de estados
            cmbEstado.Items.AddRange(new[] { "Asistió", "No asistió", "Pendiente" });

            // Cargamos el grid
            CargarTurnos();
        }

        private void CargarTurnos()
        {
            _turnos = _ctrl.ObtenerTurnosParaAsistencia();
            dgvTurnos.DataSource = null;
            dgvTurnos.DataSource = _turnos;
            dgvTurnos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTurnos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTurnos.ReadOnly = true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un turno.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cmbEstado.SelectedIndex < 0)
            {
                MessageBox.Show("Seleccione un estado de asistencia.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dtoGrid = dgvTurnos.CurrentRow.DataBoundItem as TurnoAsistenciaListDto;
            var input = new RegistrarAsistenciaInputDto
            {
                TurnoID = dtoGrid.ID,
                Estado = cmbEstado.SelectedItem.ToString(),
                Observaciones = txtObservaciones.Text.Trim()
            };

            try
            {
                _ctrl.RegistrarAsistencia(input);
                MessageBox.Show("Asistencia registrada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar y recargar
                cmbEstado.SelectedIndex = -1;
                txtObservaciones.Clear();
                CargarTurnos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
