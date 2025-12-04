using System;
using System.Collections.Generic;
using System.Linq;

namespace Blue_Sky.Classes
{
    internal class Map()
    {
        public string name = "";
        public string description = "";

        private readonly HashSet<string> readTiles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readVehicles = new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> readHumans = new(StringComparer.OrdinalIgnoreCase);

        private readonly List<Tile> tiles = [];
        private readonly List<Vehicle> vehicles = [];
        private readonly List<Human> humans = [];

        public bool AddTile(Tile tile)
        {
            bool newEntry = readTiles.Add(tile.path);
            if (newEntry) this.tiles.Add(tile);
            return newEntry;
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
            return [.. tiles.Select(x => x.path)];
        }

        public List<string> GetMissingTilePaths()
        {
            return [.. tiles.FindAll(Tile.MissingTile).Select(x => x.path)];
        }
    }
}
