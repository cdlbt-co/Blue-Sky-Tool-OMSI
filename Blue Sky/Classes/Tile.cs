using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    internal class Tile(string path)
    {
        public string path = path;
        public bool isMissing = false;

        private HashSet<Sceneryobject> readObjects = [];
        private HashSet<Spline> readSplines = [];

        public List<Sceneryobject> objects = [];
        public List<Spline> splines = [];

        public bool AddObject(Sceneryobject sceneryobject)
        {
            bool exists = readObjects.Add(sceneryobject);
            if (exists)
                this.objects.Add(sceneryobject);
            return exists;
        }

        public bool AddSpline(Spline spline)
        {
            bool exists = readSplines.Add(spline);
            if (exists)
                this.splines.Add(spline);
            return exists;
        }

        public static bool MissingTile(Tile tile)
        {
            return tile.isMissing;
        }
    }
}
