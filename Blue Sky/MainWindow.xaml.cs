using Blue_Sky.Classes;
using Blue_Sky.Readers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Blue_Sky
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
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


        private void btno3d_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog picko3dFile = new OpenFileDialog
            {
                Filter = "OMSI 3D File|*.o3d|All files|*.*",
                InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\OMSI 2"
            };

            if (picko3dFile.ShowDialog() == true)
            {
                //tabLogFile_Clear();
                txto3dFileDir.Text = picko3dFile.FileName;

                // Show map load screen
                MapLoadScreen m = new()
                {
                    Topmost = true
                };
                m.Show();
                BlueSkyWindow.IsEnabled = false;

                Task task = Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading o3d..."));

                    O3D o3d = O3DReader.ReadO3D(picko3dFile.FileName);

                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Collecting Data...";

                        if (o3d != null) txto3dFileInfo.Text = String.Join("\r\n", o3d.GetMaterialPathList());

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
                tabObjects_Clear();
                tabSplines_Clear();
                tabAicars_Clear();
                tabHumans_Clear();

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

                    // Lists to store file locations
                    List<string> tiles = map.GetTilePaths();
                    List<string> tilesMissing = map.GetMissingTilePaths();
                    List<string> objects = map.GetObjectPaths();
                    List<string> objectsMissing = map.GetMissingObjectPaths();
                    List<string> splines = map.GetSplinePaths();
                    List<string> splinesMissing = map.GetMissingSplinesPaths();
                    List<string> aicars = map.GetVehiclePaths();
                    List<string> aicarsMissing = map.GetMissingVehiclePaths();
                    List<string> humans = map.GetHumanPaths();
                    List<string> humansMissing = map.GetMissingHumanPaths();

                    // Sort data and output to textboxes
                    /* No need to sort tiles for easy reference to global.cfg */
                    objects.Sort();
                    objectsMissing.Sort();
                    splines.Sort();
                    splinesMissing.Sort();
                    aicars.Sort();
                    aicarsMissing.Sort();
                    humans.Sort();
                    humansMissing.Sort();

                    // Use dispatch invoke to print list to textboxes
                    // Output final data to first page
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        // Use string.join to build one string instead of old foreach loops
                        lvTilesList.ItemsSource = map.tiles;
                        lvTilesMissingList.ItemsSource = map.tiles.FindAll(Tile.IsTileMissing);
                        txtObjectsList.Text = String.Join("\r\n", objects);
                        txtObjectsMissingList.Text = String.Join("\r\n", objectsMissing);
                        txtSplinesList.Text = String.Join("\r\n", splines);
                        txtSplinesMissingList.Text = String.Join("\r\n", splinesMissing);
                        txtAicarsList.Text = String.Join("\r\n", aicars);
                        txtAicarsMissingList.Text = String.Join("\r\n", aicarsMissing);
                        txtHumansList.Text = String.Join("\r\n", humans);
                        txtHumansMissingList.Text = String.Join("\r\n", humansMissing);

                        // Count objects and splines after reading map file
                        txtMapTileCount.Text = tiles.Count.ToString();
                        txtMapTileMissing.Text = tilesMissing.Count.ToString();
                        tabTiles.Header = $"Tiles ({tilesMissing.Count})";
                        txtMapObjectCount.Text = objects.Count.ToString();
                        txtMapObjectMissing.Text = objectsMissing.Count.ToString();
                        tabObjects.Header = $"Objects ({objectsMissing.Count})";
                        txtMapSplineCount.Text = splines.Count.ToString();
                        txtMapSplineMissing.Text = splinesMissing.Count.ToString();
                        tabSplines.Header = $"Splines ({splinesMissing.Count})";
                        txtMapAicarCount.Text = aicars.Count.ToString();
                        txtMapAicarMissing.Text = aicarsMissing.Count.ToString();
                        tabAicar.Header = $"AI Vehicles ({aicarsMissing.Count})";
                        txtMapHumanCount.Text = humans.Count.ToString();
                        txtMapHumanMissing.Text = humansMissing.Count.ToString();
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
            ListView lv = sender as ListView;
            GridView gv = lv.View as GridView;

            double availableWidth = lv.ActualWidth - SystemParameters.VerticalScrollBarWidth;

            int columns = gv.Columns.Count;
            for (int i = 1; i < columns; i++)
            {
                availableWidth -= gv.Columns[i].Width;
            }

            gv.Columns[0].Width = availableWidth;
        }

        private void lvTilesList_ClickExplore(object sender, RoutedEventArgs e)
        {

        }

        private void lvTilesList_ClickObjects(object sender, RoutedEventArgs e)
        {

        }

        private void lvTilesList_ClickSplines(object sender, RoutedEventArgs e)
        {

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

        private void tabObjects_Clear()
        {
            txtObjectsList.Clear();
            txtObjectsMissingList.Clear();
        }

        private void tabSplines_Clear()
        {
            txtSplinesList.Clear();
            txtSplinesMissingList.Clear();
        }
        private void tabAicars_Clear()
        {
            txtAicarsList.Clear();
            txtAicarsMissingList.Clear();
        }
        private void tabHumans_Clear()
        {
            txtHumansList.Clear();
            txtHumansMissingList.Clear();
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
