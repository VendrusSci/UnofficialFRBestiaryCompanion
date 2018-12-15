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
        SoftwareVersion
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
            LatestUbcVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteUbcVersionFile, "version.txt");
            LatestLauncherVersion = StatusChecks.GetLatestVersionNumber(ApplicationPaths.RemoteLauncherExePath, "launcherversion.txt");
            RemoteExeFullPath = Path.Combine(ApplicationPaths.RemoteExePath, LatestUbcVersion, ApplicationPaths.ExeFile);
            RemoteLauncherExeFullPath = Path.Combine(ApplicationPaths.RemoteLauncherExePath, LatestLauncherVersion, ApplicationPaths.LauncherExeFile);
        }

        public void UpdateUbcSoftware()
        {

        }

        public void UpdateVersionFile()
        {

        }

        public void UpdateFamiliars()
        {

        }

        public void UpdateLauncher()
        {

        }
       
    }
}
