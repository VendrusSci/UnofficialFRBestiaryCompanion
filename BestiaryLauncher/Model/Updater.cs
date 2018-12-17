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

    class Updater
    {
        private string LatestUbcVersion;
        private string LatestLauncherVersion;
        private string LatestReleasePath;

        public Updater()
        {
            Directory.CreateDirectory(ApplicationPaths.GetTempDirectory());
            LatestReleasePath = ApplicationPaths.RemoteGitReleasePath + LatestUbcVersion;
            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(new FileDownloader(), ApplicationPaths.RemoteUbcVersionFile);
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(new FileDownloader(), ApplicationPaths.RemoteGitReleasePath);
        }

        public void LaunchUbc()
        {
            var ubc = new Process();
            ubc.StartInfo.FileName = Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile);
            ubc.Start();
        }

        public bool LauncherUpdateAvailable()
        {
            return StatusChecks.IsVersionDifferent(new FileLoader(), VersionType.LauncherVersion, LatestLauncherVersion);
        }

        public bool FamiliarUpdateAvailable()
        {
            bool result = false;
            if(StatusChecks.IsVersionDifferent(new FileLoader(), VersionType.UbcVersion, LatestUbcVersion) != true)
            {
                result = StatusChecks.FamiliarUpdateAvailable(new FileLoader(), new FileDownloader());
            }
            return result;
        }

        public bool UbcUpdateAvailable()
        {
            bool result = false;
            if (StatusChecks.IsVersionDifferent(new FileLoader(), VersionType.UbcVersion, LatestUbcVersion) != true)
            {
                result = StatusChecks.UbcUpdateAvailable(new FileLoader(), new FileDownloader(), Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile));
            }
            return result;
        }

        public bool UpdateUbcSoftware()
        {
            bool result = true;
            //Requires updating:
            //Bestiary.png
            string bestiaryImgName = "bestiary.png";
            result |= GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetResourcesDirectory(), bestiaryImgName),
                new FileDownloader(),
                ApplicationPaths.RemoteBestiaryImgPath);
            //Executable
            result |= GetFileAndOverwrite(
                Path.Combine(ApplicationPaths.GetDataDirectory(),ApplicationPaths.UbcExeFile),
                new FileDownloader(),
                Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile));
            //DisplayIcons
            result |= GetResourcesFolderAndOverwrite("DisplayIcons");
            //ViewIcons
            result |= GetResourcesFolderAndOverwrite("ViewIcons");

            return result;
        }

        public bool UpdateFamiliars()
        {
            bool result = true;
            //Requires updating:
            //Icons
            result |= GetResourcesFolderAndOverwrite("Icons");
            //Images
            result |= GetResourcesFolderAndOverwrite("Images");
            //FamiliarData folder contents
            result |= GetResourcesFolderAndOverwrite("FamiliarData");

            return result;
        }

        public bool UpdateLauncher()
        {
            //Requires updating:
            //Launcher.exe
            //Rename own executable
            File.Move(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile), "Backup_" + ApplicationPaths.LauncherExeFile);
            //Load in new executable
            return GetFileAndOverwrite(
                Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile),
                new FileDownloader(),
                Path.Combine(LatestReleasePath, ApplicationPaths.LauncherExeFile)
                );
        }

        public static bool UpdateVersionFile(VersionType software)
        {
            if (software == VersionType.UbcVersion)
            {
                return GetFileAndOverwrite(
                    ApplicationPaths.GetUBCVersionPath(),
                    new FileDownloader(),
                    ApplicationPaths.RemoteUbcVersionFile
                    );
            }
            else if (software == VersionType.LauncherVersion)
            {
                return GetFileAndOverwrite(
                    ApplicationPaths.GetLauncherVersionPath(),
                    new FileDownloader(),
                    ApplicationPaths.RemoteLauncherVersionFile
                    );
            }
            else
            {
                return false;
            }
        }

        private static string m_FileBackup = ".bak";
        private static bool GetFileAndOverwrite(string localPath, IDownloadFiles downloader, string remotePath)
        {
            var backupPath = localPath + m_FileBackup;
            if(File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }
            File.Move(localPath, backupPath);
            string tempPath = downloader.DownloadToDirectory(remotePath, localPath);
            if(tempPath == null)
            {
                File.Move(backupPath, localPath);
                return false;
            }
            return true;
        }

        
        private bool GetResourcesFolderAndOverwrite(string folderName)
        {
            return GetFolderAndOverwrite(
                new FileDownloader(),
                Path.Combine(LatestReleasePath, folderName + ".zip"),
                Path.Combine(ApplicationPaths.GetResourcesDirectory(), folderName),
                new FileUnzipper()
                );
        }

        private static string m_FolderBackup = "_Bkup";
        private static bool GetFolderAndOverwrite(IDownloadFiles downloader, string remoteZip, string localDir, IUnzipFiles unzipper)
        {
            var zipFile = downloader.DownloadToDirectory(remoteZip, localDir);
            if(zipFile != null)
            {
                var backupPath = localDir + m_FolderBackup;
                if(Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath);
                }
                Directory.Move(localDir, backupPath);
                var folderPath = unzipper.Unzip(zipFile);
                return true;
            }
            return false;
        }
    }
}
