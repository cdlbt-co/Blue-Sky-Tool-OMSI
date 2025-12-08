using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Spline(string fileName, string path) : OmsiFile(fileName, path)
    {
        public List<Texture> textures { get; } = [];

    }
}
