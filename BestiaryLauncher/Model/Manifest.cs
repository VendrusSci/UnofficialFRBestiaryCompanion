using System.IO;
using System.Text;
using Newtonsoft.Json;

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
                var json = JsonConvert.SerializeObject(new ManifestData());
                fileManipulator.WriteAllBytes(fullPath, Encoding.ASCII.GetBytes(json));
            }
        }

        public ManifestData FetchLocal(ILoadFiles fileLoader, string filePath)
        {
            try
            {
                return JsonConvert.DeserializeObject<ManifestData>(fileLoader.LoadAsString(filePath));
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
                return JsonConvert.DeserializeObject<ManifestData>(fileDownloader.DownloadAsString(url));
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
                //need something there!
                var json = JsonConvert.SerializeObject(manifestData);
                fileManipulator.WriteAllBytes(fullPath, Encoding.ASCII.GetBytes(json));
            }
        }
    }

    public class ManifestData
    {
        public string Version { get; set; }

        public string UBCLauncherZip { get; set; }
        public string UnofficialBestiaryCompanionZip { get; set; }

        public string FamiliarDataZip { get; set; }

        public string ViewIconsZip { get; set; }
        public string DisplayIconsZip { get; set; }
        public string IconsZip { get; set; }
        public string ImagesZip { get; set; }
        public string LauncherImagesZip { get; set; }
    }
}
