namespace Blue_Sky.Classes
{
    public class Spline(string filePath)
    {
        public string fileName = filePath;
        public bool isMissing = false;

        public static bool IsSplineMissing(Spline spline)
        {
            return spline.isMissing;
        }
    }
}
