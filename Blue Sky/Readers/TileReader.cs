using Blue_Sky.Classes;
using System.IO;

namespace Blue_Sky.Readers
{
    internal static class TileReader
    {
        public static void ReadAllTiles(Map map)
        {
            foreach (Tile tile in map.tiles)
            {
                if (!tile.isMissing) ReadTile(map, tile);
            }
        }

        private static void ReadTile(Map map, Tile tile)
        {
            // Scan tile if file exists
            string tilePath = $"{map.folderPath}\\{tile.fileName}";

            if (!File.Exists(tilePath)) return;

            string[] tileFile = File.ReadAllLines(tilePath);
            for (int i = 0; i < tileFile.Length; i++)
            {
                // Read for objects
                if ((tileFile[i].StartsWith("[object]") ||
                    tileFile[i].StartsWith("[splineAttachement]") ||
                    tileFile[i].StartsWith("[attachObj]")) &&
                    (i + 2) < tileFile.Length)
                {
                    string objectPath = tileFile[i + 2];
                    Sceneryobject newObject = new(objectPath, map.omsiPath);

                    // Check if object missing
                    if (!File.Exists($"{map.omsiPath}\\{objectPath}"))
                        newObject.isMissing = true;

                    tile.AddObject(newObject);
                    map.AddObject(newObject);
                    i += 2;
                }

                // Read for splines
                if ((tileFile[i].StartsWith("[spline]") ||
                    tileFile[i].StartsWith("[spline_h]")) &&
                    (i + 2) < tileFile.Length)
                {
                    string splinePath = tileFile[i + 2];
                    Spline newSpline = new(splinePath, map.omsiPath);

                    // Check if spline missing
                    if (!File.Exists($"{map.omsiPath}\\{splinePath}"))
                        newSpline.isMissing = true;
                    tile.AddSpline(newSpline);
                    map.AddSpline(newSpline);
                }
            }

        }
    }
}
