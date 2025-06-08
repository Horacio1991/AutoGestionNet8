using AutoGestion.Entidades;
using AutoGestion.BLL;
using AutoGestion.Servicios;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.Vista
{
    public partial class RealizarPago : UserControl
    {
        private readonly ClienteBLL _clienteBLL = new();
        private readonly VehiculoBLL _vehiculoBLL = new();
        private readonly VentaBLL _ventaBLL = new();
        private readonly PagoBLL _pagoBLL = new();

        private Cliente clienteSeleccionado = null;

        Vendedor vendedor = new Vendedor
        {
            ID = Sesion.UsuarioActual.ID,
            Nombre = Sesion.UsuarioActual.Nombre
        };

        public RealizarPago()
        {
            InitializeComponent();
            CargarVehiculosDisponibles();
            CargarTiposDePago();
        }

        private void CargarVehiculosDisponibles()
        {
            dgvVehiculos.DataSource = null;
            dgvVehiculos.DataSource = _vehiculoBLL.ObtenerDisponibles();
        }

        private void CargarTiposDePago()
        {
            cmbTipoPago.Items.Clear();
            cmbTipoPago.Items.Add("Efectivo");
            cmbTipoPago.Items.Add("Transferencia");
            cmbTipoPago.Items.Add("Tarjeta de Crédito");
            cmbTipoPago.Items.Add("Financiación");
            cmbTipoPago.SelectedIndex = 0;
        }

        private void btnBuscarCliente_Click_1(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();

            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI.");
                return;
            }

            clienteSeleccionado = _clienteBLL.BuscarClientePorDNI(dni);

            if (clienteSeleccionado == null)
            {
                MessageBox.Show("Cliente no encontrado.");
                return;
            }

            txtNombre.Text = clienteSeleccionado.Nombre;
            txtApellido.Text = clienteSeleccionado.Apellido;
            txtContacto.Text = clienteSeleccionado.Contacto;
        }

        private void btnRegistrarPago_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (clienteSeleccionado == null)
                {
                    MessageBox.Show("Debe buscar y seleccionar un cliente.");
                    return;
                }

                if (dgvVehiculos.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Seleccione un vehículo.");
                    return;
                }

                Vehiculo vehiculo = dgvVehiculos.SelectedRows[0].DataBoundItem as Vehiculo;

                string tipoPago = cmbTipoPago.SelectedItem.ToString();
                if (!decimal.TryParse(txtMonto.Text.Trim(), out decimal monto))
                {
                    MessageBox.Show("Monto inválido.");
                    return;
                }

                int.TryParse(txtCuotas.Text.Trim(), out int cuotas);
                string otros = txtOtrosDatos.Text.Trim();

                Pago pago = new Pago
                {
                    ID = GeneradorID.ObtenerID<Pago>(),
                    TipoPago = tipoPago,
                    Monto = monto,
                    Cuotas = cuotas,
                    Detalles = otros
                };

                // Guardar el pago antes de usarlo
                _pagoBLL.RegistrarPago(pago);

                Venta venta = new Venta
                {
                    ID = GeneradorID.ObtenerID<Venta>(),
                    Cliente = clienteSeleccionado,
                    Vehiculo = vehiculo,
                    Pago = pago,
                    Fecha = DateTime.Now,
                    Estado = "Pendiente",
                    Vendedor = vendedor
                };

                _ventaBLL.FinalizarVenta(venta);
                _vehiculoBLL.ActualizarEstadoVehiculo(vehiculo, "En Proceso");
                MessageBox.Show("Pago registrado y venta pendiente creada.");

                // Limpiar todo
                txtDni.Clear();
                txtNombre.Clear();
                txtApellido.Clear();
                txtContacto.Clear();
                txtMonto.Clear();
                txtCuotas.Clear();
                txtOtrosDatos.Clear();
                clienteSeleccionado = null;
                CargarVehiculosDisponibles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar el pago: " + ex.Message);
            }
        }

    }
}
