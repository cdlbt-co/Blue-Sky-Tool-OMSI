using Blue_Sky.Classes;
using Blue_Sky.Readers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Blue_Sky
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        Map map;

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
                MapLoadScreen m = new()
                {
                    Topmost = true
                };
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

                    // Lists to store file locations
                    List<string> tiles = map.GetTilePaths();
                    List<string> tilesMissing = map.GetMissingTilePaths();
                    List<string> objects = map.GetObjectPaths();
                    List<string> objectsMissing = map.GetMissingObjectPaths();
                    List<string> splines = map.GetSplinePaths();
                    List<string> splinesMissing = map.GetMissingSplinesPaths();
                    List<string> aicars = [];
                    List<string> aicarsMissing = [];
                    List<string> humans = [];
                    List<string> humansMissing = [];

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading ailist.txt..."));

                    // Scan ai list
                    if (File.Exists(Directory.GetParent(pickMapFile.FileName) + "\\ailists.cfg"))
                    {
                        string[] aiList = File.ReadAllLines(Directory.GetParent(pickMapFile.FileName) + "\\ailists.cfg");
                        for (int i = 0; i < aiList.Length; i++)
                        {
                            // Omsi1 ailist
                            if ((aiList[i].StartsWith("[ailist]") && (i + 3) < aiList.Length))
                            {
                                int.TryParse(aiList[i + 3], out int length);
                                i += 4;

                                for (int j = 0; j < length && i + length < aiList.Length; j++)
                                {
                                    if (!aicars.Contains(aiList[i + j]))
                                    {
                                        aicars.Add(aiList[i + j]);
                                        if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + aiList[i + j]))
                                        {
                                            aicarsMissing.Add(aiList[i + j]);
                                        }
                                    }
                                }
                                i += length;
                            }

                            // Omsi2 ailist
                            if ((aiList[i].StartsWith("[aigroup_2]") && (i + 2) < aiList.Length))
                            {
                                for (i += 3; i < aiList.Length && !aiList[i].StartsWith("[end]"); i++)
                                {
                                    if (!aicars.Contains(aiList[i].Split('\t')[0]))
                                    {
                                        aicars.Add(aiList[i].Split('\t')[0]);
                                        if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + aiList[i].Split('\t')[0]))
                                        {
                                            aicarsMissing.Add(aiList[i].Split('\t')[0]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading parklist_p.txt..."));

                    // Scan park list
                    String[] parklists = Directory.EnumerateFiles(Directory.GetParent(pickMapFile.FileName).ToString(), "*.txt", SearchOption.TopDirectoryOnly).Select(System.IO.Path.GetFileName).Where(f => f.StartsWith("parklist_p")).ToArray();

                    foreach (string parklist in parklists)
                    {
                        string[] parkcars = File.ReadAllLines(Directory.GetParent(pickMapFile.FileName) + "\\" + parklist);
                        foreach (string car in parkcars)
                        {
                            if (!objects.Contains(car))
                            {
                                objects.Add(car);
                                if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + car))
                                {
                                    objectsMissing.Add(car);
                                }
                            }
                        }
                    }

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Reading humans.txt..."));

                    // Scan humans
                    if (File.Exists(Directory.GetParent(pickMapFile.FileName) + "\\humans.txt"))
                    {
                        string[] humanlist = File.ReadAllLines(Directory.GetParent(pickMapFile.FileName) + "\\humans.txt");
                        foreach (string human in humanlist)
                        {
                            if (!humans.Contains(human))
                            {
                                humans.Add(human);
                                if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + human))
                                {
                                    humansMissing.Add(human);
                                }
                            }
                        }
                    }

                    // Scan drivers
                    if (File.Exists(Directory.GetParent(pickMapFile.FileName) + "\\drivers.txt"))
                    {
                        string[] driverlist = File.ReadAllLines(Directory.GetParent(pickMapFile.FileName) + "\\drivers.txt");
                        foreach (string driver in driverlist)
                        {
                            if (!humans.Contains(driver))
                            {
                                humans.Add(driver);
                                if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + driver))
                                {
                                    humansMissing.Add(driver);
                                }
                            }
                        }
                    }

                    // Status update
                    this.Dispatcher.Invoke((Action)(() => m.lblLoading.Content = "Collecting Data..."));

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
                        txtTilesList.Text = String.Join("\r\n", tiles);
                        txtTilesMissingList.Text = String.Join("\r\n", tilesMissing);
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
                            imgMap.Source = new BitmapImage(new Uri(Directory.GetParent(pickMapFile.FileName) + "\\picture.jpg"));
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
    }
}
