using System;
using System.Collections.Generic;

namespace Blue_Sky.Classes
{
    internal class Logfile
    {
        public readonly HashSet<string> readInfo = new(StringComparer.OrdinalIgnoreCase);
        public readonly HashSet<string> readWarn = new(StringComparer.OrdinalIgnoreCase);
        public readonly HashSet<string> readError = new(StringComparer.OrdinalIgnoreCase);
        public readonly List<string> info = [];
        public readonly List<string> warn = [];
        public readonly List<string> error = [];
    }
}
