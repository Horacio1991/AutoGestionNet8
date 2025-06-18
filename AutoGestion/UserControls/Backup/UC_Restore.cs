using AutoGestion.BLL;
using AutoGestion.Servicios;
using BLL;
using Entidades;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Vista.UserControls.Backup
{
    public partial class UC_Restore : UserControl
    {
        private readonly string rutaBackups = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backups");
        private readonly string rutaDatos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DatosXML");
        private readonly BitacoraBLL _bitacoraBLL = new();

        public UC_Restore()
        {
            InitializeComponent();
            CargarBackups();
        }

        private void CargarBackups()
        {
            if (!Directory.Exists(rutaBackups))
                Directory.CreateDirectory(rutaBackups);

            var carpetas = Directory.GetDirectories(rutaBackups)
                                    .Select(f => new DirectoryInfo(f).Name)
                                    .OrderByDescending(f => f)
                                    .ToArray();

            lstBackups.Items.Clear();
            lstBackups.Items.AddRange(carpetas);
        }

        private void btnRestaurarSeleccionado_Click(object sender, EventArgs e)
        {
            if (lstBackups.SelectedItem == null)
            {
                MessageBox.Show("Seleccioná un backup para restaurar.");
                return;
            }

            string backupSeleccionado = lstBackups.SelectedItem.ToString();
            string rutaBackupSeleccionado = Path.Combine(rutaBackups, backupSeleccionado);

            try
            {
                var archivosBackup = Directory.GetFiles(rutaBackupSeleccionado);

                foreach (var archivo in archivosBackup)
                {
                    string nombreArchivo = Path.GetFileName(archivo);

                    if (nombreArchivo.Equals("bitacora.xml", StringComparison.OrdinalIgnoreCase))
                        continue; // Nunca restaurar la bitácora

                    string destino = Path.Combine(rutaDatos, nombreArchivo);
                    File.Copy(archivo, destino, overwrite: true);
                }

                // Registrar Restore en Bitácora
                var usuario = Sesion.UsuarioActual;
                _bitacoraBLL.Registrar("restore", usuario?.ID ?? 0, usuario?.Nombre ?? "Desconocido");

                MessageBox.Show("Restore realizado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al restaurar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     
    }
}
