using Bestiary.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.ViewModel
{
    class FetchUpdateViewModel
    {
        private IModel m_LocalFamiliarModel;
        private IModel m_UpdateFamiliarModel;

#if DEBUG
        private const string m_RemoteUpdateFilePath = "FRTest\\SourceFamiliars.xml";
#else
        private const string m_RemoteUpdateFilePath = null;
#endif
        private const string m_LocalUpdateFilePath = @"SourceFamiliars.xml";

        public FetchUpdateViewModel(IModel model)
        {
            m_LocalFamiliarModel = model;
        }

        private bool FetchUpdateFile(string filePath)
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

        private void UpdateLocalFile(string updateFilePath)
        {
            string filePath = Path.Combine(ApplicationPaths.GetResourcesDirectory(), "FRData.xml");
            if (!StructuralComparisons.StructuralEqualityComparer.Equals(GetHashValue(m_LocalUpdateFilePath), GetHashValue(filePath)))
            {
                //if the hashes aren't the same, make a new model and compare all the familiars
                m_UpdateFamiliarModel = new XmlModelStorage(m_LocalUpdateFilePath, null);
                foreach(var familiar in m_UpdateFamiliarModel.Familiars)
                {
                    var matchedFamiliar = m_LocalFamiliarModel.LookupFamiliar(familiar);
                    if (matchedFamiliar != null)
                    {
                        matchedFamiliar.Delete();
                    }
                    m_LocalFamiliarModel.AddFamiliar(matchedFamiliar.Fetch());
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
