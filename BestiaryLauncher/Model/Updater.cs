using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
            
        }

        public void UpdateFamiliars()
        {
            //Requires updating:
            //Icons
            //Images
            //FRData.xml
            //Venues.txt
            //Events.txt
        }

        public void UpdateLauncher()
        {

        }

        private void GetFileAndOverwrite(string localPath, string remotePath, string tempPath)
        {

        }
       
    }
}
