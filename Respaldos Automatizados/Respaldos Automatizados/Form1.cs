namespace Respaldos_Automatizados
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btn_seleccionar_unidad_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Selecciona la carpeta de destino";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.NetworkShortcuts; // Cambia al root de NetworkShortcuts

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    lbl_descargas_unidadred.Text = selectedPath + "\\Descargas";
                    lbl_documentos_unidadred.Text = selectedPath + "\\Documentos";
                    lbl_imagenes_unidadred.Text = selectedPath + "\\Imagenes";
                    lbl_escritorio_unidadred.Text = selectedPath + "\\Escritorio";

                    UpdateLocalPaths(selectedPath);
                }
            }
        }
        private void UpdateLocalPaths(string selectedPath)
        {
            // Rutas de las carpetas locales predeterminadas
            lbl_descargas_mipc.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            lbl_documentos_mipc.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            lbl_imagenes_mipc.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            lbl_escritorio_mipc.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        private int CountFilesToCopy(string sourceDir)
        {
            int fileCount = 0;

            // Contar archivos en el directorio actual
            fileCount += Directory.GetFiles(sourceDir).Length;

            // Contar archivos en subdirectorios de forma recursiva
            foreach (string subdir in Directory.GetDirectories(sourceDir))
            {
                fileCount += CountFilesToCopy(subdir);
            }

            return fileCount;
        }

        private void btn_respaldar_Click(object sender, EventArgs e)
        {
            // Rutas de las carpetas locales predeterminadas
            string sourceDesktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            string sourceDocuments = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            string sourcePictures = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            string sourceDownloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string destination = lbl_descargas_unidadred.Text.Replace("Descargas/Unidad de Red", "");

            if (string.IsNullOrEmpty(destination) || destination == "Selecciona la ruta de destino")
            {
                MessageBox.Show("Por favor, selecciona una ruta de destino válida.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // Configurar ProgressBar
                progressBar1.Value = 0;
                int totalFiles = CountFilesToCopy(sourceDesktop) + CountFilesToCopyRecursive(sourceDocuments) + CountFilesToCopy(sourcePictures) + CountFilesToCopy(sourceDownloads);

                int currentFile = 0;

                // Copiar el contenido de las carpetas
                CopyDirectoryContent(sourceDesktop, Path.Combine(destination, "Escritorio"), ref currentFile, totalFiles);
                CopyDirectoryContent(sourceDocuments, Path.Combine(destination, "Documentos"), ref currentFile, totalFiles);
                CopyDirectoryContent(sourcePictures, Path.Combine(destination, "Imagenes"), ref currentFile, totalFiles);
                CopyDirectoryContent(sourceDownloads, Path.Combine(destination, "Descargas"), ref currentFile, totalFiles);

                MessageBox.Show("Archivos copiados exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Error de acceso no autorizado al copiar archivos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Aquí podrías manejar la excepción de acceso denegado de manera específica o simplemente continuar
                // con el flujo del programa, dependiendo de tus requisitos.
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CopyDirectoryContent(string sourceDir, string destinationDir, ref int currentFile, int totalFiles)
        {
            try
            {
                // Verificar si el directorio de origen existe
                if (!Directory.Exists(sourceDir))
                {
                    MessageBox.Show($"El directorio de origen '{sourceDir}' no existe o no se puede acceder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verificar si el directorio de destino existe antes de intentar crearlo
                if (!Directory.Exists(destinationDir))
                {
                    MessageBox.Show($"El directorio de destino '{destinationDir}' no existe o no se puede acceder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Copiar los archivos del directorio
                foreach (var file in Directory.GetFiles(sourceDir))
                {
                    var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                    File.Copy(file, destFile, true);

                    // Actualizar barra de progreso
                    currentFile++;
                    int progress = (int)((double)currentFile / totalFiles * 100);
                    progressBar1.Value = progress;
                }

                // Copiar los subdirectorios de forma recursiva
                foreach (var subdir in Directory.GetDirectories(sourceDir))
                {
                    var destSubdir = Path.Combine(destinationDir, Path.GetFileName(subdir));
                    CopyDirectoryContent(subdir, destSubdir, ref currentFile, totalFiles);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // Capturar y manejar excepciones específicas de acceso denegado aquí si es necesario
                MessageBox.Show($"Error de acceso no autorizado al copiar archivos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al copiar archivos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int CountFilesToCopyRecursive(string sourceDir)
        {
            int fileCount = 0;

            // Contar archivos en el directorio actual
            fileCount += Directory.GetFiles(sourceDir).Length;

            // Contar archivos en subdirectorios de forma recursiva
            foreach (string subdir in Directory.GetDirectories(sourceDir))
            {
                fileCount += CountFilesToCopyRecursive(subdir);
            }

            return fileCount;
        }
    }
}