namespace Blue_Sky.Classes
{
    internal class Spline(string filePath)
    {
        public string fileName = filePath;
        public bool isMissing = false;

        public static bool IsSplineMissing(Spline spline)
        {
            return spline.isMissing;
        }
    }
}
