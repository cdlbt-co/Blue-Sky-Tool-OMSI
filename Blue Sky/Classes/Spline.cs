using System;
using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Spline(string filePath) : IComparable<Spline>
    {
        public string fileName { get; set; } = filePath;
        public bool isMissing { get; set; } = false;

        public List<string> textures { get; } = [];

        public static bool IsSplineMissing(Spline spline)
        {
            return spline.isMissing;
        }

        public int CompareTo(Spline other)
        {
            if (other is null) return 1;

            return string.Compare(this.fileName,
                other.fileName,
                StringComparison.OrdinalIgnoreCase);

        }
    }
}
