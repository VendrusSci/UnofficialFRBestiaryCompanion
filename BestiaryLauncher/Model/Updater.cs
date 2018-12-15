using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BestiaryLauncher.Model
{
    enum VersionType
    {
        LauncherVersion,
        SoftwareVersion
    }

    class Updater
    {
        public Updater()
        {
            Directory.CreateDirectory(ApplicationPaths.GetTempDirectory());
        }

        private bool IsVersionDifferent(VersionType project)
        {
            bool result = false;

            using (WebClient client = new WebClient())
            {
                switch (project)
                {
                    case VersionType.SoftwareVersion:
                        try
                        {
                            client.DownloadFile(ApplicationPaths.RemoteUbcVersionFile, Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.UbcVersionFile));
                        }
                        catch(WebException ex)
                        {
                            //MainViewModel.UserActionLog.Error("Failed to download file");
                        }
                        string tempVersionFile = Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.UbcVersionFile);
                        if (File.Exists(ApplicationPaths.GetUBCVersionPath()) && File.Exists(tempVersionFile))
                        {
                            if(File.ReadAllText(ApplicationPaths.GetUBCVersionPath()) == File.ReadAllText(tempVersionFile))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                    case VersionType.LauncherVersion:
                        try
                        {
                            client.DownloadFile(ApplicationPaths.RemoteLauncherVersionFile, Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherVersionFile));
                        }
                        catch (WebException ex)
                        {
                            //MainViewModel.UserActionLog.Error("Failed to download file");
                        }
                        string tempLauncherVersionFile = Path.Combine(ApplicationPaths.GetTempDirectory(), ApplicationPaths.LauncherVersionFile);
                        if (File.Exists(ApplicationPaths.GetLauncherVersionPath()) && File.Exists(tempLauncherVersionFile))
                        {
                            if (File.ReadAllText(ApplicationPaths.GetLauncherVersionPath()) == File.ReadAllText(tempLauncherVersionFile))
                            {
                                result = true;
                            }
                            else
                            {
                                result = false;
                            }
                        }
                        break;
                }
            }

                

            return result;
        }

        private bool FamiliarUpdateAvailable()
        {
            bool result = false;




            return result;
        }
    }
}
