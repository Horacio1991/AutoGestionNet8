using AutoGestion.CTRL_Vista;
using AutoGestion.DTOs;


namespace AutoGestion.Vista
{
    public partial class RegistrarTurno : UserControl
    {
        private readonly RegistrarTurnoController _ctrl = new();
        private List<VehiculoTurnoDto> _vehiculos;

        public RegistrarTurno()
        {
            InitializeComponent();
            Load += RegistrarTurno_Load_1;

            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpHora.Format = DateTimePickerFormat.Time;
            dtpHora.ShowUpDown = true; // Para que no tenga selector de calendario
            dtpHora.Value = DateTime.Today; //para la hora en 00:00:00 de hoy
        }

        private void RegistrarTurno_Load_1(object sender, EventArgs e)
        {
            CargarVehiculos();
        }

        private void CargarVehiculos()
        {
            _vehiculos = _ctrl.ObtenerVehiculosParaTurno();
            dgvVehiculos.DataSource = null;
            dgvVehiculos.DataSource = _vehiculos
                .Select(v => new
                {
                    v.ID,
                    v.Dominio,
                    v.Marca,
                    v.Modelo
                })
                .ToList();
            dgvVehiculos.ClearSelection();
        }

        private void dgvVehiculos_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvVehiculos.Rows[e.RowIndex];
            txtDominio.Text = row.Cells["Dominio"].Value.ToString();
        }

        private void dtpHora_ValueChanged_1(object sender, EventArgs e)
        {
            DateTime hora = dtpHora.Value;
            int minutos = hora.Minute >= 30 ? 30 : 0;

            dtpHora.Value = new DateTime(hora.Year, hora.Month, hora.Day, hora.Hour, minutos, 0);
        }



        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            var dni = txtDniCliente.Text.Trim();
            var dominio = txtDominio.Text.Trim();
            var fecha = dtpFecha.Value.Date;
            var hora = dtpHora.Value.TimeOfDay;

            if (string.IsNullOrEmpty(dni) || string.IsNullOrEmpty(dominio))
            {
                MessageBox.Show("Ingrese DNI de cliente y dominio de vehículo.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new TurnoInputDto
            {
                DniCliente = dni,
                DominioVehiculo = dominio,
                Fecha = fecha,
                Hora = hora
            };

            try
            {
                _ctrl.RegistrarTurno(dto);
                MessageBox.Show("Turno registrado correctamente.",
                                "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtDniCliente.Clear();
                txtDominio.Clear();
                CargarVehiculos();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo registrar el turno: {ex.Message}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
