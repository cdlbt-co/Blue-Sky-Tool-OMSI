using System;
using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    public class Tile(string fileName, string path) : OmsiFile(fileName, path)
    {

        private readonly HashSet<string> readObjects = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readSplines = new(StringComparer.OrdinalIgnoreCase);

        public List<Sceneryobject> objects { get; } = [];
        public List<Spline> splines { get; } = [];

        public bool AddObject(Sceneryobject sceneryobject)
        {
            bool exists = readObjects.Add(sceneryobject.fileName);
            if (exists)
                this.objects.Add(sceneryobject);
            return exists;
        }

        public bool AddSpline(Spline spline)
        {
            bool exists = readSplines.Add(spline.fileName);
            if (exists)
                this.splines.Add(spline);
            return exists;
        }
    }
}
