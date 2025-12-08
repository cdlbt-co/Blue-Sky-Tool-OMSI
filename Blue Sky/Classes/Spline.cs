using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Spline(string fileName, string path) : OmsiFile(fileName, path)
    {
        private HashSet<string> readTextures = [];
        public List<Texture> textures { get; } = [];

        public bool AddTexture(Texture matl)
        {
            bool exists = readTextures.Add(matl.fileName);
            if (exists)
                this.textures.Add(matl);
            return exists;
        }
    }
}
