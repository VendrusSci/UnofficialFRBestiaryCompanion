using System.IO;
using System.Text;
using BestiaryLauncher.Model;
using NSubstitute;
using NUnit.Framework;

namespace LauncherTests
{
    public class StatusChecksTests
    {
        [Test]
        public void TestIsVersionDifferentWithSameVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.VersionFile).Returns(Encoding.ASCII.GetBytes("expected version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, "expected version");
            loader.Received().Load(ApplicationPaths.VersionFile);
            Assert.IsFalse(wasDifferent, "expected version should match");
        }

        [Test]
        public void TestIsVersionDifferentWithDifferentVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.VersionFile).Returns(Encoding.ASCII.GetBytes("unexpected version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, "expected version");
            loader.Received().Load(ApplicationPaths.VersionFile);
            Assert.IsTrue(wasDifferent, "expected version should NOT match");
        }

        public void TestLauncherUpdateAvailableWithAvailable()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download("testUrl").Returns(Encoding.ASCII.GetBytes("this is new"));
            var updateAvailable = StatusChecks.LauncherUpdateAvailable(loader, downloader, "testUrl");
            loader.Received().Load(Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile));
            downloader.Received().Download("testUrl");
            Assert.IsTrue(updateAvailable, "launcher update should be available");
        }

        [Test]
        public void TestLauncherUpdateAvailableWithNotAvailable()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download("testUrl").Returns(Encoding.ASCII.GetBytes("this is old"));
            var updateAvailable = StatusChecks.LauncherUpdateAvailable(loader, downloader, "testUrl");
            loader.Received().Load(Path.Combine(ApplicationPaths.GetLauncherDirectory(), ApplicationPaths.LauncherExeFile));
            downloader.Received().Download("testUrl");
            Assert.IsFalse (updateAvailable, "launcher update should be available");
        }

        [Test]
        public void TestUbcUpdateAvailableWithAvailable()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download("testUrl").Returns(Encoding.ASCII.GetBytes("this is new"));
            var updateAvailable = StatusChecks.UbcUpdateAvailable(loader, downloader, "testUrl");
            loader.Received().Load(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile));
            downloader.Received().Download("testUrl");
            Assert.IsTrue(updateAvailable, "UBC update should be available");
        }

        [Test]
        public void TestUbcUpdateAvailableWithNotAvailable()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download("testUrl").Returns(Encoding.ASCII.GetBytes("this is old"));
            var updateAvailable = StatusChecks.UbcUpdateAvailable(loader, downloader, "testUrl");
            downloader.Received().Download("testUrl");
            Assert.IsFalse(updateAvailable, "UBC update should be available");
        }

        [Test]
        public void TestFamiliarUpdateAvailableWithAvailable()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), ApplicationPaths.FRDataFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download(ApplicationPaths.RemoteFRDataFile).Returns(Encoding.ASCII.GetBytes("this is new"));
            var updateAvailable = StatusChecks.FamiliarUpdateAvailable(loader, downloader);
            loader.Received().Load(Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), ApplicationPaths.FRDataFile));
            Assert.IsTrue(updateAvailable, "Familiar update should be available");
        }

        [Test]
        public void TestFamiliarUpdateAvailableWithNotAvailable()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), ApplicationPaths.FRDataFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download(ApplicationPaths.RemoteFRDataFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            var updateAvailable = StatusChecks.FamiliarUpdateAvailable(loader, downloader);
            loader.Received().Load(Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), ApplicationPaths.FRDataFile));
            Assert.IsFalse(updateAvailable, "Familiar update should be available");
        }
    }
}
