using System.IO;

namespace BestiaryLauncher.Model
{
    public static class ApplicationPaths
    {
        public static string UbcZip = "UnofficialBestiaryCompanion.zip";
        public static string UbcExe = "Bestiary.exe";
        public static string LauncherZip = "UBCLauncher.zip";
        public static string LauncherExe = "BestiaryLauncher.exe";
        public static string HeaderImage = "UBC.png";

        public static string RemoteFRDataFile = "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/FRData.xml";
        public static string RemoteGitReleasePath = "https://github.com/VendrusSci/UnofficialFRBestiaryCompanion/releases/download/";
        //public static string RemoteGitReleaseInfoPath = "https://api.github.com/repos/VendrusSci/UnofficialFRBestiaryCompanion/releases/latest";
        public static string RemoteGitReleaseInfoPath = "https://api.github.com/repos/VendrusSci/UnofficialFRBestiaryCompanion/releases";

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

        public static string GetHeaderImagePath()
        {
            return Path.Combine(GetBestiaryResourcesDirectory(), "DisplayIcons", HeaderImage);
        }
    }
}
