using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sideloader
{
    using System.Diagnostics;
    using System.IO;
    using Controls;
    using Microsoft.Win32;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string BUNDLETOOL_PATH = "BundleTool/bundletool-all.jar";

        private Process exportProcess;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void aabFilepathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void chooseAabButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Android App Bundle (.aab)|*.aab";
            bool? selectionResult = openFileDialog.ShowDialog();

            if (selectionResult == true)
            {
                aabFilepathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void exportApksButton_Click(object sender, RoutedEventArgs e)
        {
            ExportApks();
        }

        private void installButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {

        }

        // private ProcessingAnimation processingAnimation;

        private void ExportApks()
        {
            if (exportProcess != null)
            {
                throw new Exception("An export process is already running");
            }
            
            string aabPath = aabFilepathTextBox.Text;
            if (!File.Exists(aabPath))
            {
                // TODO : Show error dialog
            }
            
            object outputPath = Path.ChangeExtension(aabPath, ".apks");
            
            string arguments = $"-jar {BUNDLETOOL_PATH} build-apks --bundle=\"{aabPath}\" --output=\"{outputPath}\"";
            
            using (exportProcess = new Process())
            {
                exportProcess.StartInfo.FileName = @"java";
                exportProcess.StartInfo.Arguments = arguments;
                exportProcess.StartInfo.UseShellExecute = false;
                exportProcess.StartInfo.CreateNoWindow = false;
                exportProcess.StartInfo.RedirectStandardOutput = true;
            
                exportProcess.EnableRaisingEvents = true;
                exportProcess.Exited += new EventHandler(OnExportProcessExited);
            
                ShowProcessingAnimation();

                try
                {
                    exportProcess.Start();
                }
                catch (Exception e)
                {
                    HideProcessingAnimation();
                    Console.WriteLine(e);
                }
            }
        }

        private void OnExportProcessExited(object? sender, EventArgs e)
        {                    
            HideProcessingAnimation();

            string result = exportProcess.StandardOutput.ReadToEnd();
        }

        private void ShowProcessingAnimation()
        {
            processingAnimation.Visibility = Visibility.Visible;
        }

        private void HideProcessingAnimation()
        {
            processingAnimation.Visibility = Visibility.Hidden;
        }
    }
}