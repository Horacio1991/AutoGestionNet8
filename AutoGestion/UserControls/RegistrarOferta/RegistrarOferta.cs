using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;
using AutoGestion.DTOs;


namespace AutoGestion.Vista
{
    public partial class RegistrarOferta : UserControl
    {
        private readonly OfertaController _ctrl = new();

        public RegistrarOferta()
        {
            InitializeComponent();
        }

        private void btnBuscarOferente_Click_1(object sender, EventArgs e)
        {
            string dni = txtDni.Text.Trim();
            if (string.IsNullOrEmpty(dni))
            {
                MessageBox.Show("Ingrese un DNI válido.");
                return;
            }

            var dto = _ctrl.BuscarOferente(dni);
            if (dto != null)
            {
                txtNombre.Text = dto.Nombre;
                txtApellido.Text = dto.Apellido;
                txtContacto.Text = dto.Contacto;
                MessageBox.Show("Oferente encontrado.");
            }
            else
            {
                MessageBox.Show("Oferente no encontrado. Complete los datos para registrarlo.");
                txtNombre.Clear();
                txtApellido.Clear();
                txtContacto.Clear();
            }
        }



        private void btnGuardarOferta_Click(object sender, EventArgs e)
        {
            // Validaciones...
            if (string.IsNullOrWhiteSpace(txtDni.Text) ||
                string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtApellido.Text) ||
                string.IsNullOrWhiteSpace(txtContacto.Text) ||
                string.IsNullOrWhiteSpace(txtMarca.Text) ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtColor.Text) ||
                string.IsNullOrWhiteSpace(txtDominio.Text))
            {
                MessageBox.Show("Complete todos los campos.", "Atención",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new OfertaInputDto
            {
                Oferente = new OferenteDto
                {
                    Dni = txtDni.Text.Trim(),
                    Nombre = txtNombre.Text.Trim(),
                    Apellido = txtApellido.Text.Trim(),
                    Contacto = txtContacto.Text.Trim()
                },
                Vehiculo = new VehiculoDto
                {
                    Marca = txtMarca.Text.Trim(),
                    Modelo = txtModelo.Text.Trim(),
                    Año = (int)numAnio.Value,
                    Color = txtColor.Text.Trim(),
                    Dominio = txtDominio.Text.Trim(),
                    Km = (int)numKm.Value
                },
                FechaInspeccion = dtpFechaInspeccion.Value
            };

            bool ok = _ctrl.RegistrarOferta(dto);
            MessageBox.Show(ok
                ? "Oferta y vehículo guardados correctamente."
                : "Error al registrar la oferta.",
                "Resultado", MessageBoxButtons.OK,
                ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);

            // Limpio
            txtDni.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            txtContacto.Clear();
            txtMarca.Clear();
            txtModelo.Clear();
            txtColor.Clear();
            txtDominio.Clear();
            numAnio.Value = numAnio.Minimum;
            numKm.Value = 0;
        }

    }
}
