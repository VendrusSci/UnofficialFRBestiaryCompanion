using System.IO;
using System.Net;

namespace BestiaryLauncher.Model
{
    public static class StatusChecks
    {
        public static bool IsVersionDifferent(VersionType project, string version)
        {
            bool result = false;
            switch (project)
            {
                case VersionType.UbcVersion:
                    result = File.ReadAllText(ApplicationPaths.UbcVersionFile) == version;
                    break;
                case VersionType.LauncherVersion:
                    result = File.ReadAllText(ApplicationPaths.LauncherVersionFile) == version;
                    break;
            }
            return result;
        }

        public static bool SoftwareExists()
        {
            return File.Exists(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile));
        }

        public static bool FamiliarUpdateAvailable()
        {
            return DownloadAndCompare(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile),
                ApplicationPaths.RemoteFRDataFile,
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.FRDataFile));
        }

        public static bool UbcUpdateAvailable(string remotePath)
        {
            return DownloadAndCompare(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile),
                remotePath,
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.UbcExeFile));
        }

        public static bool LauncherUpdateAvailable(string remotePath)
        {
            return DownloadAndCompare(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile),
                remotePath,
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherExeFile));
        }

        public static bool DownloadAndCompare(string localPath, string remotePath, string tempPath)
        {
            bool result = false;
            if (!File.Exists(tempPath))
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(remotePath, tempPath);
                    }
                    catch (WebException ex)
                    {
                        //MainViewModel.UserActionLog.Error("Failed to download file");
                    }
                }

            }
            if (File.Exists(localPath) && File.Exists(tempPath))
            {
                if (File.ReadAllText(localPath) == File.ReadAllText(tempPath))
                {
                    result = true;
                }
            }
            return result;
        }

        public static string GetLatestVersionNumber(string remotePath, string fileName)
        {
            string versionNumber;
            string tempPath = Path.Combine(ApplicationPaths.GetTempDirectory(), fileName);
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
            using (WebClient client = new WebClient())
            {
                try
                {
                    client.DownloadFile(remotePath, tempPath);
                }
                catch (WebException ex)
                {
                    //MainViewModel.UserActionLog.Error("Failed to download file");
                }

            }
            if(File.Exists(tempPath))
            {
                versionNumber = File.ReadAllText(tempPath);
            }
            else
            {
                versionNumber = null;
            }
            return versionNumber;
        }
    }
}
