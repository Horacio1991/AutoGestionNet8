using AutoGestion.BLL;


namespace AutoGestion.Vista
{
    public partial class RegistrarTurno : UserControl
    {
        private readonly ClienteBLL _clienteBLL = new();
        private readonly VehiculoBLL _vehiculoBLL = new();
        private readonly TurnoBLL _turnoBLL = new();

        public RegistrarTurno()
        {
            InitializeComponent();

            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpHora.Format = DateTimePickerFormat.Time;
            dtpHora.ShowUpDown = true; // Para que no tenga selector de calendario
            dtpHora.Value = DateTime.Today; //para la hora en 00:00:00 de hoy
        }

        private void RegistrarTurno_Load_1(object sender, EventArgs e)
        {
            dgvVehiculos.DataSource = null;
            dgvVehiculos.DataSource = _vehiculoBLL.ObtenerDisponibles()
                .Select(v => new
                {
                    Modelo = $"{v.Marca} {v.Modelo}",
                    Dominio = v.Dominio
                }).ToList();

            dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVehiculos.ReadOnly = true;
            dgvVehiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void dgvVehiculos_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtDominio.Text = dgvVehiculos.Rows[e.RowIndex].Cells["Dominio"].Value.ToString();
            }
        }

        private void dtpHora_ValueChanged_1(object sender, EventArgs e)
        {
            DateTime hora = dtpHora.Value;
            int minutos = hora.Minute >= 30 ? 30 : 0;

            dtpHora.Value = new DateTime(hora.Year, hora.Month, hora.Day, hora.Hour, minutos, 0);
        }



        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            string dni = txtDniCliente.Text.Trim();
            string dominio = txtDominio.Text.Trim();

            if (string.IsNullOrEmpty(dni) || string.IsNullOrEmpty(dominio))
            {
                MessageBox.Show("Debe completar DNI y Dominio.");
                return;
            }

            var cliente = _clienteBLL.BuscarClientePorDNI(dni);
            if (cliente == null)
            {
                MessageBox.Show("Cliente no encontrado.");
                return;
            }

            var vehiculo = _vehiculoBLL.BuscarVehiculoPorDominio(dominio);
            if (vehiculo == null || vehiculo.Estado != "Disponible")
            {
                MessageBox.Show("Vehículo no disponible.");
                return;
            }

            DateTime fecha = dtpFecha.Value.Date;
            TimeSpan hora = dtpHora.Value.TimeOfDay;

            if (!_turnoBLL.EstaDisponible(fecha, hora, vehiculo))
            {
                MessageBox.Show("Ya existe un turno para ese día y hora con ese vehículo.");
                return;
            }

            _turnoBLL.RegistrarTurno(cliente, vehiculo, fecha, hora);
            MessageBox.Show("Turno registrado con éxito.");

            txtDniCliente.Clear();
            txtDominio.Clear();
        }

        
    }
}
