using Blue_Sky.Classes;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Blue_Sky.Readers
{
    internal static class LogReader
    {
        private const string INFO_PREFIX = "Information:";
        private const string WARN_PREFIX = "Warning:";
        private const string ERROR_PREFIX = "Error:";
        private const string SEPARATOR = " -  -   ";

        public static async Task ReadLogfileAsync(string path, Logfile file)
        {
            try
            {
                using var reader = new StreamReader(path, System.Text.Encoding.UTF8);
                string? line;

                while ((line = await reader.ReadLineAsync()) != null)
                {
                    // Remove the time prefix of log file entries
                    int sepIdx = line.IndexOf(SEPARATOR, StringComparison.Ordinal);
                    if (sepIdx < 0) continue;

                    string newLine = line[(sepIdx + SEPARATOR.Length)..].TrimStart();

                    // Check for repeating entries, only add non duplicates to list and list box
                    if (newLine.StartsWith(INFO_PREFIX) && file.readInfo.Add(newLine[13..]))
                    {
                        file.info.Add(newLine[13..]);
                    }
                    if (newLine.StartsWith(WARN_PREFIX) && file.readWarn.Add(newLine[15..]))
                    {
                        file.warn.Add(newLine[15..]);
                    }
                    if (newLine.StartsWith(ERROR_PREFIX) && file.readError.Add(newLine[17..]))
                    {
                        file.error.Add(newLine[17..]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
