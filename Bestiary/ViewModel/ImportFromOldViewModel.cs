using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    public class ImportFromOldViewModel : INotifyPropertyChanged
    {
        public string ImportStatus { get; private set; }
        private string m_Fail = "Import failed, check directory selection";
        private string m_Success = "Import succeeded, restart UBC";
        private string m_Waiting = "No folder selected";
        private string m_Cancelled = "Import cancelled";

        public ImportFromOldViewModel()
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
                            if (Path.GetFileName(path) == "User Data")
                            {
                                foreach (var file in Directory.GetFiles(path))
                                {
                                    var importFilename = Path.GetFileName(file);
                                    var existingFilename = Path.Combine(ApplicationPaths.GetUserDirectory(), importFilename);
                                    if (File.Exists(existingFilename))
                                    {
                                        var bkupFilename = existingFilename + ".bak";

                                        if (File.Exists(bkupFilename))
                                        {
                                            File.Delete(bkupFilename);
                                        }
                                        File.Move(existingFilename, bkupFilename);
                                    }
                                    File.Copy(file, existingFilename);
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

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
