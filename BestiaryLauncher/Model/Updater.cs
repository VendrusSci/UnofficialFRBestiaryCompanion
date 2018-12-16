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
            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteUbcVersionFile, ApplicationPaths.UbcVersionFile);
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteGitReleasePath, ApplicationPaths.LauncherVersionFile);
        }

        public void LaunchUbc()
        {
            var ubc = new Process();
            ubc.StartInfo.FileName = Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile);
            ubc.Start();
        }

        public bool LauncherUpdateAvailable()
        {
            return StatusChecks.IsVersionDifferent(VersionType.LauncherVersion, LatestLauncherVersion);
        }

        public bool FamiliarUpdateAvailable()
        {
            bool result = false;
            if(StatusChecks.IsVersionDifferent(VersionType.UbcVersion, LatestUbcVersion) != true)
            {
                result = StatusChecks.FamiliarUpdateAvailable(new FileLoader(), new FileDownloader());
            }
            return result;
        }

        public bool UbcUpdateAvailable()
        {
            bool result = false;
            if (StatusChecks.IsVersionDifferent(VersionType.UbcVersion, LatestUbcVersion) != true)
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
                "https://raw.githubusercontent.com/VendrusSci/UnofficialFRBestiaryCompanion/master/Bestiary/Resources/bestiary.png", 
                Path.Combine(ApplicationPaths.GetTempDirectory(), bestiaryImgName));
            //Executable
            result |= GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetDataDirectory(),ApplicationPaths.UbcExeFile),
                Path.Combine(LatestReleasePath, ApplicationPaths.UbcExeFile),
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.UbcExeFile));
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
            return GetFileAndOverwrite(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.LauncherExeFile),
                Path.Combine(LatestReleasePath, ApplicationPaths.LauncherExeFile),
                Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherExeFile));
        }

        public static bool UpdateVersionFile(VersionType software)
        {
            if (software == VersionType.UbcVersion)
            {
                return GetFileAndOverwrite(ApplicationPaths.GetUBCVersionPath(),
                    ApplicationPaths.RemoteUbcVersionFile,
                    Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.UbcVersionFile));
            }
            else if (software == VersionType.LauncherVersion)
            {
                return GetFileAndOverwrite(ApplicationPaths.GetLauncherVersionPath(),
                    ApplicationPaths.RemoteLauncherVersionFile,
                    Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherVersionFile));
            }
            else
            {
                return false;
            }
        }

        private static bool GetFileAndOverwrite(string localPath, string remotePath, string tempPath)
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
            if (GetHashOfFile(localPath) == GetHashOfFile(tempPath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool GetResourcesFolderAndOverwrite(string folderName)
        {
            return GetFolderAndOverwrite(Path.Combine(ApplicationPaths.GetResourcesDirectory(), folderName),
                Path.Combine(LatestReleasePath, folderName + ".zip"),
                Path.Combine(ApplicationPaths.GetTempDirectory(), folderName));
        }

        private static bool GetFolderAndOverwrite(string localDir, string remoteZip, string tempZip)
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
            if(GetHashOfFolder(tempDir) == GetHashOfFolder(localDir))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static string GetHashOfFile(string filePath)
        {
            using (var filestream = new FileStream(filePath, FileMode.Open))
            {
                var sha = SHA256.Create();
                return BitConverter.ToString(sha.ComputeHash(filestream)).ToLower();
            }
        }

        private static string GetHashOfFolder(string path)
        {
            var files = Directory.GetFiles(path).OrderBy(p => p).ToList();
            SHA256 sha = SHA256.Create();
            for(int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                //hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                sha.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                //hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                {
                    sha.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                }
                else
                {
                    sha.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                }
            }
            return BitConverter.ToString(sha.Hash).ToLower();
        }
    }
}
