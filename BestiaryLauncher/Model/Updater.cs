using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

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

            LatestReleasePath = ApplicationPaths.RemoteGitReleasePath + LatestUbcVersion;
            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(m_FileDownloader, ApplicationPaths.RemoteUbcVersionFile);
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(m_FileDownloader, ApplicationPaths.RemoteLauncherVersionFile);
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
            //Bestiary.png
            result &= GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.bestiaryImgFile),
                m_FileDownloader,
                ApplicationPaths.RemoteBestiaryImgPath,
                m_FileManipulator);
            //Executable
            result &= GetFileAndOverwrite(
                Path.Combine(ApplicationPaths.GetBestiaryDirectory(),ApplicationPaths.UbcExeFile),
                m_FileDownloader,
                Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile),
                m_FileManipulator);
            //DisplayIcons
            result &= GetResourcesFolderAndOverwrite("DisplayIcons");
            //ViewIcons
            result &= GetResourcesFolderAndOverwrite("ViewIcons");

            return result;
        }

        public bool UpdateFamiliars()
        {
            bool result = true;
            //Requires updating:
            //Icons
            result &= GetResourcesFolderAndOverwrite("Icons");
            //Images
            result &= GetResourcesFolderAndOverwrite("Images");
            //FamiliarData folder contents
            result &= GetResourcesFolderAndOverwrite("FamiliarData");

            return result;
        }

        public bool UpdateLauncher()
        {
            //Requires updating:
            //Launcher.exe
            //Rename own executable
            m_FileManipulator.Move(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile), "Backup_" + ApplicationPaths.LauncherExeFile);
            //Load in new executable
            return GetFileAndOverwrite(
                Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile),
                m_FileDownloader,
                Path.Combine(LatestReleasePath, ApplicationPaths.LauncherExeFile),
                m_FileManipulator
                );
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
        private bool GetFileAndOverwrite(string localPath, IDownloadFiles downloader, string remotePath, IManipulateFiles fileInterface)
        {
            var backupPath = localPath + m_FileBackup;
            if(fileInterface.Exists(backupPath))
            {
                fileInterface.Delete(backupPath);
            }
            fileInterface.Move(localPath, backupPath);
            string tempPath = downloader.DownloadToDirectory(remotePath, Path.GetDirectoryName(localPath), m_FileManipulator);
            if(tempPath == null)
            {
                fileInterface.Move(backupPath, localPath);
                return false;
            }
            return true;
        }

        
        private bool GetResourcesFolderAndOverwrite(string folderName)
        {
            return GetFolderAndOverwrite(
                m_FileDownloader,
                Path.Combine(LatestReleasePath, folderName + ".zip"),
                Path.Combine(ApplicationPaths.GetResourcesDirectory(), folderName),
                m_FileUnzipper,
                m_DirectoryManipulator
                );
        }

        private static string m_FolderBackup = "_Bkup";
        private bool GetFolderAndOverwrite(IDownloadFiles downloader, string remoteZip, string localDir, IUnzipFiles unzipper, IManipulateDirectories directoryInterface)
        {
            var zipFile = downloader.DownloadToDirectory(remoteZip, localDir, m_FileManipulator);
            if(zipFile != null)
            {
                var backupPath = localDir + m_FolderBackup;
                if(directoryInterface.Exists(backupPath))
                {
                    directoryInterface.Delete(backupPath);
                }
                directoryInterface.Move(localDir, backupPath);
                var folderPath = unzipper.Unzip(zipFile);
                return true;
            }
            return false;
        }
    }
}
