using System;
using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Sceneryobject(string fileName) : IComparable<Sceneryobject>
    {
        public string fileName { get; set; } = fileName;
        public bool isMissing { get; set; } = false;

        public List<O3D> o3ds { get; } = [];
        public List<string> textures { get; } = [];

        public static bool IsObjectMissing(Sceneryobject sceneryobject)
        {
            return sceneryobject.isMissing;
        }

        public int CompareTo(Sceneryobject other)
        {
            if (other is null) return 1;

            return string.Compare(this.fileName,
                other.fileName,
                StringComparison.OrdinalIgnoreCase);

        }
    }
}
