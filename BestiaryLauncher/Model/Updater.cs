using System.ComponentModel;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using BestiaryLauncher.ViewModels;

namespace BestiaryLauncher.Model
{
    public enum VersionType
    {
        LauncherVersion,
        UbcVersion
    }

    public class Updater : INotifyPropertyChanged
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
            m_ProcessStarter.Start(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExe));
        }

        public bool SoftwareUpdateAvailable()
        {
            return IsVersionDifferent(LatestVersionNumber);
        }

        public bool LauncherUpdateAvailable()
        {
            return (m_LatestManifestData.UBCLauncherZip != m_LocalManifestData.UBCLauncherZip) 
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
            return (m_LatestManifestData.UnofficialBestiaryCompanionZip != m_LocalManifestData.UnofficialBestiaryCompanionZip)
                || (m_LatestManifestData.DisplayIconsZip != m_LocalManifestData.DisplayIconsZip)
                || (m_LatestManifestData.ViewIconsZip != m_LocalManifestData.ViewIconsZip);
        }

        public bool UpdateUbcSoftware()
        {
            bool result = true;
            //Executable
            if(m_LatestManifestData.UnofficialBestiaryCompanionZip != m_LocalManifestData.UnofficialBestiaryCompanionZip)
            {
                MainViewModel.UserActionLog.Info("UBC zip update available");
                var interimResult = GetFolderAndOverwrite(
                    Path.Combine(ApplicationPaths.GetBestiaryDirectory(),ApplicationPaths.UbcZip),
                    Path.Combine(LatestReleasePath, ApplicationPaths.UbcZip));
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("UBC zip update successful");
                    m_LocalManifestData.UnofficialBestiaryCompanionZip = m_LatestManifestData.UnofficialBestiaryCompanionZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("UBC zip update failed");
                }
                result &= interimResult;
            }
            //DisplayIcons
            if(m_LatestManifestData.DisplayIconsZip != m_LocalManifestData.DisplayIconsZip)
            {
                MainViewModel.UserActionLog.Info("DisplayIcons zip update available");
                var interimResult = GetBestiaryResourcesFolderAndOverwrite("DisplayIcons");
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("DisplayIcons zip update successful");
                    m_LocalManifestData.DisplayIconsZip = m_LatestManifestData.DisplayIconsZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("DisplayIcons zip update failed");
                }
                result &= interimResult;
            }
            //ViewIcons
            if(m_LatestManifestData.ViewIconsZip != m_LocalManifestData.ViewIconsZip)
            {
                MainViewModel.UserActionLog.Info("ViewIcons zip update available");
                var interimResult = GetBestiaryResourcesFolderAndOverwrite("ViewIcons");
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("ViewIcons zip update successful");
                    m_LocalManifestData.ViewIconsZip = m_LatestManifestData.ViewIconsZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("ViewIcons zip update failed");
                }
                result &= interimResult;
            }
            return result;
        }

        public bool UpdateFamiliars()
        {
            bool result = true;
            //Icons
            if(m_LatestManifestData.IconsZip != m_LocalManifestData.IconsZip)
            {
                MainViewModel.UserActionLog.Info("Icons zip update available");
                var interimResult = GetBestiaryResourcesFolderAndOverwrite("Icons");
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("Icons zip update successful");
                    m_LocalManifestData.IconsZip = m_LatestManifestData.IconsZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("Icons zip update failed");
                }
                result &= interimResult;
            }
            //Images
            if(m_LatestManifestData.ImagesZip != m_LocalManifestData.ImagesZip)
            {
                MainViewModel.UserActionLog.Info("Images zip update available");
                var interimResult = GetBestiaryResourcesFolderAndOverwrite("Images");
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("Images zip update successful");
                    m_LocalManifestData.ImagesZip = m_LatestManifestData.ImagesZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("Images zip update failed");
                }
                result &= interimResult;
            }
            //FamiliarData folder contents
            if(m_LatestManifestData.FamiliarDataZip != m_LocalManifestData.FamiliarDataZip)
            {
                MainViewModel.UserActionLog.Info("Familiar data update available");
                var interimResult = GetBestiaryResourcesFolderAndOverwrite("FamiliarData");
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("Familiar data update successful");
                    m_LocalManifestData.FamiliarDataZip = m_LatestManifestData.FamiliarDataZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("Familiar data update failed");
                }
                result &= interimResult;
            }
            return result;
        }

        public bool UpdateLauncher()
        {
            bool result = true;
            //Launcher.exe
            if (m_LatestManifestData.UBCLauncherZip != m_LocalManifestData.UBCLauncherZip)
            {
                MainViewModel.UserActionLog.Info("Launcher zip update available");
                //Rename own executable
                MainViewModel.UserActionLog.Info("Renaming executable");
                var exePath = Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExe);
                var backupExePath = Path.Combine(ApplicationPaths.GetLauncherDirectory(), "bkup_" + ApplicationPaths.LauncherExe);
                if (m_FileManipulator.Exists(backupExePath))
                {
                    MainViewModel.UserActionLog.Info("Deleting old backup");
                    m_FileManipulator.Delete(backupExePath);
                }
                MainViewModel.UserActionLog.Info("Creating backup");
                m_FileManipulator.Move(exePath, backupExePath);
                //Load in new executable
                var interimResult = GetFolderAndOverwrite(
                    Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherZip),
                    Path.Combine(LatestReleasePath, ApplicationPaths.LauncherZip));
                if(interimResult)
                {
                    MainViewModel.UserActionLog.Info("Launcher zip update successful");
                    m_LocalManifestData.UBCLauncherZip = m_LatestManifestData.UBCLauncherZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("Launcher zip update failed");
                }
                result &= interimResult;
            }
            if (m_LatestManifestData.LauncherImagesZip != m_LocalManifestData.LauncherImagesZip)
            {
                MainViewModel.UserActionLog.Info("Launcher images zip update available");
                var interimResult = GetFolderAndOverwrite(
                    ApplicationPaths.GetLauncherImagesDirectory(),
                    Path.Combine(LatestReleasePath, Path.GetFileName(ApplicationPaths.GetLauncherImagesDirectory()) + ".zip")
                    );
                if (interimResult)
                {
                    MainViewModel.UserActionLog.Info("Launcher images zip update successful");
                    m_LocalManifestData.LauncherImagesZip = m_LatestManifestData.LauncherImagesZip;
                    Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
                }
                else
                {
                    MainViewModel.UserActionLog.Error("Launcher images zip update failed");
                }
                result &= interimResult;
            }
            return result;
        }

        public bool UpdateVersion()
        {
            m_LocalManifestData.Version = LatestVersionNumber;
            Manifest.UpdateManifest(m_FileManipulator, m_LocalManifestData);
            return true;
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

        public bool IsVersionDifferent(string version)
        {
            var localVersion = m_LocalManifestData.Version; //m_FileLoader.LoadAsString(ApplicationPaths.GetVersionPath());
            return localVersion != version;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
