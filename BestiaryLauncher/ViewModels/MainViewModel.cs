using BestiaryLauncher.Model;
using Bestiary;
using System.Windows.Input;
using System.Windows;

namespace BestiaryLauncher.ViewModels
{
    public class MainViewModel
    {
        private Updater m_Updater;
        public MainViewModel()
        {
            m_Updater = new Updater();
            if(m_Updater.LauncherUpdateAvailable())
            {
                //Set Launcher update stuff to visible
                //warn that no UBC updates can occur until launcher is updated
                //if OK
                //m_Updater.UpdateLauncher();
                //Prompt restart
                //else
                //close
            }
            else
            {
                //Set Launcher stuff to not visible
            }
        }

        private LambdaCommand m_UpdateFamiliars;
        public ICommand UpdateFamiliars
        {
            get
            {
                if(m_UpdateFamiliars == null)
                {
                    m_UpdateFamiliars = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_Updater.UpdateFamiliars();
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
                if(m_UpdateSoftware == null)
                {
                    m_UpdateSoftware = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_Updater.UpdateUbcSoftware();
                            if(m_Updater.FamiliarUpdateAvailable())
                            {
                                m_Updater.UpdateFamiliars();
                            }
                        },
                        onCanExecute: (p) =>
                        {
                            return m_Updater.UbcUpdateAvailable();
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
                if(m_NoUpdate == null)
                {
                    m_NoUpdate = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_Updater.LaunchUbc();
                            Application.Current.Shutdown();
                        }
                    );
                }
                return m_NoUpdate;
            }
        }
    }
}
