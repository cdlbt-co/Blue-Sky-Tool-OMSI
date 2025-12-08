using Blue_Sky.Classes;
using Blue_Sky.Readers;
using Blue_Sky.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Blue_Sky
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Map map;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogFile_Click(object sender, RoutedEventArgs e)
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
                BlueSkyWindow.IsEnabled = false;

                Task task = Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading logfile.txt..."));

                    string[] logfile = File.ReadAllLines(pickLogFile.FileName);

                    HashSet<string> readInfo = new(StringComparer.OrdinalIgnoreCase);
                    HashSet<string> readWarn = new(StringComparer.OrdinalIgnoreCase);
                    HashSet<string> readError = new(StringComparer.OrdinalIgnoreCase);
                    List<string> info = [];
                    List<string> warn = [];
                    List<string> error = [];

                    foreach (string line in logfile)
                    {
                        if (line.Contains(" -  -   "))
                        {
                            // Remove the time prefix of log file entries
                            string newLine = line.Split([" -  -   "], StringSplitOptions.None)[1].TrimStart();

                            // Check for repeating entries, only add non duplicates to list and list box
                            if (newLine.StartsWith("Information:") && readInfo.Add(newLine[13..]))
                            {
                                info.Add(newLine[13..]);
                            }
                            if (newLine.StartsWith("Warning:") && readWarn.Add(newLine[15..]))
                            {
                                warn.Add(newLine[15..]);
                            }
                            if (newLine.StartsWith("Error:") && readError.Add(newLine[17..]))
                            {
                                error.Add(newLine[17..]);
                            }
                        }
                    }

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Collecting Data...";

                        tabLogFileWarn.Header = "Warnings (" + warn.Count + ")";
                        tabLogFileError.Header = "Errors (" + error.Count + ")";
                        txtLogFileInfo.Text = String.Join("\r\n", info);
                        txtLogFileWarn.Text = String.Join("\r\n", warn);
                        txtLogFileError.Text = String.Join("\r\n", error);

                        // Re-enable main window and close loading screen
                        BlueSkyWindow.IsEnabled = true;
                        m.Close();
                    }));
                });
            }
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pickMapFile = new OpenFileDialog();
            pickMapFile.Filter = "OMSI Map File|global.cfg|All files (*.*)|*.*";
            pickMapFile.InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\OMSI 2\maps";
            pickMapFile.ShowHiddenItems = true;

            if (pickMapFile.ShowDialog() == true)
            {
                // Clear everything
                tabMap_Clear();

                txtMapDir.Text = pickMapFile.FileName;

                // Show map load screen
                MapLoadScreen m = new() { Topmost = true };
                m.Show();
                BlueSkyWindow.IsEnabled = false;

                Task task = Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading global.cfg..."));

                    // Read map info
                    map = new(pickMapFile.FileName);
                    MapReader.ReadMap(map);

                    // Status update
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        txtMapName.Text = map.name;
                        txtMapDescription.Text = map.description;
                        m.lblLoading.Content = "Reading tiles ...";
                    }));

                    // Read tiles for objects and splines
                    TileReader.ReadAllTiles(map);
                    ScoReader.ReadAllObjects(map);
                    O3DReader.ReadAllO3DTextures(map);

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading ailist.txt..."));

                    MapReader.ReadAilist(map);

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading parklist_p.txt..."));

                    MapReader.ReadParklist(map);

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading humans.txt, drivers.txt..."));

                    MapReader.ReadHumans(map);

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Collecting Data..."));

                    // Sort data and output to textboxes
                    map.objects.Sort();
                    map.splines.Sort();
                    map.vehicles.Sort();
                    map.humans.Sort();

                    List<Tile> tilesMissing = map.tiles.FindAll(OmsiFile.IsFileMissing);
                    List<Sceneryobject> objectsMissing = map.objects.FindAll(OmsiFile.IsFileMissing);
                    List<Spline> splinesMissing = map.splines.FindAll(OmsiFile.IsFileMissing);
                    List<Vehicle> vehiclesMissing = map.vehicles.FindAll(OmsiFile.IsFileMissing);
                    List<Human> humansMissing = map.humans.FindAll(OmsiFile.IsFileMissing);

                    // Use dispatch invoke to print list to textboxes
                    // Output final data to first page
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        // Use string.join to build one string instead of old foreach loops
                        lvTilesList.ItemsSource = map.tiles;
                        lvTilesMissingList.ItemsSource = tilesMissing;
                        lvObjectsList.ItemsSource = map.objects;
                        lvObjectsMissingList.ItemsSource = objectsMissing;
                        lvSplinesList.ItemsSource = map.splines;
                        lvSplinesMissingList.ItemsSource = splinesMissing;
                        lvVehiclesList.ItemsSource = map.vehicles;
                        lvVehiclesMissingList.ItemsSource = vehiclesMissing;
                        lvHumansList.ItemsSource = map.humans;
                        lvHumansMissingList.ItemsSource = humansMissing;

                        // Count objects and splines after reading map file
                        txtMapTileCount.Text = $"{map.tiles.Count}";
                        txtMapTileMissing.Text = $"{tilesMissing.Count}";
                        tabTiles.Header = $"Tiles ({tilesMissing.Count})";
                        txtMapObjectCount.Text = $"{map.objects.Count}";
                        txtMapObjectMissing.Text = $"{objectsMissing.Count}";
                        tabObjects.Header = $"Objects ({objectsMissing.Count})";
                        txtMapSplineCount.Text = $"{map.splines.Count}";
                        txtMapSplineMissing.Text = $"{splinesMissing.Count}";
                        tabSplines.Header = $"Splines ({splinesMissing.Count})";
                        txtMapAicarCount.Text = $"{map.vehicles.Count}";
                        txtMapAicarMissing.Text = $"{vehiclesMissing.Count}";
                        tabAicar.Header = $"AI Vehicles ({vehiclesMissing.Count})";
                        txtMapHumanCount.Text = $"{map.humans.Count}";
                        txtMapHumanMissing.Text = $"{humansMissing.Count}";
                        tabHuman.Header = $"Humans ({humansMissing.Count})";

                        // Try to read map picture
                        try
                        {
                            imgMap.Source = new BitmapImage(new Uri($"{map.folderPath}\\picture.jpg"));
                        }
                        catch (FileNotFoundException)
                        {
                            imgMap.Source = null;
                        }

                        // Re-enable main window and close loading screen
                        BlueSkyWindow.IsEnabled = true;
                        m.Close();
                    }));
                });
            }
        }

        private void ListView_UpdateColumnWidth(object sender, RoutedEventArgs e)
        {
            // Get the gridview to be resized
            ListView lv = sender as ListView;
            GridView gv = lv.View as GridView;

            // Get the total available width the gridview can be
            double availableWidth = lv.ActualWidth - SystemParameters.VerticalScrollBarWidth;

            // Subtract the width of the columns other than main filename column
            int columns = gv.Columns.Count;
            for (int i = 1; i < columns; i++) availableWidth -= gv.Columns[i].Width;
            
            // If available space for first column is still above 0 adjust accordingly
            if (availableWidth>0) gv.Columns[0].Width = availableWidth;
        }

        private static void ShowFileInFolder(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var psi = new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"/select,\"{filePath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open Explorer.\n\n{ex.Message}", "Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void lvTilesList_ClickExplore(object sender, RoutedEventArgs e)
        {
            ShowFileInFolder($"{map.folderPath}\\{((sender as MenuItem).DataContext as Tile).fileName}");
        }

        private void lvTilesList_ClickDetail(object sender, RoutedEventArgs e)
        {
            TileDetailWindow tw = new((sender as MenuItem).DataContext as Tile);
            tw.Show();
        }

        private void lvObjectsList_ClickExplore(object sender, RoutedEventArgs e)
        {
            ShowFileInFolder($"{map.omsiPath}\\{((sender as MenuItem).DataContext as Sceneryobject).fileName}");
        }

        private void lvObjectsList_ClickDetail(object sender, RoutedEventArgs e)
        {
        }

        private void lvSplinesList_ClickExplore(object sender, RoutedEventArgs e)
        {
            ShowFileInFolder($"{map.omsiPath}\\{((sender as MenuItem).DataContext as Spline).fileName}");
        }

        private void lvSplinesList_ClickDetail(object sender, RoutedEventArgs e)
        {
        }

        private void lvVehiclesList_ClickExplore(object sender, RoutedEventArgs e)
        {
            ShowFileInFolder($"{map.omsiPath}\\{((sender as MenuItem).DataContext as Vehicle).fileName}");
        }

        private void lvHumansList_ClickExplore(object sender, RoutedEventArgs e)
        {
            ShowFileInFolder($"{map.omsiPath}\\{((sender as MenuItem).DataContext as Human).fileName}");
        }

        private void tabMap_Clear()
        {
            txtMapDir.Clear();
            txtMapName.Clear();
            txtMapTileCount.Clear();
            txtMapTileMissing.Clear();
            txtMapObjectCount.Clear();
            txtMapObjectMissing.Clear();
            txtMapSplineCount.Clear();
            txtMapSplineMissing.Clear();
            txtMapAicarCount.Clear();
            txtMapAicarMissing.Clear();
            txtMapHumanCount.Clear();
            txtMapHumanMissing.Clear();
            txtMapDescription.Clear();
        }

        private void tabLogFile_Clear()
        {
            txtLogFileDir.Clear();
            tabLogFileWarn.Header = "Warnings";
            tabLogFileError.Header = "Errors";
            txtLogFileInfo.Clear();
            txtLogFileWarn.Clear();
            txtLogFileError.Clear();
        }

        private void lvTilesList_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
