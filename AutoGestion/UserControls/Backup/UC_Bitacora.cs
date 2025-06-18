using AutoGestion.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vista.UserControls.Backup
{
    public partial class UC_Bitacora : UserControl
    {
        public UC_Bitacora()
        {
            InitializeComponent();
            rbTodos.Checked = true;
            CargarBitacora();
        }

        private void CargarBitacora()
        {
            var lista = BitacoraService.ObtenerTodo();

            if (rbSoloBackups.Checked)
                lista = lista.Where(b => b.Detalle.ToLower() == "backup").ToList();
            else if (rbSoloRestores.Checked)
                lista = lista.Where(b => b.Detalle.ToLower() == "restore").ToList();

            dgvBitacora.DataSource = lista.Select(b => new
            {
                Fecha = b.FechaRegistro.ToString("g"),
                Detalle = b.Detalle,
                ID_Usuario = b.UsuarioID,
                Usuario = b.UsuarioNombre
            }).ToList();

            dgvBitacora.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBitacora.ReadOnly = true;
            dgvBitacora.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void rbTodos_CheckedChanged_1(object sender, EventArgs e) => CargarBitacora();
        private void rbSoloBackups_CheckedChanged_1(object sender, EventArgs e) => CargarBitacora();
        private void rbSoloRestores_CheckedChanged_1(object sender, EventArgs e) => CargarBitacora();
        private void btnRecargarBitacora_Click(object sender, EventArgs e)=> CargarBitacora();

     
    }
}
