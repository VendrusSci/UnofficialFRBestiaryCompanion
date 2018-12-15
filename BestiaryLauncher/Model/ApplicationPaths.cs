using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestiaryLauncher.Model
{
    static class ApplicationPaths
    {
        public static string LauncherVersionFile = "launcherversion.txt";
        public static string UbcVersionFile = "version.txt";

        public static string RemoteLauncherVersionFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/BestiaryLauncher/launcherversion.txt";;
        public static string RemoteUbcVersionFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/version.txt";

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

        public static string GetLauncherVersionPath()
        {
            return Path.Combine(GetDataDirectory(), LauncherVersionFile);
        }

        public static string GetUBCVersionPath()
        {
            return Path.Combine(GetResourcesDirectory(), UbcVersionFile);
        }

        public static string GetTempDirectory()
        {
            return Path.Combine(GetDataDirectory(), "Temp");
        }
        
    }
}
