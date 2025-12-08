using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Sceneryobject(string fileName, string path) : OmsiFile(fileName, path)
    {
        private HashSet<string> readO3ds = [];
        private HashSet<string> readTextures = [];
        private HashSet<string> readScripts = [];

        public List<O3D> o3ds { get; } = [];
        public List<Texture> textures { get; } = [];
        public List<Script> scripts { get; } = [];

        public bool AddO3D(O3D o3d)
        {
            bool exists = readO3ds.Add(o3d.fileName);
            if (exists)
                this.o3ds.Add(o3d);
            return exists;
        }

        public bool AddTexture(Texture matl)
        {
            bool exists = readTextures.Add(matl.fileName);
            if (exists)
                this.textures.Add(matl);
            return exists;
        }

        public bool AddScript(Script script)
        {
            bool exists = readScripts.Add(script.fileName);
            if (exists)
                this.scripts.Add(script);
            return exists;
        }
    }
}
