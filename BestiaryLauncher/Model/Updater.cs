using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BestiaryLauncher.Model
{
    public enum VersionType
    {
        LauncherVersion,
        UbcVersion
    }

    public class Updater
    {
        private string LatestReleasePath;
        private string LatestVersionNumber;

        private ILoadFiles m_FileLoader;
        private IDownloadFiles m_FileDownloader;
        private IUnzipFiles m_FileUnzipper;
        private IManipulateFiles m_FileManipulator;
        private IManipulateDirectories m_DirectoryManipulator;
        private IStartProcesses m_ProcessStarter;

        private ManifestData m_LatestManifestData;
        private ManifestData m_LocalManifestData;

        public Updater(ILoadFiles fileLoader, IDownloadFiles fileDownloader, IUnzipFiles fileUnzipper, 
            IManipulateFiles fileManipulator, IManipulateDirectories directoryManipulator, IStartProcesses processStarter)
        {
            m_FileLoader = fileLoader;
            m_FileDownloader = fileDownloader;
            m_FileUnzipper = fileUnzipper;
            m_FileManipulator = fileManipulator;
            m_DirectoryManipulator = directoryManipulator;
            m_ProcessStarter = processStarter;

            LatestVersionNumber = FetchLatestVersion();
            LatestReleasePath = Path.Combine(ApplicationPaths.RemoteGitReleasePath, LatestVersionNumber);

            m_LatestManifestData = new Manifest(m_FileManipulator)
                .FetchLatest(m_FileDownloader, Path.Combine(LatestReleasePath, "manifest.txt"));
            m_LocalManifestData = new Manifest(m_FileManipulator)
                .FetchLocal(m_FileLoader, Path.Combine(ApplicationPaths.GetLauncherResourcesDirectory(), "manifest.txt"));
        }

        public void LaunchUbc()
        {
            m_ProcessStarter.Start(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile));
        }

        public bool SoftwareUpdateAvailable()
        {
            return StatusChecks.IsVersionDifferent(m_FileLoader, LatestVersionNumber);
        }

        public bool LauncherUpdateAvailable()
        {
            return (m_LatestManifestData.LauncherExe != m_LocalManifestData.LauncherExe) 
                || (m_LatestManifestData.LauncherImagesZip != m_LocalManifestData.LauncherImagesZip);
        }

        public bool FamiliarUpdateAvailable()
        {
            return (m_LatestManifestData.FamiliarDataZip != m_LocalManifestData.FamiliarDataZip)
                || (m_LatestManifestData.IconsZip != m_LocalManifestData.IconsZip)
                || (m_LatestManifestData.ImagesZip != m_LocalManifestData.ImagesZip);
        }

        public bool UbcUpdateAvailable()
        {
            return (m_LatestManifestData.BestiaryExe != m_LocalManifestData.BestiaryExe)
                || (m_LatestManifestData.DisplayIconsZip != m_LocalManifestData.DisplayIconsZip)
                || (m_LatestManifestData.ViewIconsZip != m_LocalManifestData.ViewIconsZip);
        }

        public bool UpdateUbcSoftware()
        {
            bool result = true;
            //Executable
            if(m_LatestManifestData.BestiaryExe != m_LocalManifestData.BestiaryExe)
            {
                result &= GetFileAndOverwrite(
                    Path.Combine(ApplicationPaths.GetBestiaryDirectory(),ApplicationPaths.UbcExeFile),
                    m_FileDownloader,
                    Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile),
                    m_FileManipulator);
            }
            //DisplayIcons
            if(m_LatestManifestData.DisplayIconsZip != m_LocalManifestData.DisplayIconsZip)
            {
                result &= GetBestiaryResourcesFolderAndOverwrite("DisplayIcons");
            }
            //ViewIcons
            if(m_LatestManifestData.ViewIconsZip != m_LocalManifestData.ViewIconsZip)
            {
                result &= GetBestiaryResourcesFolderAndOverwrite("ViewIcons");
            }
            return result;
        }

        public bool UpdateFamiliars()
        {
            bool result = true;
            //Icons
            if(m_LatestManifestData.IconsZip != m_LocalManifestData.IconsZip)
            {
                result &= GetBestiaryResourcesFolderAndOverwrite("Icons");
            }
            //Images
            if(m_LatestManifestData.ImagesZip != m_LocalManifestData.ImagesZip)
            {
                result &= GetBestiaryResourcesFolderAndOverwrite("Images");
            }
            //FamiliarData folder contents
            if(m_LatestManifestData.FamiliarDataZip != m_LocalManifestData.FamiliarDataZip)
            {
                result &= GetBestiaryResourcesFolderAndOverwrite("FamiliarData");
            }
            return result;
        }

        public bool UpdateLauncher()
        {
            bool result = true;
            //Launcher.exe
            if (m_LatestManifestData.LauncherExe != m_LocalManifestData.LauncherExe)
            {
                //Rename own executable
                var exePath = Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile);
                var backupExePath = Path.Combine(ApplicationPaths.GetLauncherDirectory(), "bkup_" + ApplicationPaths.LauncherExeFile);
                if (m_FileManipulator.Exists(backupExePath))
                {
                    m_FileManipulator.Delete(backupExePath);
                }
                m_FileManipulator.Move(exePath, backupExePath);
                //Load in new executable
                result &= GetFileAndOverwrite(
                    Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile),
                    m_FileDownloader,
                    Path.Combine(LatestReleasePath, ApplicationPaths.LauncherExeFile),
                    m_FileManipulator
                    );
            }
            if (m_LatestManifestData.LauncherExe != m_LocalManifestData.LauncherExe)
            {
                result &= GetFolderAndOverwrite(
                    ApplicationPaths.GetLauncherImagesDirectory(),
                    Path.Combine(LatestReleasePath, Path.GetFileName(ApplicationPaths.GetLauncherImagesDirectory()) + ".zip")
                    );
            }
            return result;
        }

        public bool UpdateVersionFile()
        {
            return false;
        }

        private static string m_FileBackup = ".bak";
        private bool GetFileAndOverwrite(string localPath, IDownloadFiles downloader, string remotePath, IManipulateFiles fileManipulator)
        {
            var backupPath = localPath + m_FileBackup;
            //Delete any backups that exist
            if(fileManipulator.Exists(backupPath))
            {
                fileManipulator.Delete(backupPath);
            }
            //Backup the existing file if it has already been installed
            if(fileManipulator.Exists(localPath))
            {
                fileManipulator.Move(localPath, backupPath);
            }
            
            string tempPath = downloader.DownloadToDirectory(remotePath, Path.GetDirectoryName(localPath), m_FileManipulator);
            if(tempPath == null)
            {
                if (fileManipulator.Exists(backupPath))
                {
                    fileManipulator.Move(backupPath, localPath);
                }
                return false;
            }
            return true;
        }

        private bool GetBestiaryResourcesFolderAndOverwrite(string folderName)
        {
            return GetFolderAndOverwrite(
                Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), folderName),
                Path.Combine(LatestReleasePath, folderName + ".zip")
                );
        }

        private static string m_FolderBackup = "_Bkup";
        private bool GetFolderAndOverwrite(string localDir, string remoteZip)
        {
            var zipFile = m_FileDownloader.DownloadToDirectory(remoteZip, Path.GetDirectoryName(localDir), m_FileManipulator);
            if(zipFile != null)
            {
                var backupPath = localDir + m_FolderBackup;
                //If backup exists, delete it
                if(m_DirectoryManipulator.Exists(backupPath))
                {
                    m_DirectoryManipulator.Delete(backupPath);
                }
                //if current directory exists, make it a backup
                if(m_DirectoryManipulator.Exists(localDir))
                {
                    m_DirectoryManipulator.Move(localDir, backupPath);
                }
                
                var folderPath = m_FileUnzipper.Unzip(zipFile);
                return true;
            }
            return false;
        }

        private string FetchLatestVersion()
        {
            string releaseInfoJsonText = m_FileDownloader.DownloadAsString(ApplicationPaths.RemoteGitReleaseInfoPath);
            var releaseInfoJsonObject = JObject.Parse(releaseInfoJsonText);
            if(releaseInfoJsonObject.TryGetValue("tag_name", out var tagName))
            {
                if (tagName.Type == JTokenType.String)
                {
                    return tagName.Value<string>();
                }
            }
            return null;
        }
    }
}
