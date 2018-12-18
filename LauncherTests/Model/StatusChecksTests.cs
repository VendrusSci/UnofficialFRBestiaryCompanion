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
            loader.Load(ApplicationPaths.UbcVersionFile).Returns(Encoding.ASCII.GetBytes("expected version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, VersionType.UbcVersion, "expected version");
            loader.Received().Load(ApplicationPaths.UbcVersionFile);
            Assert.IsFalse(wasDifferent, "expected version should match");
        }

        [Test]
        public void TestIsVersionDifferentWithDifferentVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.UbcVersionFile).Returns(Encoding.ASCII.GetBytes("unexpected version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, VersionType.UbcVersion, "expected version");
            loader.Received().Load(ApplicationPaths.UbcVersionFile);
            Assert.IsTrue(wasDifferent, "expected version should NOT match");
        }

        [Test]
        public void TestIsLauncherVersionDifferentWithSameVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.GetLauncherVersionPath()).Returns(Encoding.ASCII.GetBytes("expected launcher version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, VersionType.LauncherVersion, "expected launcher version");
            loader.Received().Load(ApplicationPaths.GetLauncherVersionPath());
            Assert.IsFalse(wasDifferent, "expected launcher version should match");
        }

        [Test]
        public void TestIsLauncherVersionDifferentWithDifferentVersion()
        {
            var loader = Substitute.For<ILoadFiles>();
            loader.Load(ApplicationPaths.GetLauncherVersionPath()).Returns(Encoding.ASCII.GetBytes("unexpected launcher version"));
            var wasDifferent = StatusChecks.IsVersionDifferent(loader, VersionType.LauncherVersion, "expected launcher version");
            loader.Received().Load(ApplicationPaths.GetLauncherVersionPath());
            Assert.IsTrue(wasDifferent, "expected launcher version should NOT match");
        }

        [Test]
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

        [Test]
        public void TestGetLatestVersionNumber()
        {
            var downloader = Substitute.For<IDownloadFiles>();
            downloader.Download("testUrl").Returns(Encoding.ASCII.GetBytes("this is a version number"));
            var versionNumber = StatusChecks.GetLatestVersionNumber(downloader, "testUrl");
            downloader.Received().Download("testUrl");
            Assert.IsTrue(versionNumber == "this is a version number", "Version number received");
        }
    }
}
