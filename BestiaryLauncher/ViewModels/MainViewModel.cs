using BestiaryLauncher.Model;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media.Imaging;

namespace BestiaryLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public Visibility LauncherUpdateStatus { get; private set; }
        public Visibility SoftwareUpdateStatus { get; private set; }
        public Visibility InternetConnectionStatus { get; private set; }
        public string LaunchButtonText { get; private set; }
        public string UpdateStatusText { get; private set; }
        public bool UbcExists { get; private set; }
        public BitmapImage HeaderImage { get; private set; }

        private string m_LaunchButtonUpdateAvailable = "Postpone Update and Launch";
        private string m_LaunchButtonNoUpdateAvailable = "Launch";
        private string m_UpdateSuccess = "Update Successful";
        private string m_UpdateFail = "Update Failed, check connection or report bug";

        private bool m_InternetExists = true;

        private ICloseApplications m_ApplicationCloser;
        private IStartProcesses m_ProcessStarter;
        private IManipulateDirectories m_DirectoryManipulator;
        private Updater m_Updater;
        public MainViewModel(ILoadFiles fileLoader, IDownloadFiles fileDownloader, IUnzipFiles fileUnzipper,
            IManipulateFiles fileManipulator, IManipulateDirectories directoryManipulator, 
            IStartProcesses processStarter, ICloseApplications applicationCloser)
        {
            m_Updater = new Updater(fileLoader, fileDownloader, fileUnzipper, fileManipulator, directoryManipulator, processStarter);
            m_InternetExists = m_Updater.LatestVersionNumber != null;

            m_ApplicationCloser = applicationCloser;
            m_DirectoryManipulator = directoryManipulator;
            m_ProcessStarter = processStarter;

            LauncherUpdateStatus = Visibility.Hidden;
            SoftwareUpdateStatus = Visibility.Hidden;
            InternetConnectionStatus = Visibility.Hidden;
            UbcExists = false;

            HeaderImage = ImageLoader.LoadImage(ApplicationPaths.GetHeaderImagePath());

            if (fileManipulator.Exists(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExe)))
            {
                LaunchButtonText = m_LaunchButtonUpdateAvailable;
                UbcExists = true;
            }
            else
            {
                UserActionLog.Info("No UBC exists, awaiting first install");
                LaunchButtonText = "Awaiting Install";
                UbcExists = false;
            }

            if (m_InternetExists)
            {
                if (m_Updater.SoftwareUpdateAvailable())
                {
                    UserActionLog.Info("Version different, update available");
                    if (m_Updater.LauncherUpdateAvailable())
                    {
                        UserActionLog.Info("Launcher update available");
                        //Set Launcher update stuff to visible
                        LauncherUpdateStatus = Visibility.Visible;
                    }
                    else
                    {
                        UserActionLog.Info("No launcher update available");
                        //Set UBC update stuff to visible
                        SoftwareUpdateStatus = Visibility.Visible;
                    }
                }
                else
                {
                    UserActionLog.Info("Version identical, launching UBC");
                    NoUpdate.Execute(null);
                }
            }
            else
            {
                UserActionLog.Error("Cannot access Git, no update checking/updating available");
                InternetConnectionStatus = Visibility.Visible;
            }
        }

        private bool m_UpdateInProgress = false;
        private LambdaCommand m_UpdateLauncher;
        public ICommand UpdateLauncher
        {
            get
            {
                if (m_UpdateLauncher == null)
                {
                    m_UpdateLauncher = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_UpdateInProgress = true;
                            UserActionLog.Info("Updating launcher...");
                            if (m_Updater.UpdateLauncher())
                            {
                                UserActionLog.Info("Launcher update successful");
                                if (!m_Updater.AnyUpdatesRemaining())
                                {
                                    m_Updater.UpdateVersion();
                                }
                                Thread.Sleep(2000);
                                m_ProcessStarter.Start(Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExe));
                                m_ApplicationCloser.Close();
                            }
                            else
                            {
                                UserActionLog.Error("Launcher update failed");
                                UpdateStatusText = "Launcher update failed";
                            }
                            m_UpdateInProgress = false;
                        },
                        onCanExecute: (p) =>
                        {
                            return (m_Updater.LauncherUpdateAvailable() && !m_UpdateInProgress);
                        }
                    );
                }
                return m_UpdateLauncher;
            }
        }

        private LambdaCommand m_UpdateFamiliars;
        public ICommand UpdateFamiliars
        {
            get
            {
                if (m_UpdateFamiliars == null)
                {
                    m_UpdateFamiliars = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_UpdateInProgress = true;
                            UserActionLog.Info("Updating familiars...");
                            UpdateStatusText = "Updating Familiars...";
                            if (m_Updater.UpdateFamiliars())
                            {
                                UserActionLog.Info("Familiar update succeeded");
                                UpdateStatusText = m_UpdateSuccess;
                            }
                            else
                            {
                                UserActionLog.Error("Familiar update failed");
                                UpdateStatusText = m_UpdateFail;
                            }
                            if (!CheckForUbcUpdates())
                            {
                                UbcExists = true;
                                LaunchButtonText = m_LaunchButtonNoUpdateAvailable;
                            }
                            if (!m_Updater.AnyUpdatesRemaining())
                            {
                                m_Updater.UpdateVersion();
                            }
                            m_UpdateInProgress = false;
                        },
                        onCanExecute: (p) =>
                        {
                            return (m_Updater.FamiliarUpdateAvailable() && !m_UpdateInProgress);
                        }
                    );
                }
                return m_UpdateFamiliars;
            }
        }

        private LambdaCommand m_UpdateSoftware;
        public ICommand UpdateSoftware
        {
            get
            {
                if (m_UpdateSoftware == null)
                {
                    m_UpdateSoftware = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_UpdateInProgress = true;
                            //If the folder structure does not exist, need to create it
                            if (!m_DirectoryManipulator.Exists(ApplicationPaths.GetBestiaryDirectory()))
                            {
                                UserActionLog.Info("Base directory structure does not exists, creating folders");
                                m_DirectoryManipulator.Create(ApplicationPaths.GetBestiaryDirectory());
                                m_DirectoryManipulator.Create(ApplicationPaths.GetBestiaryResourcesDirectory());
                                m_DirectoryManipulator.Create(ApplicationPaths.GetBestiaryUserDataDirectory());
                            }

                            UserActionLog.Info("Updating software...");
                            UpdateStatusText = "Updating Software...";
                            bool result = m_Updater.UpdateUbcSoftware();
                            if(result)
                            {
                                UserActionLog.Info("UBC update succeeded");
                                UpdateStatusText = m_UpdateSuccess;
                            }
                            else
                            {
                                UserActionLog.Error("UBC update failed");
                                UpdateStatusText = m_UpdateFail;
                            }
                            if (m_Updater.FamiliarUpdateAvailable())
                            {
                                UserActionLog.Info("Updating familiars...");
                                UpdateStatusText = "Updating Familiars...";
                                result &= m_Updater.UpdateFamiliars();
                                UpdateStatusText = result ? m_UpdateSuccess : m_UpdateFail;
                            }
                            if(result)
                            {
                                UserActionLog.Info("Familiar update succeeded");
                                UpdateStatusText = "Update complete!";
                                UbcExists = true;
                                LaunchButtonText = "Launch UBC";
                            }
                            else
                            {
                                UserActionLog.Error("Familiar update failed");
                            }
                            if(!m_Updater.AnyUpdatesRemaining())
                            {
                                m_Updater.UpdateVersion();
                            }
                            HeaderImage = ImageLoader.LoadImage(ApplicationPaths.GetHeaderImagePath());
                            m_UpdateInProgress = false;
                        },
                        onCanExecute: (p) =>
                        {
                            return (m_Updater.UbcUpdateAvailable() | (!UbcExists)) && !m_UpdateInProgress;
                        }
                    );
                }
                return m_UpdateSoftware;
            }
        }

        private LambdaCommand m_NoUpdate;
        public ICommand NoUpdate
        {
            get
            {
                if (m_NoUpdate == null)
                {
                    m_NoUpdate = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            if (!m_Updater.AnyUpdatesRemaining())
                            {
                                m_Updater.UpdateVersion();
                            }
                            UserActionLog.Info("Launching UBC");
                            m_Updater.LaunchUbc();
                            UserActionLog.Info("Closing launcher");
                            m_ApplicationCloser.Close();
                        }
                    );
                }
                return m_NoUpdate;
            }
        }

        private bool CheckForUbcUpdates()
        {
            return (m_Updater.UbcUpdateAvailable() || m_Updater.FamiliarUpdateAvailable());
        }

        public static readonly log4net.ILog UserActionLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
