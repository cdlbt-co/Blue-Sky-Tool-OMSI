using Blue_Sky.Readers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue_Sky.Classes
{
    internal class Map(string globalcfgPath)
    {
        public string globalcfgPath = globalcfgPath;
        public string folderPath = FolderHelper.GetParent(globalcfgPath);
        public string omsiPath = FolderHelper.GetParent(globalcfgPath, 2);
        public string name = "";
        public string description = "";

        private readonly HashSet<string> readTiles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readObjects = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readSplines = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readVehicles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readHumans = new(StringComparer.OrdinalIgnoreCase);

        public readonly List<Tile> tiles = [];
        public readonly List<Sceneryobject> objects = [];
        public readonly List<Spline> splines = [];
        public readonly List<Vehicle> vehicles = [];
        public readonly List<Human> humans = [];

        public bool AddTile(Tile tile)
        {
            bool newEntry = readTiles.Add(tile.fileName);
            if (newEntry) this.tiles.Add(tile);
            return newEntry;
        }

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

        public bool AddVehicle(Vehicle vehicle)
        {
            bool newEntry = readVehicles.Add(vehicle.path);
            if (newEntry) this.vehicles.Add(vehicle);
            return newEntry;
        }

        public bool AddHuman(Human human)
        {
            bool newEntry = readHumans.Add(human.path);
            if (newEntry) this.humans.Add(human);
            return newEntry;
        }

        public List<string> GetTilePaths()
        {
            return [.. tiles.Select(x => x.fileName)];
        }

        public List<string> GetMissingTilePaths()
        {
            return [.. tiles.FindAll(Tile.IsTileMissing).Select(x => x.fileName)];
        }

        public List<string> GetObjectPaths()
        {
            return [.. objects.Select(x => x.fileName)];
        }

        public List<string> GetMissingObjectPaths()
        {
            return [.. objects.FindAll(Sceneryobject.IsObjectMissing).Select(x => x.fileName)];
        }
        public List<string> GetSplinePaths()
        {
            return [.. splines.Select(x => x.fileName)];
        }

        public List<string> GetMissingSplinesPaths()
        {
            return [.. splines.FindAll(Spline.IsSplineMissing).Select(x => x.fileName)];
        }
    }
}
