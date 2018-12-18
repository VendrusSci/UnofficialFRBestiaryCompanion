using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestiaryLauncher.Model
{
    public static class ApplicationPaths
    {
        public static string LauncherVersionFile = "launcherversion.txt";
        public static string UbcVersionFile = "version.txt";
        public static string FRDataFile = "FRData.xml";
        public static string UbcExeFile = "Bestiary.exe";
        public static string LauncherExeFile = "BestiaryLauncher.exe";

        public static string RemoteLauncherVersionFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/BestiaryLauncher/launcherversion.txt";
        public static string RemoteUbcVersionFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/version.txt";
        public static string RemoteFRDataFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/FRData.xml";
        public static string RemoteGitReleasePath = "https://github.com/VendrusSci/UnofficialFRBestiaryCompanion/releases/download/";
        public static string RemoteGitReleaseInfoPath = "https://api.github.com/repos/VendrusSci/UnofficialFRBestiaryCompanion/releases/latest";

        public static string GetLauncherDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public static string GetLauncherResourcesDirectory()
        {
            return Path.Combine(GetLauncherDirectory(), "Resources");
        }

        public static string GetLauncherImagesDirectory()
        {
            return Path.Combine(GetLauncherResourcesDirectory(), "LauncherImages");
        }

        public static string GetBestiaryDirectory()
        {
            return Path.Combine(GetLauncherDirectory(), "Unofficial Bestiary Companion");
        }

        public static string GetBestiaryResourcesDirectory()
        {
            return Path.Combine(GetBestiaryDirectory(), "Resources");
        }

        public static string GetBestiaryUserDataDirectory()
        {
            return Path.Combine(GetBestiaryDirectory(), "User Data");
        }

        public static string GetLauncherVersionPath()
        {
            return Path.Combine(GetLauncherResourcesDirectory(), LauncherVersionFile);
        }

        public static string GetUBCVersionPath()
        {
            return Path.Combine(GetBestiaryResourcesDirectory(), UbcVersionFile);
        }
    }
}
