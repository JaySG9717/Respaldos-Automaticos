ESTE ES EL CODIGO BASE, NO PRESENTA NINGUN ERROR PERO FALTA AGREGARLE FEATURES COMO LA BARRA DE PROGRESO PARA MONITOREAR EL PROGRESO DEL RESPALDO:

using System;
using System.IO;
using System.Windows.Forms;

namespace BackupApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            // Rutas de las carpetas locales predeterminadas
            string sourceDesktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            string sourceDocuments = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            string sourcePictures = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            string sourceDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string destination = lblDestDownloads.Text.Replace("Descargas/Unidad de Red", "");

            if (string.IsNullOrEmpty(destination) || destination == "Selecciona la ruta de destino")
            {
                MessageBox.Show("Por favor, selecciona una ruta de destino válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Copiar el contenido de las carpetas
                CopyDirectoryContent(sourceDesktop, Path.Combine(destination, "Escritorio"));
                CopyDirectoryContent(sourceDocuments, Path.Combine(destination, "Documentos"));
                CopyDirectoryContent(sourcePictures, Path.Combine(destination, "Imagenes"));
                CopyDirectoryContent(sourceDownloads, Path.Combine(destination, "Descargas"));

                MessageBox.Show("Archivos copiados exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CopyDirectoryContent(string sourceDir, string destinationDir)
        {
            // Crear el directorio de destino si no existe
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Copiar los archivos del directorio
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            // Copiar los subdirectorios de forma recursiva
            foreach (var subdir in Directory.GetDirectories(sourceDir))
            {
                var destSubdir = Path.Combine(destinationDir, Path.GetFileName(subdir));
                CopyDirectoryContent(subdir, destSubdir);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Selecciona la carpeta de destino";
                folderBrowserDialog.ShowNewFolderButton = true;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    lblDestDownloads.Text = selectedPath + "\\Descargas";
                    lblDestDocuments.Text = selectedPath + "\\Documentos";
                    lblDestPictures.Text = selectedPath + "\\Imagenes";
                    lblDestDesktop.Text = selectedPath + "\\Escritorio";
                }
            }
        }
    }
}
