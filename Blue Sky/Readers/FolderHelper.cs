using System.IO;

namespace Blue_Sky.Readers
{
    internal static class FolderHelper
    {
        public static string GetParent(string filePath, int level = 0)
        {
            string folderPath = filePath;
            for (int i = level; i >= 0; i--)
            {
                folderPath = Directory.GetParent(folderPath).FullName;
            }
            return folderPath;
        }
    }
}
