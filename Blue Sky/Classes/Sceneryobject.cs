namespace Blue_Sky.Classes
{
    internal class Sceneryobject(string fileName)
    {
        public string fileName = fileName;
        public bool isMissing = false;

        public static bool IsObjectMissing(Sceneryobject sceneryobject)
        {
            return sceneryobject.isMissing;
        }
    }
}
