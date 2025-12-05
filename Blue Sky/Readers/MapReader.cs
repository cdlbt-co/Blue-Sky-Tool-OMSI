using Blue_Sky.Classes;
using System.IO;
using System.Linq;

namespace Blue_Sky.Readers
{
    internal static class MapReader
    {
        public static void ReadMap(Map map)
        {
            string[] mapFile = File.ReadAllLines(map.globalcfgPath);

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
                    if (!File.Exists($"{map.folderPath}\\{tilePath}"))
                        newTile.isMissing = true;

                    map.AddTile(newTile);
                    i += 3;
                }
            }
        }

        public static void ReadAilist(Map map)
        {
            string ailistPath = $"{map.folderPath}\\ailists.cfg";
            // Read ai list
            if (!File.Exists(ailistPath)) return;

            string[] ailistFile = File.ReadAllLines(ailistPath);

            for (int i = 0; i < ailistFile.Length; i++)
            {
                // Omsi1 ailist
                if ((ailistFile[i].StartsWith("[ailist]") && (i + 3) < ailistFile.Length))
                {
                    if (!int.TryParse(ailistFile[i + 3], out int listLength)) continue;
                    i += 4;

                    for (int j = 0; j < listLength && i + listLength < ailistFile.Length; j++)
                    {
                        string vehiclePath = ailistFile[i + j];
                        Vehicle newVehicle = new(vehiclePath);

                        // Check if vehicle missing
                        if (!File.Exists($"{map.omsiPath}\\{vehiclePath}"))
                            newVehicle.isMissing = true;

                        map.AddVehicle(newVehicle);
                    }
                    i += listLength;
                }

                // Omsi2 ailist
                if ((ailistFile[i].StartsWith("[aigroup_2]") && (i + 2) < ailistFile.Length))
                {
                    for (i += 3; i < ailistFile.Length && !ailistFile[i].StartsWith("[end]"); i++)
                    {
                        string vehiclePath = ailistFile[i].Split('\t')[0];
                        Vehicle newVehicle = new(vehiclePath);

                        // Check if vehicle missing
                        if (!File.Exists($"{map.omsiPath}\\{vehiclePath}"))
                            newVehicle.isMissing = true;

                        map.AddVehicle(newVehicle);

                    }
                }
            }
        }

        public static void ReadParklist(Map map)
        {
            // Read park list(s)
            string[] parklistFileNames = [.. Directory.EnumerateFiles(map.folderPath, "*.txt", SearchOption.TopDirectoryOnly)
                .Select(System.IO.Path.GetFileName).Where(f => f.StartsWith("parklist_p"))];

            foreach (string fileName in parklistFileNames)
            {
                string[] parkedCarPaths = File.ReadAllLines($"{map.folderPath}\\{fileName}");

                foreach (string parkedCarPath in parkedCarPaths)
                {
                    Sceneryobject newParkedCar = new(parkedCarPath);

                    if (!File.Exists($"{map.omsiPath}\\{parkedCarPath}"))
                        newParkedCar.isMissing = true;

                    map.AddObject(newParkedCar);
                }
            }
        }

        public static void ReadHumans(Map map)
        {
            string humansPath = $"{map.folderPath}\\humans.txt";
            string driversPath = $"{map.folderPath}\\drivers.txt";
            string[] humansList = [];
            string[] driversList = [];

            // Read humans and drivers
            if (File.Exists(humansPath))
                humansList = File.ReadAllLines(humansPath);
            if (File.Exists(driversPath))
                driversList = File.ReadAllLines(driversPath);

            foreach (string humanPath in humansList.Union(driversList))
            {
                Human newHuman = new(humanPath);

                if (!File.Exists($"{map.omsiPath}\\{humanPath}"))
                    newHuman.isMissing = true;

                map.AddHuman(newHuman);
            }
        }
    }
}
