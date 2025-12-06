using Blue_Sky.Classes;
using System.Windows;

namespace Blue_Sky.Windows
{
    /// <summary>
    /// Interaction logic for TileDetailWindow.xaml
    /// </summary>
    public partial class TileDetailWindow : Window
    {
        private readonly Tile tile;

        public TileDetailWindow(Tile tile)
        {
            InitializeComponent();
            this.tile = tile;
            PopulateWindow();
        }

        private void PopulateWindow()
        {
            this.Title = $"Details of {tile.fileName}";
            lblTitle.Content = tile.fileName;
        }
    }
}
