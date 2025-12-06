using System;

namespace Blue_Sky.Classes
{
    public class Vehicle(string fileName) : IComparable<Vehicle>
    {
        public string fileName { get; set; } = fileName;
        public bool isMissing { get; set; } = false;

        public static bool IsVehicleMissing(Vehicle vehicle)
        {
            return vehicle.isMissing;
        }

        public int CompareTo(Vehicle other)
        {
            if (other is null) return 1;

            return string.Compare(this.fileName,
                other.fileName,
                StringComparison.OrdinalIgnoreCase);

        }
    }
}
