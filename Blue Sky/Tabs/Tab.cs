using Blue_Sky.Classes;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Blue_Sky.Tabs
{
    public class Tab : UserControl
    {

        public MainWindow mainWindow = null; // Reference to the MainWindow

        // get a reference to main windows when it is available.
        // The Loaded Event is set in the XAML code above.
        public void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = Window.GetWindow(this) as MainWindow;
        }

        public static void ShowFileInFolder(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ProcessStartInfo psi = new()
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

        public void ListView_ClickExplore(object sender, RoutedEventArgs e)
        {
            ShowFileInFolder(((sender as MenuItem).DataContext as OmsiFile).fullPathName);
        }

        public void ListView_UpdateColumnWidth(object sender, RoutedEventArgs e)
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
            if (availableWidth > 0) gv.Columns[0].Width = availableWidth;
        }

    }
}
