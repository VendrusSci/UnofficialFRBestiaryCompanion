using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace BestiaryLauncher.Model
{
    public class Manifest
    {
        public Manifest(IManipulateFiles fileManipulator)
        {
            var fullPath = Path.Combine(ApplicationPaths.GetLauncherResourcesDirectory(), "manifest.txt");
            if (!fileManipulator.Exists(fullPath))
            {
                //need something there!
                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ManifestData));
                ser.WriteObject(ms, new ManifestData());
                fileManipulator.WriteAllBytes(fullPath, ms.ToArray());
                ms.Close();
            }
        }

        public ManifestData FetchLocal(ILoadFiles fileLoader, string filePath)
        {
            try
            {
                //return JsonConvert.DeserializeObject<ManifestData>(fileLoader.LoadAsString(filePath));

                ManifestData temp = new ManifestData();
                MemoryStream ms = new MemoryStream(fileLoader.Load(filePath));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(temp.GetType());
                temp = ser.ReadObject(ms) as ManifestData;
                ms.Close();
                return temp;
            }
            catch
            {
                return null;
            }
        }

        public ManifestData FetchLatest(IDownloadFiles fileDownloader, string url)
        {
            try
            {
                //return JsonConvert.DeserializeObject<ManifestData>(fileDownloader.DownloadAsString(url));
                ManifestData temp = new ManifestData();
                MemoryStream ms = new MemoryStream(fileDownloader.Download(url));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(temp.GetType());
                temp = ser.ReadObject(ms) as ManifestData;
                ms.Close();
                return temp;
            }
            catch
            {
                return null;
            }
        }

        public static void UpdateManifest(IManipulateFiles fileManipulator, ManifestData manifestData)
        {
            var fullPath = Path.Combine(ApplicationPaths.GetLauncherResourcesDirectory(), "manifest.txt");
            if (fileManipulator.Exists(fullPath))
            {
                //var json = JsonConvert.SerializeObject(manifestData);
                //fileManipulator.WriteAllBytes(fullPath, Encoding.ASCII.GetBytes(json));

                MemoryStream ms = new MemoryStream();
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ManifestData));
                ser.WriteObject(ms, manifestData);
                fileManipulator.WriteAllBytes(fullPath, ms.ToArray());
                ms.Close();
            }
        }
    }

    [DataContract]
    public class ManifestData
    {
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public string UBCLauncherZip { get; set; }
        [DataMember]
        public string UnofficialBestiaryCompanionZip { get; set; }
        [DataMember]
        public string FamiliarDataZip { get; set; }
        [DataMember]
        public string ViewIconsZip { get; set; }
        [DataMember]
        public string DisplayIconsZip { get; set; }
        [DataMember]
        public string IconsZip { get; set; }
        [DataMember]
        public string ImagesZip { get; set; }
        [DataMember]
        public string LauncherImagesZip { get; set; }
    }
}
