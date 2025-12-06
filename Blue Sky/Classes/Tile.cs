using System;
using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Tile(string fileName)
    {
        public string fileName { get; set; } = fileName;
        public bool isMissing { get; set; } = false;

        private readonly HashSet<string> readObjects = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readSplines = new(StringComparer.OrdinalIgnoreCase);

        public List<string> objects { get; } = [];
        public List<string> splines { get; } = [];

        public bool AddObject(Sceneryobject sceneryobject)
        {
            bool exists = readObjects.Add(sceneryobject.fileName);
            if (exists)
                this.objects.Add(sceneryobject.fileName);
            return exists;
        }

        public bool AddSpline(Spline spline)
        {
            bool exists = readSplines.Add(spline.fileName);
            if (exists)
                this.splines.Add(spline.fileName);
            return exists;
        }

        public static bool IsTileMissing(Tile tile)
        {
            return tile.isMissing;
        }
        public override bool Equals(object obj)
        {
            Tile tile2 = obj as Tile;
            if (tile2 == null) return false;
            return this.fileName == (obj as Tile).fileName;
        }

        public override int GetHashCode()
        {
            return this.fileName.GetHashCode();
        }
    }
}
