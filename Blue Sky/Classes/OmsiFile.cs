using System;

namespace Blue_Sky.Classes
{
    public class OmsiFile(string fileName, string path) : IComparable<OmsiFile>
    {
        public string fileName { get; set; } = fileName;
        public string path { get; set; } = path;
        public string fullPathName { get; set; } = $"{path}\\{fileName}";
        public bool isMissing { get; set; } = false;

        public override bool Equals(object obj)
        {
            if (obj is not OmsiFile other) return false;
            return this.fullPathName == other.fullPathName;
        }

        public override int GetHashCode()
        {
            return this.fullPathName.GetHashCode();
        }

        public int CompareTo(OmsiFile other)
        {
            if (other is null) return 1;
            return string.Compare(this.fullPathName,
                other.fullPathName,
                StringComparison.OrdinalIgnoreCase);

        }

        public static bool IsFileMissing(OmsiFile file)
        {
            return file.isMissing;
        }
    }
    public class Human(string fileName, string path) : OmsiFile(fileName, path);

    public class Script(string fileName, string path) : OmsiFile(fileName, path);

    public class Texture(string fileName, string path, string type) : OmsiFile(fileName, path)
    {
        public string type { get; set; } = type;
    }

    public class Vehicle(string fileName, string path) : OmsiFile(fileName, path);
}
