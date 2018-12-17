using System.IO;
using System.Text;
using BestiaryLauncher.Model;
using NSubstitute;
using NUnit.Framework;

namespace LauncherTests.Model
{
    public class UpdaterTests
    {
        private ILoadFiles loader;
        private IDownloadFiles downloader;
        private IUnzipFiles unzipper;
        private IManipulateFiles fileManipulator;
        private IManipulateDirectories directoryManipulator;

        [SetUp]
        public void CreateSubstitutes()
        {
            loader = Substitute.For<ILoadFiles>();
            downloader = Substitute.For<IDownloadFiles>();
            unzipper = Substitute.For<IUnzipFiles>();
            fileManipulator = Substitute.For<IManipulateFiles>();
            directoryManipulator = Substitute.For<IManipulateDirectories>();
        }

        [Test]
        public void TestIsLauncherUpdateAvailableWithAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteLauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(ApplicationPaths.LauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasDifferent = updater.LauncherUpdateAvailable();
            Assert.IsTrue(wasDifferent, "Update should be available");
        }

        [Test]
        public void TestIsLauncherUpdateAvailableWithNotAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteLauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(ApplicationPaths.LauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasDifferent = updater.LauncherUpdateAvailable();
            Assert.IsFalse(wasDifferent, "Update should not be available");
        }

        [Test]
        public void TestIsSoftwareUpdateAvailableWithAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteUbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(ApplicationPaths.UbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasDifferent = updater.SoftwareUpdateAvailable();
            Assert.IsTrue(wasDifferent, "Update should be available");
        }

        [Test]
        public void TestIsSoftwareUpdateAvailableWithNotAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteUbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(ApplicationPaths.UbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasDifferent = updater.SoftwareUpdateAvailable();
            Assert.IsFalse(wasDifferent, "Update should not be available");
        }

        [Test]
        public void TestFamiliarUpdateAvailableWithAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteFRDataFile).Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasAvailable = updater.FamiliarUpdateAvailable();
            Assert.IsTrue(wasAvailable, "Familiar update should be available");
        }

        [Test]
        public void TestFamiliarUpdateAvailableWithNotAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteFRDataFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasAvailable = updater.FamiliarUpdateAvailable();
            Assert.IsFalse(wasAvailable, "Familiar update should not be available");
        }

        [Test]
        public void TestUbcUpdateAvailableWithAvailable()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasAvailable = updater.FamiliarUpdateAvailable();
            Assert.IsTrue(wasAvailable, "Familiar update should be available");
        }

        [Test]
        public void TestUbcUpdateAvailableWithNotAvailable()
        {
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasAvailable = updater.FamiliarUpdateAvailable();
            Assert.IsFalse(wasAvailable, "Familiar update should not be available");
        }

        [Test]
        public void TestUpdateUbcSoftwareWithSuccess()
        {
            downloader.Download(ApplicationPaths.RemoteBestiaryImgPath)
                .Returns(Encoding.ASCII.GetBytes("success"));
            downloader.Download(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.bestiaryImgFile))
                .Returns(Encoding.ASCII.GetBytes("success"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasSuccess = updater.UpdateUbcSoftware();
            Assert.IsTrue(wasSuccess, "Update should have suceeded");
        }

        [Test]
        public void TestUpdateUbcSoftwareWithDownloadFailure()
        {
            downloader.Download(ApplicationPaths.RemoteBestiaryImgPath)
                .Returns(_ => null);
            downloader.Download(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.bestiaryImgFile))
                .Returns(Encoding.ASCII.GetBytes("success"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator);
            var wasSuccess = updater.UpdateUbcSoftware();
            Assert.IsFalse(wasSuccess, "Update should have failed");
        }
    }
}
