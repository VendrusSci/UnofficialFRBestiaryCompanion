using Bestiary.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    class FetchUpdateViewModel : INotifyPropertyChanged
    {
        public FetchUpdateViewModel(IModel model)
        {
            m_LocalFamiliarModel = model;
        }

        public string StatusString { get; private set; }

        private LambdaCommand m_FetchUpdate;
        private bool m_IsBusy = false;
        public ICommand FetchUpdate
        {
            get
            {
                if (m_FetchUpdate == null)
                {
                    m_FetchUpdate = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            Task.Run(() =>
                            {
                                m_IsBusy = true;

                                StatusString = "Fetching updated familiar list...";

                                if (FetchUpdateFile())
                                {
                                    StatusString = "Familiar list found, updating local file";
                                    UpdateLocalFile();
                                    StatusString = "Update complete!";
                                }
                                else
                                {
                                    StatusString = "Can't find file to update from";
                                }

                                m_IsBusy = false;
                            });
                        },
                        onCanExecute: (p) =>
                        {
                            return !m_IsBusy;
                        }
                    );
                }
                return m_FetchUpdate;
            }
        }

        private IModel m_LocalFamiliarModel;
        private IModel m_UpdateFamiliarModel;

#if DEBUG
        private const string m_RemoteUpdateFilePath = "FRTest\\SourceFamiliars.xml";
#else
        private const string m_RemoteUpdateFilePath = null;
#endif
        private const string m_LocalUpdateFilePath = "SourceFamiliars.xml";

        public event PropertyChangedEventHandler PropertyChanged;

        private bool FetchUpdateFile()
        {
#if !DEBUG
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(m_RemoteUpdateFilePath, m_LocalUpdateFilePath);
            }
#endif
            if(File.Exists(m_LocalUpdateFilePath))
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private void UpdateLocalFile()
        {
            string filePath = Path.Combine(ApplicationPaths.GetResourcesDirectory(), "FRData.xml");
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(GetHashValue(m_LocalUpdateFilePath), GetHashValue(filePath)))
            {
                //if the hashes aren't the same, make a new model and compare all the familiars
                m_UpdateFamiliarModel = new XmlModelStorage(m_LocalUpdateFilePath, "");
                foreach(var familiar in m_UpdateFamiliarModel.Familiars)
                {
                    var localFamiliar = m_LocalFamiliarModel.LookupFamiliar(familiar);
                    if (localFamiliar != null)
                    {
                        localFamiliar.Delete();
                    }
                    m_LocalFamiliarModel.AddFamiliar(m_UpdateFamiliarModel.LookupFamiliar(familiar).Fetch());
                }
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
