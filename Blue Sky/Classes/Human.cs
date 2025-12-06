using System;

namespace Blue_Sky.Classes
{
    public class Human(string fileName) : IComparable<Human>
    {
        public string fileName { get; set; } = fileName;
        public bool isMissing { get; set; } = false;

        public static bool IsHumanMissing(Human human)
        {
            return human.isMissing;
        }

        public int CompareTo(Human other)
        {
            if (other is null) return 1;

            return string.Compare(this.fileName,
                other.fileName,
                StringComparison.OrdinalIgnoreCase);

        }
    }
}
