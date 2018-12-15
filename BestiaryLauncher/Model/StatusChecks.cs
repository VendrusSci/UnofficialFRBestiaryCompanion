using System.IO;
using System.Net;

namespace BestiaryLauncher.Model
{
    public static class StatusChecks
    {
        public static bool IsVersionDifferent(VersionType project, string ubcVersion, string launcherVersion)
        {
            bool result = false;
            switch (project)
            {
                case VersionType.SoftwareVersion:
                    result = File.ReadAllText(ApplicationPaths.UbcVersionFile) == ubcVersion;
                    break;
                case VersionType.LauncherVersion:
                    result = File.ReadAllText(ApplicationPaths.LauncherVersionFile) == launcherVersion;
                    break;
            }
            return result;
        }

        public static bool SoftwareExists()
        {
            return File.Exists(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.ExeFile));
        }

        public static bool FamiliarUpdateAvailable()
        {
            return DownloadAndCompare(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile),
                ApplicationPaths.RemoteFRDataFile,
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.FRDataFile));
        }

        public static bool SoftwareUpdateAvailable(string remotePath)
        {
            return DownloadAndCompare(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.ExeFile),
                remotePath,
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.ExeFile));
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
                if (File.Exists(localPath) && File.Exists(tempPath))
                {
                    if (File.ReadAllText(localPath) == File.ReadAllText(tempPath))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        public static string GetLatestVersionNumber(string remotePath, string fileName)
        {
            string versionNumber;
            string tempPath = Path.Combine(ApplicationPaths.GetTempDirectory(), fileName);
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
                if (File.Exists(tempPath))
                {
                    versionNumber = File.ReadAllText(tempPath);
                }
                else
                {
                    versionNumber = null;
                }
            }
            File.Delete(tempPath);
            return versionNumber;
        }
    }
}
