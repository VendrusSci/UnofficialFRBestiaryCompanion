using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary
{
    static class ApplicationPaths
    {
        public static string GetDataDirectory()
        {
#if DEBUG
            return "..\\..\\";
#else
            return Directory.GetCurrentDirectory();
#endif
        }

        public static string GetResourcesDirectory()
        {
            return Path.Combine(GetDataDirectory(), "Resources");
        }
    }
}
