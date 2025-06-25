using AutoGestion.BLL;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.Entidades;
using AutoGestion.Servicios;
using AutoGestion.Servicios.Utilidades;


namespace AutoGestion.Vista
{
    public partial class RealizarPago : UserControl
    {
        private readonly ClienteBLL _clienteBLL = new();
        private readonly VehiculoBLL _vehiculoBLL = new();
        private readonly PagoBLL _pagoBLL = new();
        private readonly VentaBLL _ventaBLL = new();

        private ClienteDto _clienteSeleccionado;
        private VehiculoDto _vehiculoSeleccionado;

        public RealizarPago()
        {
            InitializeComponent();
            CargarVehiculosDisponibles();
            CargarTiposDePago();
        }

        private void CargarVehiculosDisponibles()
        {
            // Transformamos cada entidad Vehículo en su DTO
            var dtos = _vehiculoBLL.ObtenerDisponibles()
                                   .Select(VehiculoDto.FromEntity)
                                   .ToList();

            dgvVehiculos.DataSource = dtos;
            dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVehiculos.ReadOnly = true;
            dgvVehiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void CargarTiposDePago()
        {
            cmbTipoPago.Items.Clear();
            cmbTipoPago.Items.AddRange(new object[]
            {
                "Efectivo",
                "Transferencia",
                "Tarjeta de Crédito",
                "Financiación"
            });
            cmbTipoPago.SelectedIndex = 0;
        }

        private void btnBuscarCliente_Click_1(object sender, EventArgs e)
        {
            var dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI.", "Validación",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Aquí podrías crear un ClienteController si quisieras aislar la BLL aún más,
            // pero por ahora usamos directamente la BLL:
            var entidad = _clienteBLL.BuscarClientePorDNI(dni);
            if (entidad == null)
            {
                MessageBox.Show("Cliente no encontrado.", "Información",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Mapeamos a DTO para la capa de Vista
            _clienteSeleccionado = new ClienteDto
            {
                Dni = entidad.Dni,
                Nombre = entidad.Nombre,
                Apellido = entidad.Apellido,
                Contacto = entidad.Contacto
            };

            txtNombre.Text = _clienteSeleccionado.Nombre;
            txtApellido.Text = _clienteSeleccionado.Apellido;
            txtContacto.Text = _clienteSeleccionado.Contacto;
        }

        private void btnRegistrarPago_Click_1(object sender, EventArgs e)
        {
            // Validaciones
            if (_clienteSeleccionado == null)
            {
                MessageBox.Show("Debe buscar y seleccionar un cliente.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_vehiculoSeleccionado == null)
            {
                MessageBox.Show("Seleccione un vehículo.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!decimal.TryParse(txtMonto.Text.Trim(), out decimal monto))
            {
                MessageBox.Show("Monto inválido.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int.TryParse(txtCuotas.Text.Trim(), out int cuotas);

            // 1) Crear y guardar Pago
            var pago = new Pago
            {
                ID = GeneradorID.ObtenerID<Pago>(),
                TipoPago = cmbTipoPago.SelectedItem.ToString(),
                Monto = monto,
                Cuotas = cuotas,
                Detalles = txtOtrosDatos.Text.Trim()
            };
            _pagoBLL.RegistrarPago(pago);

            // 2) Recuperar la entidad completa del vehículo
            var vehiculoEntidad = _vehiculoBLL.BuscarVehiculoPorDominio(_vehiculoSeleccionado.Dominio);

            // 3) Crear y guardar Venta
            var venta = new Venta
            {
                ID = GeneradorID.ObtenerID<Venta>(),
                Cliente = new Cliente
                {
                    Dni = _clienteSeleccionado.Dni,
                    Nombre = _clienteSeleccionado.Nombre,
                    Apellido = _clienteSeleccionado.Apellido,
                    Contacto = _clienteSeleccionado.Contacto
                },
                Vehiculo = vehiculoEntidad,
                Pago = pago,
                Fecha = DateTime.Now,
                Estado = "Pendiente",
                Vendedor = new Vendedor
                {
                    ID = Sesion.UsuarioActual.ID,
                    Nombre = Sesion.UsuarioActual.Nombre
                }
            };
            _ventaBLL.FinalizarVenta(venta);

            // 4) Reservar vehículo (cambia su estado para que no aparezca en disponibles)
            _vehiculoBLL.ActualizarEstadoVehiculo(vehiculoEntidad, "En Proceso");

            MessageBox.Show("✅ Pago registrado y venta pendiente creada.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 5) Limpiar formulario y recargar grid
            LimpiarFormulario();
            CargarVehiculosDisponibles();
        }

        private void LimpiarFormulario()
        {
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtContacto.Clear();
            txtMonto.Clear();
            txtCuotas.Clear();
            txtOtrosDatos.Clear();

            _clienteSeleccionado = null;
            _vehiculoSeleccionado = null;
        }

        private void dgvVehiculos_SelectionChanged(object sender, EventArgs e)
        {
            // Capturamos el DTO seleccionado
            _vehiculoSeleccionado = dgvVehiculos.CurrentRow?.DataBoundItem as VehiculoDto;
        }
    }
}
