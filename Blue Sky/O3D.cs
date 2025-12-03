using System.Collections.Generic;
using System.Linq;

namespace Blue_Sky
{
    public class O3D
    {
        private readonly List<Material> materials;

        public O3D() {
            materials = [];
        }

        public void AddMaterial(string matlPath)
        {
            materials.Add(new Material(matlPath));
        }

        public List<string> GetMaterialPathList()
        {
            return [.. materials.Select(x => x.ToString())];
        }

        private class Material(string matlPath)
        {
            private readonly string matlPath = matlPath;

            override public string ToString()
            {
                return matlPath; 
            }
        }

    }

}
