using Blue_Sky.Classes;
using System;
using System.IO;

namespace Blue_Sky.Readers
{
    internal static class ScoReader
    {
        public static void ReadAllObjects(Map map)
        {
            foreach (Sceneryobject sceneryobject in map.objects)
            {
                if (!sceneryobject.isMissing) ReadObject(map, sceneryobject);
            }
        }

        private static void ReadObject(Map map, Sceneryobject sceneryobject)
        {
            // Scan sco if file exists
            string scoPath = $"{map.omsiPath}\\{sceneryobject.fileName}";

            if (!File.Exists(scoPath)) return;

            string[] scoFile = File.ReadAllLines(scoPath);
            for (int i = 0; i < scoFile.Length; i++)
            {
                // Read for o3d
                if (scoFile[i].StartsWith("[mesh]") &&
                    (i + 1) < scoFile.Length &&
                    scoFile[i + 1].EndsWith("o3d", StringComparison.OrdinalIgnoreCase))
                {
                    string o3dFileName = scoFile[i + 1];
                    string scoFolder = FolderHelper.UpDir(scoPath);
                    O3D newO3d = new(o3dFileName, scoFolder, sceneryobject);

                    // Check if object missing
                    if (!File.Exists($"{scoFolder}\\model\\{o3dFileName}"))
                        newO3d.isMissing = true;

                    sceneryobject.AddO3D(newO3d);
                    map.AddO3D(newO3d);
                    i += 1;
                }

                // Read for script files
                if ((scoFile[i].StartsWith("[script]") ||
                    scoFile[i].StartsWith("[stringvarnamelist]") ||
                    scoFile[i].StartsWith("[varnamelist]")) &&
                    (i + 1) < scoFile.Length)
                {
                    if (!int.TryParse(scoFile[i + 1], out int listLength)) continue;
                    i += 2;

                    for (int j = 0; j < listLength && i + listLength < scoFile.Length; j++)
                    {
                        string scriptPath = scoFile[i + j];
                        Script newScript = new(scriptPath, map.omsiPath);

                        // Check if vehicle missing
                        if (!File.Exists($"{map.omsiPath}\\{scriptPath}"))
                            newScript.isMissing = true;

                        sceneryobject.AddScript(newScript);
                    }
                    i += listLength;
                }
            }
        }
    }
}
