using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace BestiaryLauncher.Model
{
    public static class StatusChecks
    {
        public static bool IsVersionDifferent(ILoadFiles loader, VersionType project, string version)
        {
            bool result = false;
            switch (project)
            {
                case VersionType.UbcVersion:
                    result = loader.LoadAsString(ApplicationPaths.UbcVersionFile) != version;
                    break;
                case VersionType.LauncherVersion:
                    result = loader.LoadAsString(ApplicationPaths.LauncherVersionFile) != version;
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

        private static bool DownloadAndCompare(ILoadFiles loader, string localPath, IDownloadFiles downloader, string remoteUrl)
        {
            bool result = false;
            var localContents = loader.LoadAsString(localPath);
            var remoteContents = downloader.DownloadAsString(remoteUrl);
            
            if (localContents != null && remoteContents != null)
            {
                if (localContents != remoteContents)
                {
                    result = true;
                }
            }
            return result;
        }

        public static string GetLatestVersionNumber(IDownloadFiles downloader, string remotePath)
        {
            return downloader.DownloadAsString(remotePath);
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

    public interface IManipulateFiles
    {
        void Delete(string filePath);
        bool Exists(string filePath);
        void Move(string sourceFilepath, string destFilepath);
        void WriteAllBytes(string filePath, byte[] data);
    }

    public interface IManipulateDirectories
    {
        void Delete(string dirPath);
        bool Exists(string dirPath);
        void Move(string sourceDir, string destDir);
    }

    public interface IUnzipFiles
    {
        string Unzip(string filePath);
    }

    public interface IStartProcesses
    {
        void Start(string filePath);
    }

    public interface ICloseApplications
    {
        void Close();
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

        public static string DownloadToDirectory(this IDownloadFiles downloader, string url, string directoryPath, IManipulateFiles fileManipulator)
        {
            var fileName = Path.Combine(directoryPath, Path.GetFileName(url));
            var data = downloader.Download(url);
            if (data != null)
            {
                fileManipulator.WriteAllBytes(Path.Combine(directoryPath, Path.GetFileName(url)), data);
                return fileName;
            }
            return null;
        }
    }
    
    public static class FileLoaderExtensionMethods
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

    class FileManipulator : IManipulateFiles
    {
        public void Delete(string filePath)
        {
            File.Delete(filePath);
        }

        public bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public void Move(string sourceFilepath, string destFilepath)
        {
            File.Move(sourceFilepath, destFilepath);
        }

        public void WriteAllBytes(string filePath, byte[] data)
        {
            File.WriteAllBytes(filePath, data);
        }
    }

    class DirectoryManipulator : IManipulateDirectories
    {
        public void Delete(string dirPath)
        {
            Directory.Delete(dirPath);
        }

        public bool Exists(string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        public void Move(string sourceDir, string destDir)
        {
            Directory.Move(sourceDir, destDir);
        }
    }

    class FileUnzipper : IUnzipFiles
    {
        public string Unzip(string filePath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                var dirPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                archive.ExtractToDirectory(dirPath);
                return dirPath;
            }
        }
    }

    class ProcessStarter : IStartProcesses
    {
        public void Start(string filePath)
        {
            Process.Start(filePath);
        }
    }

    class ApplicationCloser : ICloseApplications
    {
        public void Close()
        {
            Application.Current.Shutdown();
        }
    }
}
