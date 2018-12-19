using System.IO;

namespace BestiaryLauncher.Model
{
    class TestDownloader : IDownloadFiles
    {
        private string basePath = Path.Combine("..\\..\\..\\", "BestiaryReleaseData");

        public byte[] Download(string url)
        {
            if(url == ApplicationPaths.RemoteGitReleaseInfoPath)
            {
                return new FileDownloader().Download(url);
            }
            else
            {
                var fakePath = Path.Combine(basePath, Path.GetFileName(url));
                return File.ReadAllBytes(fakePath);
            }
            
        }
    }
}
