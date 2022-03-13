using System.Collections.Generic;

namespace Console
{
    internal class Program
    {
        private static readonly object _crititcalSection = new object();

        private static readonly Dictionary<string, string> _criticalResource = new Dictionary<string, string>();
        
        public static void Main(string[] args)
        {
            //  There is no way to do this any other way around!! I can't! I can't!
            lock (_crititcalSection)
            {
                _criticalResource["Hello"] = "World";
            }
        }
    }
}