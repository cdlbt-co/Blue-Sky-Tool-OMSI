using System.IO;

namespace Blue_Sky.Classes
{
    internal static class MapReader
    {
        public static Map ReadMap(string mapPath)
        {
            string[] mapFile = File.ReadAllLines(mapPath);

            Map map = new();

            // Read map file
            for (int i = 0; i < mapFile.Length; i++)
            {
                // Read map name
                if (mapFile[i].StartsWith("[name]") && (i + 1) < mapFile.Length)
                {
                    map.name = mapFile[i + 1];
                }

                // Read description
                else if (mapFile[i].StartsWith("[description]"))
                {
                    string description = "";

                    // Move to next line if not end of file and description end
                    for (i++; i < mapFile.Length && !mapFile[i].StartsWith("[end]"); i++) description += mapFile[i] + "\r\n";

                    map.description = description;
                }

                // Read tile list
                else if (mapFile[i].StartsWith("[map]") && (i + 3) < mapFile.Length)
                {
                    string tilePath = mapFile[i + 3];
                    Tile newTile = new(tilePath);

                    // Check if tile missing
                    if (!File.Exists(Directory.GetParent(mapPath) + "\\" + tilePath))
                        newTile.isMissing = true;

                    map.AddTile(newTile);
                    i += 3;
                }
            }
            return map;
        }
    }
}
