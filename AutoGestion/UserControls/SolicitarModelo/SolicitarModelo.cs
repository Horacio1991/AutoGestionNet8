using AutoGestion.CTRL_Vista;
using AutoGestion.CTRL_Vista.Modelos;


namespace AutoGestion.Vista
{
    public partial class SolicitarModelo : UserControl
    {
        private readonly VehiculoController _controller = new();

        public SolicitarModelo()
        {
            InitializeComponent();
            CargarTodosLosVehiculos();
        }

        private void CargarTodosLosVehiculos()
        {
            // Traigo todos los vehiculos ya mapeados a DTO
            var lista = _controller.ObtenerDisponibles();
            dgvResultados.DataSource = lista;
            FormatearGrid();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string texto = txtModelo.Text.Trim();
            if (string.IsNullOrEmpty(texto))
            {
                MessageBox.Show("Por favor ingrese un modelo.");
                return;
            }

            // 1) Intento por modelo exacto
            var exactos = _controller.BuscarPorModelo(texto);
            if (exactos.Count > 0)
            {
                dgvResultados.DataSource = exactos;
            }
            else
            {
                // 2) Fallback: por misma marca
                var porMarca = _controller.BuscarPorMarca(texto);
                if (porMarca.Count > 0)
                {
                    MessageBox.Show("No se encontraron coincidencias exactas. Mostrando vehículos de la misma marca.");
                    dgvResultados.DataSource = porMarca;
                }
                else
                {
                    MessageBox.Show("No se encontraron vehículos disponibles.");
                    dgvResultados.DataSource = new List<VehiculoDto>();
                }
            }

            FormatearGrid();
        }

        private void btnMostrarTodos_Click(object sender, EventArgs e)
        {
            txtModelo.Clear();
            CargarTodosLosVehiculos();
        }

        private void FormatearGrid()
        {
            dgvResultados.AutoGenerateColumns = true;  // sólo las propiedades del DTO
            dgvResultados.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvResultados.ReadOnly = true;
            dgvResultados.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

    }
}
