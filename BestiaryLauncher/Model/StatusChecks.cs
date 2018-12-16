using System.IO;
using System.Linq;
using System.Net;
using System.Text;

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

        public static bool FamiliarUpdateAvailable(ILoadFiles loader, IDownloadFiles downloader)
        {
            return DownloadAndCompare(
                loader,
                Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile),
                downloader,
                ApplicationPaths.RemoteFRDataFile
            );
        }

        public static bool UbcUpdateAvailable(ILoadFiles loader, IDownloadFiles downloader, string remotePath)
        {
            return DownloadAndCompare(
                loader,
                Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile),
                downloader,
                remotePath
            );
        }

        public static bool LauncherUpdateAvailable(ILoadFiles loader, IDownloadFiles downloader, string remotePath)
        {
            return DownloadAndCompare(
                loader,
                Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile),
                downloader,
                remotePath
            );
        }

        public static bool DownloadAndCompare(ILoadFiles loader, string localPath, IDownloadFiles downloader, string remoteUrl)
        {
            bool result = false;
            var localContents = loader.LoadAsString(localPath);
            var remoteContents = downloader.DownloadAsString(remoteUrl);
            
            if (localContents != null && remoteContents != null)
            {
                if (localContents.SequenceEqual(remoteContents))
                {
                    result = true;
                }
            }
            return result;
        }

        public static string GetLatestVersionNumber(string remotePath, string fileName)
        {
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
                    return File.ReadAllText(tempPath);
                }
                catch (WebException)
                {
                    //MainViewModel.UserActionLog.Error("Failed to download file");
                }

            }
            return null;
        }
    }

    public interface ILoadFiles
    {
        byte[] Load(string filepath);
    }

    public interface IDownloadFiles
    {
        byte[] Download(string url);
    }

    static class FileDownloaderExtensionMethods
    {
        public static string DownloadAsString(this IDownloadFiles downloader, string url)
        {
            var data = downloader.Download(url);
            if(data == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(data);
        }
    }
    
    static class FileLoaderExtensionMethods
    {
        public static string LoadAsString(this ILoadFiles loader, string filepath)
        {
            var data = loader.Load(filepath);
            if (data == null)
            {
                return null;
            }
            return Encoding.ASCII.GetString(data);
        }
    }

    class FileLoader : ILoadFiles
    {
        public byte[] Load(string filepath)
        {
            try
            {
                return File.ReadAllBytes(filepath);
            }
            catch(FileNotFoundException)
            {
            }
            catch(DirectoryNotFoundException)
            {
            }
            return null;
        }
    }

    class FileDownloader : IDownloadFiles
    {
        public byte[] Download(string url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadData(url);
                }
                catch (WebException)
                {
                    //MainViewModel.UserActionLog.Error("Failed to download file");
                }
            }

            return null;
        }
    }
}
