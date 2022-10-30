using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace Blue_Sky
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pickLogFile = new OpenFileDialog
            {
                Filter = "OMSI Log File|logfile.txt|All files|*.*",
                InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\OMSI 2"
            };

            if (pickLogFile.ShowDialog() == true)
            {
                tabLogFile_Clear();
                txtLogFileDir.Text = pickLogFile.FileName;

                string[] logfile = File.ReadAllLines(pickLogFile.FileName);

                List<string> info = new List<string>();
                List<string> warn = new List<string>();
                List<string> error = new List<string>();
                foreach (string line in logfile)
                {
                    if (line.Contains(" -  -   "))
                    {
                        // Remove the time prefix of log file entries
                        string newLine = line.Split(new[] { " -  -   " }, StringSplitOptions.None)[1].TrimStart();

                        // Check for repeating entries, only add non duplicates to list and list box
                        if (newLine.StartsWith("Information:") && !info.Contains(newLine.Substring(13)))
                        {
                            newLine = newLine.Substring(13);
                            info.Add(newLine);
                            txtLogFileInfo.Text += newLine + "\r\n";
                        }
                        if (newLine.StartsWith("Warning:") && !warn.Contains(newLine.Substring(15)))
                        {
                            newLine = newLine.Substring(15);
                            warn.Add(newLine);
                            txtLogFileWarn.Text += newLine + "\r\n";
                        }
                        if (newLine.StartsWith("Error:") && !error.Contains(newLine.Substring(17)))
                        {
                            newLine = newLine.Substring(17);
                            error.Add(newLine);
                            txtLogFileError.Text += newLine + "\r\n";
                        }
                    }
                    tabLogFileWarn.Header = "Warnings (" + warn.Count + ")";
                    tabLogFileError.Header = "Errors (" + error.Count + ")";
                }
            }
        }

        private void btnMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pickMapFile = new OpenFileDialog();
            pickMapFile.Filter = "OMSI Map File|global.cfg|All files (*.*)|*.*";
            pickMapFile.InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\OMSI 2\maps";

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
                MapLoadScreen m = new MapLoadScreen();
                m.Show();

                Task task = Task.Factory.StartNew(() =>
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Reading global.cfg...";
                        BlueSkyWindow.IsEnabled = false;
                    }));

                    string[] mapFile = File.ReadAllLines(pickMapFile.FileName);

                    // Lists to store file locations
                    List<string> tiles = new List<string>();
                    List<string> tilesMissing = new List<string>();
                    List<string> objects = new List<string>();
                    List<string> objectsMissing = new List<string>();
                    List<string> splines = new List<string>();
                    List<string> splinesMissing = new List<string>();
                    List<string> aicars = new List<string>();
                    List<string> aicarsMissing = new List<string>();
                    List<string> humans = new List<string>();
                    List<string> humansMissing = new List<string>();

                    // Read map file
                    for (int i = 0; i < mapFile.Length; i++)
                    {
                        // Read map name
                        if (mapFile[i].StartsWith("[name]") && (i + 1) < mapFile.Length)
                        {
                            this.Dispatcher.Invoke((Action)(() =>
                            {
                                txtMapName.Text = mapFile[i + 1];
                            }));
                        }

                        // Read description
                        else if (mapFile[i].StartsWith("[description]"))
                        {
                            // Move to next line, if not end of file and description end, add line to description textbox
                            for (i++; i < mapFile.Length && !mapFile[i].StartsWith("[end]"); i++)
                            {
                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    txtMapDescription.Text += mapFile[i] + "\r\n";
                                }));
                            }
                        }

                        // Read tile list
                        else if (mapFile[i].StartsWith("[map]") && (i + 3) < mapFile.Length)
                        {
                            // Check duplicates
                            if (!tiles.Contains(mapFile[i + 3]))
                            {
                                tiles.Add(mapFile[i + 3]);

                                // Check missing tiles at the same time, if file not exist add to missing tiles list
                                if (!File.Exists(Directory.GetParent(pickMapFile.FileName) + "\\" + mapFile[i + 3]))
                                {
                                    tilesMissing.Add(mapFile[i + 3]);
                                }
                            }
                        }
                    }

                    // Count tile after reading map file
                    // No, this step is merged with object spline counting etc

                    //this.Dispatcher.Invoke((Action)(() =>
                    //{
                    //    txtMapTileCount.Text = tiles.Count.ToString();
                    //    txtMapTileMissing.Text = tilesMissing.Count.ToString();
                    //}));

                    // Set progress bar to 10 after finish reading for tiles
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.pbrLoading.Value = 10;
                    }));

                    // Scan tiles for objects and splines
                    for (int t = 0; t < tiles.Count; t++)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            m.lblLoading.Content = "Reading tiles (" + (t + 1) + "/" + tiles.Count + ")...";
                            m.pbrLoading.Value = 10 + (double)t / (double)tiles.Count * 80d;
                        }));

                        if (File.Exists(Directory.GetParent(pickMapFile.FileName) + "\\" + tiles[t]))
                        {
                            string[] tileFile = File.ReadAllLines(Directory.GetParent(pickMapFile.FileName) + "\\" + tiles[t]);
                            for (int i = 0; i < tileFile.Length; i++)
                            {
                                // Read for objects
                                if ((tileFile[i].StartsWith("[object]") || tileFile[i].StartsWith("[splineAttachement]") || tileFile[i].StartsWith("[attachObj]")) && (i + 2) < tileFile.Length)
                                {
                                    if (!objects.Contains(tileFile[i + 2]))
                                    {
                                        objects.Add(tileFile[i + 2]);
                                        if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + tileFile[i + 2]))
                                        {
                                            objectsMissing.Add(tileFile[i + 2]);
                                        }
                                    }
                                }

                                // Read for splines
                                if ((tileFile[i].StartsWith("[spline]") || tileFile[i].StartsWith("[spline_h]")) && (i + 2) < tileFile.Length)
                                {
                                    if (!splines.Contains(tileFile[i + 2]))
                                    {
                                        splines.Add(tileFile[i + 2]);
                                        if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + tileFile[i + 2]))
                                        {
                                            splinesMissing.Add(tileFile[i + 2]);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Set progress bar to 90 after finish reading for objects and splines, then set to start reading ai list
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Reading ailist.txt...";
                        m.pbrLoading.Value = 90;
                    }));

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
                                    if (!aicars.Contains(aiList[i+j]))
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

                    // Set progress bar to 95 after reading ai list, then set start read parklist
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Reading parklist_p.txt...";
                        m.pbrLoading.Value = 95;
                    }));

                    // Scan park list
                    String[] parklists = Directory.EnumerateFiles(Directory.GetParent(pickMapFile.FileName).ToString(), "*.txt", SearchOption.TopDirectoryOnly).Select(System.IO.Path.GetFileName).Where(f => f.StartsWith("parklist_p")).ToArray();

                    foreach (string parklist in parklists)
                    {
                        string[] parkcars = File.ReadAllLines(Directory.GetParent(pickMapFile.FileName) + "\\" + parklist);
                        foreach (string car in parkcars)
                        {
                            if(!objects.Contains(car))
                            {
                                objects.Add(car);
                                if (!File.Exists(Directory.GetParent(Directory.GetParent(Directory.GetParent(pickMapFile.FileName).FullName).FullName) + "\\" + car))
                                {
                                    objectsMissing.Add(car);
                                }
                            }
                        }
                    }

                    // Set progress bar to 98 after finish reading parklist, then set to start reading human
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Reading humans.txt...";
                        m.pbrLoading.Value = 98;
                    }));

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

                    // Set progress bar to 100 after finish reading human, then set to collecting data
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        m.lblLoading.Content = "Collecting Data...";
                        m.pbrLoading.Value = 100;
                    }));

                    // Sort data and output to textboxes
                    /*
                     * No need to sort tiles for easy reference to global.cfg
                     * tiles.Sort();
                     * tilesMissing.Sort();
                     */
                    objects.Sort();
                    objectsMissing.Sort();
                    splines.Sort();
                    splinesMissing.Sort();
                    aicars.Sort();
                    aicarsMissing.Sort();
                    humans.Sort();
                    humansMissing.Sort();

                    // Use dispatch invoke to print list to textboxes
                    foreach (string file in tiles)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtTilesList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in tilesMissing)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtTilesMissingList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in objects)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtObjectsList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in objectsMissing)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtObjectsMissingList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in splines)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtSplinesList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in splinesMissing)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtSplinesMissingList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in aicars)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtAicarsList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in aicarsMissing)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtAicarsMissingList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in humans)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtHumansList.Text += file + "\r\n";
                        }));
                    }
                    foreach (string file in humansMissing)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            txtHumansMissingList.Text += file + "\r\n";
                        }));
                    }

                    // Output final data to first page
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        // Count objects and splines after reading map file
                        txtMapTileCount.Text = tiles.Count.ToString();
                        txtMapTileMissing.Text = tilesMissing.Count.ToString();
                        tabTiles.Header = "Tiles (" + tilesMissing.Count.ToString() + " Missing)";
                        txtMapObjectCount.Text = objects.Count.ToString();
                        txtMapObjectMissing.Text = objectsMissing.Count.ToString();
                        tabObjects.Header = "Objects (" + objectsMissing.Count.ToString() + " Missing)";
                        txtMapSplineCount.Text = splines.Count.ToString();
                        txtMapSplineMissing.Text = splinesMissing.Count.ToString();
                        tabSplines.Header = "Splines (" + splinesMissing.Count.ToString() + " Missing)";
                        txtMapAicarCount.Text = aicars.Count.ToString();
                        txtMapAicarMissing.Text = aicarsMissing.Count.ToString();
                        tabAicar.Header = "AI Vehicles (" + aicarsMissing.Count.ToString() + " Missing)";
                        txtMapHumanCount.Text = humans.Count.ToString();
                        txtMapHumanMissing.Text = humansMissing.Count.ToString();
                        tabHuman.Header = "Humans (" + humansMissing.Count.ToString() + " Missing)";

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
