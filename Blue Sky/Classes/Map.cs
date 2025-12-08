using Blue_Sky.Readers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue_Sky.Classes
{
    public class Map(string globalcfgPath)
    {
        public string globalcfgPath = globalcfgPath;
        public string folderPath = FolderHelper.UpDir(globalcfgPath);
        public string omsiPath = FolderHelper.UpDir(globalcfgPath, 2);
        public string name = "";
        public string description = "";

        private readonly HashSet<string> readTiles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readObjects = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readSplines = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readVehicles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readHumans = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readO3ds = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readTextures = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readScripts = new(StringComparer.OrdinalIgnoreCase);

        public readonly List<Tile> tiles = [];
        public readonly List<Sceneryobject> objects = [];
        public readonly List<Spline> splines = [];
        public readonly List<Vehicle> vehicles = [];
        public readonly List<Human> humans = [];
        public readonly List<O3D> o3ds = [];
        public readonly List<Texture> textures = [];
        public readonly List<Script> scripts = [];

        public bool AddTile(Tile tile)
        {
            bool exists = readTiles.Add($"{tile.path}\\{tile.fileName}");
            if (exists) this.tiles.Add(tile);
            return exists;
        }
        public bool AddObject(Sceneryobject sceneryobject)
        {
            bool exists = readObjects.Add($"{sceneryobject.path}\\{sceneryobject.fileName}");
            if (exists)
                this.objects.Add(sceneryobject);
            return exists;
        }
        public bool AddSpline(Spline spline)
        {
            bool exists = readSplines.Add($"{spline.path}\\{spline.fileName}");
            if (exists)
                this.splines.Add(spline);
            return exists;
        }
        public bool AddVehicle(Vehicle vehicle)
        {
            bool exists = readVehicles.Add($"{vehicle.path}\\{vehicle.fileName}");
            if (exists) this.vehicles.Add(vehicle);
            return exists;
        }
        public bool AddHuman(Human human)
        {
            bool exists = readHumans.Add($"{human.path}\\{human.fileName}");
            if (exists) this.humans.Add(human);
            return exists;
        }
        public bool AddO3D(O3D o3d)
        {
            bool exists = readO3ds.Add($"{o3d.path}\\model\\{o3d.fileName}");
            if (exists)
                this.o3ds.Add(o3d);
            return exists;
        }
        public bool AddTexture(Texture matl)
        {
            bool exists = readTextures.Add($"{matl.path}\\texture\\{matl.fileName}");
            if (exists)
                this.textures.Add(matl);
            return exists;
        }
        public bool AddScript(Script script)
        {
            bool exists = readScripts.Add($"{script.path}\\{script.fileName}");
            if (exists)
                this.scripts.Add(script);
            return exists;
        }

        public List<string> GetTilePaths()
        {
            return [.. tiles.Select(x => x.fileName)];
        }
        public List<string> GetMissingTilePaths()
        {
            return [.. tiles.FindAll(OmsiFile.IsFileMissing).Select(x => x.fileName)];
        }
        public List<string> GetObjectPaths()
        {
            return [.. objects.Select(x => x.fileName)];
        }
        public List<string> GetMissingObjectPaths()
        {
            return [.. objects.FindAll(OmsiFile.IsFileMissing).Select(x => x.fileName)];
        }
        public List<string> GetSplinePaths()
        {
            return [.. splines.Select(x => x.fileName)];
        }
        public List<string> GetMissingSplinesPaths()
        {
            return [.. splines.FindAll(OmsiFile.IsFileMissing).Select(x => x.fileName)];
        }
        public List<string> GetVehiclePaths()
        {
            return [.. vehicles.Select(x => x.fileName)];
        }
        public List<string> GetMissingVehiclePaths()
        {
            return [.. vehicles.FindAll(OmsiFile.IsFileMissing).Select(x => x.fileName)];
        }
        public List<string> GetHumanPaths()
        {
            return [.. humans.Select(x => x.fileName)];
        }
        public List<string> GetMissingHumanPaths()
        {
            return [.. humans.FindAll(OmsiFile.IsFileMissing).Select(x => x.fileName)];
        }
    }
}
