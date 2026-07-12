using Blue_Sky.Classes;
using Blue_Sky.Readers;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Blue_Sky.Tabs
{
    /// <summary>
    /// Interaction logic for TabLogfileData.xaml
    /// </summary>
    public partial class TabLogfileData : Tab
    {
        public TabLogfileData()
        {
            InitializeComponent();
        }

        public async void btnLogFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pickLogFile = new OpenFileDialog
            {
                Filter = "OMSI Log File|*.txt|All files|*.*",
                InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\OMSI 2"
            };

            if (pickLogFile.ShowDialog() == true)
            {
                tabLogFile_Clear();
                txtLogFileDir.Text = pickLogFile.FileName;

                // Show map load screen
                MapLoadScreen m = new()
                {
                    Topmost = true
                };
                m.Show();
                this.mainWindow.BlueSkyWindow.IsEnabled = false;

                Task task = Task.Factory.StartNew(async () =>
                {
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading logfile.txt..."));

                    Logfile file = new();

                    await LogReader.ReadLogfileAsync(pickLogFile.FileName, file);

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Collecting Data...";

                        tabLogFileWarn.Header = "Warnings (" + file.warn.Count + ")";
                        tabLogFileError.Header = "Errors (" + file.error.Count + ")";
                        txtLogFileInfo.Text = String.Join("\r\n", file.info);
                        txtLogFileWarn.Text = String.Join("\r\n", file.warn);
                        txtLogFileError.Text = String.Join("\r\n", file.error);

                        // Re-enable main window and close loading screen
                        this.mainWindow.BlueSkyWindow.IsEnabled = true;
                        m.Close();
                    }));
                });
            }
        }


        public void tabLogFile_Clear()
        {
            txtLogFileDir.Clear();
            tabLogFileWarn.Header = "Warnings";
            tabLogFileError.Header = "Errors";
            txtLogFileInfo.Clear();
            txtLogFileWarn.Clear();
            txtLogFileError.Clear();
        }
    }
}
