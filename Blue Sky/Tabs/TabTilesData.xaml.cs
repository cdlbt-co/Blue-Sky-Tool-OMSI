using Blue_Sky.Classes;
using Blue_Sky.Windows;
using System.Windows;
using System.Windows.Controls;

namespace Blue_Sky.Tabs
{
    /// <summary>
    /// Interaction logic for TabTilesData.xaml
    /// </summary>
    public partial class TabTilesData : Tab
    {
        public TabTilesData()
        {
            InitializeComponent();
        }
        private void lvTilesList_ClickDetail(object sender, RoutedEventArgs e)
        {
            TileDetailWindow tw = new((sender as MenuItem).DataContext as Tile);
            tw.Show();
        }

    }
}
