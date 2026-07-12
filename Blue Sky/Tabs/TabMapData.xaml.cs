using Blue_Sky.Classes;
using Blue_Sky.Readers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Blue_Sky.Tabs
{
    /// <summary>
    /// Interaction logic for TabMapData.xaml
    /// </summary>
    public partial class TabMapData : Tab
    {
        private Map map;

        public TabMapData()
        {
            InitializeComponent();
        }

        public void btnMap_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pickMapFile = new()
            {
                Filter = "OMSI Map File|global.cfg|All files (*.*)|*.*",
                InitialDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\OMSI 2\maps",
                ShowHiddenItems = true
            };

            if (pickMapFile.ShowDialog() == true)
            {
                // Clear everything
                tabMap_Clear();

                txtMapDir.Text = pickMapFile.FileName;

                // Show map load screen
                MapLoadScreen m = new() { Topmost = true };
                m.Show();
                this.mainWindow.BlueSkyWindow.IsEnabled = false;

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
                    SliReader.ReadAllSplinees(map);

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
                    List<Texture> texturesMissing = map.textures.FindAll(OmsiFile.IsFileMissing);

                    // Use dispatch invoke to print list to textboxes
                    // Output final data to first page
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        // Use string.join to build one string instead of old foreach loops
                        this.mainWindow.ucTabTiles.lvTilesList.ItemsSource = map.tiles;
                        this.mainWindow.ucTabTiles.lvTilesMissingList.ItemsSource = tilesMissing;
                        this.mainWindow.ucTabObjects.lvObjectsList.ItemsSource = map.objects;
                        this.mainWindow.ucTabObjects.lvObjectsMissingList.ItemsSource = objectsMissing;
                        this.mainWindow.ucTabSplines.lvSplinesList.ItemsSource = map.splines;
                        this.mainWindow.ucTabSplines.lvSplinesMissingList.ItemsSource = splinesMissing;
                        this.mainWindow.ucTabAicars.lvVehiclesList.ItemsSource = map.vehicles;
                        this.mainWindow.ucTabAicars.lvVehiclesMissingList.ItemsSource = vehiclesMissing;
                        this.mainWindow.ucTabHumans.lvHumansList.ItemsSource = map.humans;
                        this.mainWindow.ucTabHumans.lvHumansMissingList.ItemsSource = humansMissing;
                        this.mainWindow.ucTabTextures.lvTexturesList.ItemsSource = map.textures;
                        this.mainWindow.ucTabTextures.lvTexturesMissingList.ItemsSource = texturesMissing;

                        // Count objects and splines after reading map file
                        txtMapTileCount.Text = $"{map.tiles.Count}";
                        txtMapTileMissing.Text = $"{tilesMissing.Count}";
                        this.mainWindow.tabTiles.Header = $"Tiles ({tilesMissing.Count})";
                        txtMapObjectCount.Text = $"{map.objects.Count}";
                        txtMapObjectMissing.Text = $"{objectsMissing.Count}";
                        this.mainWindow.tabObjects.Header = $"Objects ({objectsMissing.Count})";
                        txtMapSplineCount.Text = $"{map.splines.Count}";
                        txtMapSplineMissing.Text = $"{splinesMissing.Count}";
                        this.mainWindow.tabSplines.Header = $"Splines ({splinesMissing.Count})";
                        txtMapAicarCount.Text = $"{map.vehicles.Count}";
                        txtMapAicarMissing.Text = $"{vehiclesMissing.Count}";
                        this.mainWindow.tabAicars.Header = $"AI Vehicles ({vehiclesMissing.Count})";
                        txtMapHumanCount.Text = $"{map.humans.Count}";
                        txtMapHumanMissing.Text = $"{humansMissing.Count}";
                        this.mainWindow.tabHumans.Header = $"Humans ({humansMissing.Count})";
                        txtMapTextureCount.Text = $"{map.textures.Count}";
                        txtMapTextureMissing.Text = $"{texturesMissing.Count}";
                        this.mainWindow.tabTextures.Header = $"Textures ({texturesMissing.Count})";

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
                        this.mainWindow.BlueSkyWindow.IsEnabled = true;
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
    }
}
