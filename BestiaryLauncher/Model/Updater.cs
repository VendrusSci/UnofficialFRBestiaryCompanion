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
        private string RemoteExeFullPath;
        private string RemoteLauncherExeFullPath;
        private string LatestUbcVersion;
        private string LatestLauncherVersion;
        public Updater()
        {
            Directory.CreateDirectory(ApplicationPaths.GetTempDirectory());
            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteUbcVersionFile, ApplicationPaths.UbcVersionFile);
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteLauncherExePath, ApplicationPaths.LauncherVersionFile);
            RemoteExeFullPath = Path.Combine(ApplicationPaths.RemoteExePath, LatestUbcVersion, ApplicationPaths.ExeFile);
            RemoteLauncherExeFullPath = Path.Combine(ApplicationPaths.RemoteLauncherExePath, LatestLauncherVersion, ApplicationPaths.LauncherExeFile);
        }

        public void UpdateUbcSoftware()
        {
            //requires updating:
            //executable
            //DisplayIcons
            //ViewIcons
            
            //Bestiary.png
            
        }

        public void UpdateVersionFile(VersionType software)
        {
            if(software == VersionType.UbcVersion)
            {

            }
            else if(software == VersionType.LauncherVersion)
            {

            }
        }

        public void UpdateFamiliars()
        {

            //Requires updating:
            //Icons
            //Images
            //FamiliarData folder contents
        }

        public void UpdateLauncher()
        {

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
