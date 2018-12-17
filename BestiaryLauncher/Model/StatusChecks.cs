﻿using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

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
                if (!localContents.SequenceEqual(remoteContents))
                {
                    result = true;
                }
            }
            return result;
        }

        public static string GetLatestVersionNumber(IDownloadFiles downloader, string remotePath
            )
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

    public interface IDeleteFiles
    {
        void Delete(string filePath);
    }

    public interface IUnzipFiles
    {
        string Unzip(string filePath);
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

        public static string DownloadToDirectory(this IDownloadFiles downloader, string url, string directoryPath)
        {
            var fileName = Path.Combine(directoryPath, Path.GetFileName(url));
            var data = downloader.Download(url);
            if (data != null)
            {
                File.WriteAllBytes(Path.Combine(directoryPath, Path.GetFileName(url)), data);
                return fileName;
            }
            return null;
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

    class FileDeleter : IDeleteFiles
    {
        public void Delete(string filePath)
        {
            File.Delete(filePath);
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
}
