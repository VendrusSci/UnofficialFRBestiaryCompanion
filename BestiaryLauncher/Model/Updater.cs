using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace BestiaryLauncher.Model
{
    public enum VersionType
    {
        LauncherVersion,
        UbcVersion
    }

    public class Updater
    {
        private string LatestUbcVersion;
        private string LatestLauncherVersion;
        private string LatestReleasePath;

        private ILoadFiles m_FileLoader;
        private IDownloadFiles m_FileDownloader;
        private IUnzipFiles m_FileUnzipper;
        private IManipulateFiles m_FileManipulator;
        private IManipulateDirectories m_DirectoryManipulator;
        private IStartProcesses m_ProcessStarter;

        public Updater(ILoadFiles fileLoader, IDownloadFiles fileDownloader, IUnzipFiles fileUnzipper, 
            IManipulateFiles fileManipulator, IManipulateDirectories directoryManipulator, IStartProcesses processStarter)
        {
            m_FileLoader = fileLoader;
            m_FileDownloader = fileDownloader;
            m_FileUnzipper = fileUnzipper;
            m_FileManipulator = fileManipulator;
            m_DirectoryManipulator = directoryManipulator;
            m_ProcessStarter = processStarter;

            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(m_FileDownloader, ApplicationPaths.RemoteUbcVersionFile);
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(m_FileDownloader, ApplicationPaths.RemoteLauncherVersionFile);
            LatestReleasePath = ApplicationPaths.RemoteGitReleasePath + FetchLatestReleaseNumber();
        }

        public void LaunchUbc()
        {
            m_ProcessStarter.Start(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile));
        }

        public bool SoftwareUpdateAvailable()
        {
            return StatusChecks.IsVersionDifferent(m_FileLoader, VersionType.UbcVersion, LatestUbcVersion);
        }

        public bool LauncherUpdateAvailable()
        {
            return StatusChecks.IsVersionDifferent(m_FileLoader, VersionType.LauncherVersion, LatestLauncherVersion);
        }

        public bool FamiliarUpdateAvailable()
        {
             return StatusChecks.FamiliarUpdateAvailable(m_FileLoader, m_FileDownloader);
        }

        public bool UbcUpdateAvailable()
        {
            return StatusChecks.UbcUpdateAvailable(m_FileLoader, m_FileDownloader, Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile));
        }

        public bool UpdateUbcSoftware()
        {
            bool result = true;
            //Requires updating:
            //Executable
            result &= GetFileAndOverwrite(
                Path.Combine(ApplicationPaths.GetBestiaryDirectory(),ApplicationPaths.UbcExeFile),
                m_FileDownloader,
                Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile),
                m_FileManipulator);
            //DisplayIcons
            result &= GetBestiaryResourcesFolderAndOverwrite("DisplayIcons");
            //ViewIcons
            result &= GetBestiaryResourcesFolderAndOverwrite("ViewIcons");

            return result;
        }

        public bool UpdateFamiliars()
        {
            bool result = true;
            //Requires updating:
            //Icons
            result &= GetBestiaryResourcesFolderAndOverwrite("Icons");
            //Images
            result &= GetBestiaryResourcesFolderAndOverwrite("Images");
            //FamiliarData folder contents
            result &= GetBestiaryResourcesFolderAndOverwrite("FamiliarData");

            return result;
        }

        public bool UpdateLauncher()
        {
            bool result = true;
            //Requires updating:
            //Launcher.exe
            //Rename own executable
            m_FileManipulator.Move(Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile), "Backup_" + ApplicationPaths.LauncherExeFile);
            //Load in new executable
            result &= GetFileAndOverwrite(
                Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile),
                m_FileDownloader,
                Path.Combine(LatestReleasePath, ApplicationPaths.LauncherExeFile),
                m_FileManipulator
                );
            result &= GetFolderAndOverwrite(
                ApplicationPaths.GetLauncherImagesDirectory(),
                Path.Combine(LatestReleasePath, Path.GetFileName(ApplicationPaths.GetLauncherImagesDirectory()) + ".zip")
                );
            return result;
        }

        public bool UpdateVersionFile(VersionType software)
        {
            if (software == VersionType.UbcVersion)
            {
                return GetFileAndOverwrite(
                    ApplicationPaths.GetUBCVersionPath(),
                    m_FileDownloader,
                    ApplicationPaths.RemoteUbcVersionFile,
                    m_FileManipulator
                    );
            }
            else if (software == VersionType.LauncherVersion)
            {
                return GetFileAndOverwrite(
                    ApplicationPaths.GetLauncherVersionPath(),
                    m_FileDownloader,
                    ApplicationPaths.RemoteLauncherVersionFile,
                    m_FileManipulator
                    );
            }
            else
            {
                return false;
            }
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

        private string FetchLatestReleaseNumber()
        {
            string releaseInfo = m_FileDownloader.DownloadAsString(ApplicationPaths.RemoteGitReleaseInfoPath);
            dynamic releaseObject = JsonConvert.DeserializeObject(releaseInfo);
            return releaseObject.tag_name;
        }
    }
}
