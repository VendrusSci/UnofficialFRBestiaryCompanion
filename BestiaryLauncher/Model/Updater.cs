using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;

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
            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteUbcVersionFile, ApplicationPaths.UbcVersionFile);
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteGitReleasePath, ApplicationPaths.LauncherVersionFile);
        }

        public void UpdateUbcSoftware()
        {
            //Requires updating:
            //Bestiary.png
            string bestiaryImgName = "bestiary.png";
            GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetResourcesDirectory(), bestiaryImgName), 
                "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/bestiary.png", 
                Path.Combine(ApplicationPaths.GetTempDirectory(), bestiaryImgName));
            //Executable
            GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetDataDirectory(),ApplicationPaths.ExeFile),
                Path.Combine(LatestReleasePath, ApplicationPaths.ExeFile),
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.ExeFile));
            //DisplayIcons
            GetResourcesFolderAndOverwrite("DisplayIcons");
            //ViewIcons
            GetResourcesFolderAndOverwrite("ViewIcons");
        }

        public void UpdateFamiliars()
        {
            //Requires updating:
            //Icons
            GetResourcesFolderAndOverwrite("Icons");
            //Images
            GetResourcesFolderAndOverwrite("Images");
            //FamiliarData folder contents
            GetResourcesFolderAndOverwrite("FamiliarData");
        }

        public void UpdateLauncher()
        {
            //Requires updating:
            //Launcher.exe
            //Rename own executable
            File.Move(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile), "Backup_" + ApplicationPaths.LauncherExeFile);
            //Load in new executable
            GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile),
                Path.Combine(LatestReleasePath, ApplicationPaths.LauncherExeFile),
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherExeFile));
        }

        public void UpdateVersionFile(VersionType software)
        {
            if (software == VersionType.UbcVersion)
            {
                GetFileAndOverwrite(ApplicationPaths.GetUBCVersionPath(),
                    ApplicationPaths.RemoteUbcVersionFile,
                    Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.UbcVersionFile));
            }
            else if (software == VersionType.LauncherVersion)
            {
                GetFileAndOverwrite(ApplicationPaths.GetLauncherVersionPath(),
                    ApplicationPaths.RemoteLauncherVersionFile,
                    Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherVersionFile));
            }
        }

        private void GetFileAndOverwrite(string localPath, string remotePath, string tempPath)
        {
            if(!File.Exists(tempPath))
            {
                using(WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(remotePath, tempPath);
                    }
                    catch(WebException ex)
                    {
                        //Debug text here
                    }
                }
            }
            File.Copy(tempPath, localPath, true);
        }

        private void GetResourcesFolderAndOverwrite(string folderName)
        {
            GetFolderAndOverwrite(Path.Combine(ApplicationPaths.GetResourcesDirectory(), folderName),
                Path.Combine(LatestReleasePath, folderName + ".zip"),
                Path.Combine(ApplicationPaths.GetTempDirectory(), folderName));
        }

        private void GetFolderAndOverwrite(string localDir, string remoteZip, string tempZip)
        {
            string tempDir = Path.GetDirectoryName(tempZip) + Path.GetFileNameWithoutExtension(tempZip);
            if(!File.Exists(tempZip))
            {
                using (WebClient client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(remoteZip, tempZip);
                    }
                    catch(WebException ex)
                    {
                        //Debug text here
                    }
                }
            }
            if(File.Exists(tempZip))
            {
                using (ZipArchive archive = ZipFile.OpenRead(tempZip))
                {
                    archive.ExtractToDirectory(tempDir);
                }
            }
            foreach(var file in Directory.GetFiles(localDir))
            {
                File.Delete(file);
            }
            foreach(var file in Directory.GetFiles(tempDir))
            {
                File.Copy(file, Path.Combine(localDir, file));
            }
        }

        private byte[] GetHashValue(string filePath)
        {
            using (var filestream = new FileStream(filePath, FileMode.Open))
            {
                var sha = SHA256.Create();
                return sha.ComputeHash(filestream);
            }
        }
    }
}
