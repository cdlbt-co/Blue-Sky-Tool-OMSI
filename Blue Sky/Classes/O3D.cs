using System.Collections.Generic;
using System.Globalization;

namespace Blue_Sky.Classes
{
    public class O3D(string fileName, string path, Sceneryobject owner) : OmsiFile(fileName, path)
    {
        public readonly Sceneryobject owner = owner;

        private readonly HashSet<string> readMaterials = [];

        public readonly List<Texture> materials = [];

        public bool AddMaterial(Texture texture)
        {
            bool exists = readMaterials.Add(texture.fileName);
            if (exists)
                this.materials.Add(texture);
            return exists;
        }
    }

}
