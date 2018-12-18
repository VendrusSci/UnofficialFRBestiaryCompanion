using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BestiaryLauncher.Model
{
    class TestDownloader : IDownloadFiles
    {
        private string basePath = Path.Combine("..\\..\\..\\", "BestiaryTestData");

        public byte[] Download(string url)
        {
            var fakePath = Path.Combine(basePath, Path.GetFileName(url));
            return File.ReadAllBytes(fakePath);
        }
    }
}
