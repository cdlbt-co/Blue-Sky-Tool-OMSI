namespace Blue_Sky.Classes
{
    internal class Human(string fileName)
    {
        public string fileName = fileName;
        public bool isMissing = false;

        public static bool IsHumanMissing(Human human)
        {
            return human.isMissing;
        }
    }
}
