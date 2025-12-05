namespace Blue_Sky.Classes
{
    internal class Vehicle(string fileName)
    {
        public string fileName = fileName;
        public bool isMissing = false;

        public static bool IsVehicleMissing(Vehicle vehicle)
        {
            return vehicle.isMissing;
        }
    }
}
