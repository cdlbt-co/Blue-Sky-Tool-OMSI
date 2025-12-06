using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue_Sky.Classes
{
    public class O3D(string fileName) : IComparable<O3D>
    {
        public string fileName { get; set; } = fileName;
        public bool isMissing { get; set; } = false;

        private readonly List<Material> materials = [];

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
        public int CompareTo(O3D other)
        {
            if (other is null) return 1;

            return string.Compare(this.fileName,
                other.fileName,
                StringComparison.OrdinalIgnoreCase);

        }
    }

}
