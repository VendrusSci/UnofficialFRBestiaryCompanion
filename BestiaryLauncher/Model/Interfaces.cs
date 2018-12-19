using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Windows;

namespace BestiaryLauncher.Model
{
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
        void Create(string dirPath);
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
                client.Headers[HttpRequestHeader.UserAgent] = "UBCLauncher";
                try
                {
                    return client.DownloadData(url);
                }
                catch (WebException ex)
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
            Directory.Delete(dirPath, true);
        }

        public bool Exists(string dirPath)
        {
            return Directory.Exists(dirPath);
        }

        public void Move(string sourceDir, string destDir)
        {
            Directory.Move(sourceDir, destDir);
        }

        public void Create(string dirPath)
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    class FileUnzipper : IUnzipFiles
    {
        public string Unzip(string filePath)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            using (ZipArchive archive = ZipFile.OpenRead(filePath))
            {
                try
                {
                    archive.ExtractToDirectory(dirPath);
                }
                catch(System.IO.IOException)
                {
                    foreach(var file in archive.Entries)
                    {
                        var unzippedFile = Path.Combine(dirPath, file.Name);
                        var backupFile = unzippedFile + ".bak";
                        if(File.Exists(backupFile))
                        {
                            File.Delete(backupFile);
                        }
                        if (File.Exists(unzippedFile))
                        {
                            File.Move(unzippedFile, backupFile);
                        }
                    }
                    archive.ExtractToDirectory(dirPath);
                }
            }
            File.Delete(filePath);
            return dirPath;
        }
    }

    class ProcessStarter : IStartProcesses
    {
        public void Start(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = filePath,
                WorkingDirectory = Path.GetDirectoryName(filePath),
            };
            Process.Start(startInfo);
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
