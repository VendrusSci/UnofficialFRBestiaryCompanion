using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

namespace Bestiary.ViewModel.OptionsViews
{
    class FamiliarImportViewModel : INotifyPropertyChanged
    {
        public string ImportStatus { get; private set; }
        private string m_Fail = "Update failed, check directory selection";
        private string m_Success = "Update succeeded, restart UBC";
        private string m_Waiting = "No folder selected";
        private string m_Cancelled = "Update cancelled";

        public FamiliarImportViewModel()
        {
            ImportStatus = m_Waiting;
        }

        private LambdaCommand m_GetImportDirectory;
        public ICommand GetImportDirectory
        {
            get
            {
                m_GetImportDirectory = new LambdaCommand(
                    onExecute: (p) =>
                    {
                        VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
                        dlg.SelectedPath = Directory.GetCurrentDirectory();
                        dlg.ShowNewFolderButton = false;

                        if (dlg.ShowDialog().Value == true)
                        {
                            string path = dlg.SelectedPath;
                            if (Path.GetFileName(path) == "FamiliarUpdate")
                            {
                                //FamiliarUpdate contains FamiliarData, Icons and Images
                                HandleBackup("FamiliarData");
                                HandleBackup("Icons");
                                HandleBackup("Images");

                                using (ZipArchive archive = ZipFile.OpenRead(path))
                                {
                                    try
                                    {
                                        archive.ExtractToDirectory(ApplicationPaths.GetResourcesDirectory());
                                    }
                                    catch(IOException)
                                    {
                                        MainViewModel.UserActionLog.Error("How did you manage this");
                                    }
                                }
                                ImportStatus = m_Success;
                            }
                            else
                            {
                                ImportStatus = m_Fail;
                            }
                        }
                        else
                        {
                            ImportStatus = m_Cancelled;
                        }
                    }
                );
                return m_GetImportDirectory;
            }
        }

        private void HandleBackup(string dirName)
        {
            var dirPath = Path.Combine(ApplicationPaths.GetResourcesDirectory(), dirName);
            var bkup_dir = dirPath + "_bak";
            if (Directory.Exists(bkup_dir))
            {
                Directory.Delete(bkup_dir);
            }
            if (Directory.Exists(dirPath))
            {
                Directory.Move(dirPath, bkup_dir);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
