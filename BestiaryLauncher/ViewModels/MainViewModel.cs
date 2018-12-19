using BestiaryLauncher.Model;
using Bestiary;
using System.Windows.Input;
using System.Windows;

namespace BestiaryLauncher.ViewModels
{
    public class MainViewModel
    {
        public string LauncherUpdateStatus { get; private set; }
        public string SoftwareUpdateStatus { get; private set; }
        public string LaunchButtonText { get; private set; }
        public string UpdateStatusText { get; private set; }
        public bool UbcExists { get; private set; }

        private string m_Hidden = "Hidden";
        private string m_Visible = "Visible";
        private string m_LaunchButtonUpdateAvailable = "Postpone Update and Launch";
        private string m_LaunchButtonNoUpdateAvailable = "Launch";
        private string m_UpdateSuccess = "Update Successful";
        private string m_UpdateFail = "Update Failed, check connection or report bug";

        private ICloseApplications m_ApplicationCloser;
        private IManipulateDirectories m_DirectoryManipulator;
        private Updater m_Updater;
        public MainViewModel(ILoadFiles fileLoader, IDownloadFiles fileDownloader, IUnzipFiles fileUnzipper,
            IManipulateFiles fileManipulator, IManipulateDirectories directoryManipulator, 
            IStartProcesses processStarter, ICloseApplications applicationCloser)
        {
            m_Updater = new Updater(fileLoader, fileDownloader, fileUnzipper, fileManipulator, directoryManipulator, processStarter);
            m_ApplicationCloser = applicationCloser;
            m_DirectoryManipulator = directoryManipulator;

            LauncherUpdateStatus = m_Hidden;
            SoftwareUpdateStatus = m_Hidden;
            UbcExists = false;

            if(m_Updater.SoftwareUpdateAvailable())
            {
                if (m_Updater.LauncherUpdateAvailable())
                {
                    //Set Launcher update stuff to visible
                    LauncherUpdateStatus = m_Visible;
                }
                else
                {
                    //Set UBC update stuff to visible
                    SoftwareUpdateStatus = m_Visible;
                }
            }
            else
            {
                NoUpdate.Execute(null);
            }

            if (directoryManipulator.Exists(ApplicationPaths.GetBestiaryDirectory()))
            {
                LaunchButtonText = m_LaunchButtonUpdateAvailable;
                UbcExists = true;
            }
            else
            {
                LaunchButtonText = "Awaiting Install";
                UbcExists = false;
            }
        }

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
                            if(m_Updater.UpdateLauncher())
                            {
                                UpdateStatusText = "Launcher update complete";
                                LauncherUpdateStatus = m_Hidden;
                                SoftwareUpdateStatus = m_Visible;
                            }
                            else
                            {
                                UpdateStatusText = "Launcher update failed";
                            }
                        },
                        onCanExecute: (p) =>
                        {
                            return m_Updater.LauncherUpdateAvailable();
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
                            UpdateStatusText = "Updating Familiars...";
                            bool result = m_Updater.UpdateFamiliars();
                            UpdateStatusText = result ? m_UpdateSuccess : m_UpdateFail;
                            if (!CheckForUbcUpdates())
                            {
                                LaunchButtonText = m_LaunchButtonNoUpdateAvailable;
                            }
                        },
                        onCanExecute: (p) =>
                        {
                            return m_Updater.FamiliarUpdateAvailable();
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
                            //If the folder structure does not exist, need to create it
                            if (!m_DirectoryManipulator.Exists(ApplicationPaths.GetBestiaryDirectory()))
                            {
                                m_DirectoryManipulator.Create(ApplicationPaths.GetBestiaryDirectory());
                                m_DirectoryManipulator.Create(ApplicationPaths.GetBestiaryResourcesDirectory());
                                m_DirectoryManipulator.Create(ApplicationPaths.GetBestiaryUserDataDirectory());
                            }

                            UpdateStatusText = "Updating Software...";
                            bool result = m_Updater.UpdateUbcSoftware();
                            UpdateStatusText = result ? m_UpdateSuccess : m_UpdateFail;
                            if (m_Updater.FamiliarUpdateAvailable())
                            {
                                UpdateStatusText = "Updating Familiars...";
                                result = m_Updater.UpdateFamiliars();
                                UpdateStatusText = result ? m_UpdateSuccess : m_UpdateFail;
                            }
                        },
                        onCanExecute: (p) =>
                        {
                            return m_Updater.UbcUpdateAvailable() | (!UbcExists);
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
                            m_Updater.LaunchUbc();
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
    }
}
