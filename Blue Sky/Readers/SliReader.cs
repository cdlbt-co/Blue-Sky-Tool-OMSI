using Blue_Sky.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Blue_Sky.Readers
{
    internal class SliReader
    {
        public static void ReadAllSplinees(Map map)
        {
            foreach (Spline spline in map.splines)
            {
                if (!spline.isMissing) ReadSpline(map,spline);
            }
        }

        private static void ReadSpline(Map map, Spline spline)
        {
            // Scan sli if file exists
            string sliPath = $"{map.omsiPath}\\{spline.fileName}";

            if (!File.Exists(sliPath)) return;

            string[] sliFile = File.ReadAllLines(sliPath);
            for (int i = 0; i < sliFile.Length; i++)
            {
                // Read for texture
                if (sliFile[i].StartsWith("[texture]") &&
                    (i + 1) < sliFile.Length)
                {
                    string textureFileName = sliFile[i + 1];
                    string textureFolderPath = $"{spline.path}\\texture";
                    Texture newTexture = new(textureFileName, textureFolderPath, "Spline");

                    // Check if object missing
                    if (!File.Exists($"{textureFolderPath}\\{textureFileName}"))
                        newTexture.isMissing = true;

                    spline.AddTexture(newTexture);
                    map.AddTexture(newTexture);
                    i += 1;
                }
            }
        }
    }
}
