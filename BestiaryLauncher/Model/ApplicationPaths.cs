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
        public static string FRDataFile = "FRData.xml";
        public static string UserDataFile = "UserData.xml";
        public static string BookmarkFile = "BookmarkData.xml";
        public static string ExeFile = "Bestiary.exe";
        public static string LauncherExeFile = "BestiaryLauncher.exe";

        public static string RemoteLauncherVersionFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/BestiaryLauncher/launcherversion.txt";;
        public static string RemoteUbcVersionFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/version.txt";
        public static string RemoteFRDataFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/FRData.xml";
        public static string RemoteExePath = "https://github.com/VendrusSci/UnofficialFRBestiaryCompanion/releases/download/";
        public static string RemoteLauncherExePath = "https://github.com/VendrusSci/UnofficialFRBestiaryCompanion/releases/download/";


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
        
        public static string GetUserDirectory()
        {
            return Path.Combine(GetDataDirectory(), "User Data");
        }
    }
}
