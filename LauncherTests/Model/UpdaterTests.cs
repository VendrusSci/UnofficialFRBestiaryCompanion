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
        private IStartProcesses processStarter;

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
            loader.Load(ApplicationPaths.GetLauncherVersionPath()).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasDifferent = updater.LauncherUpdateAvailable();
            Assert.IsTrue(wasDifferent, "Update should be available");
        }

        [Test]
        public void TestIsLauncherUpdateAvailableWithNotAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteLauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(ApplicationPaths.GetLauncherVersionPath()).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasDifferent = updater.LauncherUpdateAvailable();
            Assert.IsFalse(wasDifferent, "Update should not be available");
        }

        [Test]
        public void TestIsSoftwareUpdateAvailableWithAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteUbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(ApplicationPaths.UbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasDifferent = updater.SoftwareUpdateAvailable();
            Assert.IsTrue(wasDifferent, "Update should be available");
        }

        [Test]
        public void TestIsSoftwareUpdateAvailableWithNotAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteUbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(ApplicationPaths.UbcVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasDifferent = updater.SoftwareUpdateAvailable();
            Assert.IsFalse(wasDifferent, "Update should not be available");
        }

        [Test]
        public void TestFamiliarUpdateAvailableWithAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteFRDataFile).Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), ApplicationPaths.FRDataFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasAvailable = updater.FamiliarUpdateAvailable();
            Assert.IsTrue(wasAvailable, "Familiar update should be available");
        }

        [Test]
        public void TestFamiliarUpdateAvailableWithNotAvailable()
        {
            downloader.Download(ApplicationPaths.RemoteFRDataFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryResourcesDirectory(), ApplicationPaths.FRDataFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasAvailable = updater.FamiliarUpdateAvailable();
            Assert.IsFalse(wasAvailable, "Familiar update should not be available");
        }

        [Test]
        public void TestUbcUpdateAvailableWithAvailable()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(Encoding.ASCII.GetBytes("this is new"));
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasAvailable = updater.UbcUpdateAvailable();
            Assert.IsTrue(wasAvailable, "Ubc update should be available");
        }

        [Test]
        public void TestUbcUpdateAvailableWithNotAvailable()
        {
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is old"));
            loader.Load(Path.Combine(ApplicationPaths.GetBestiaryDirectory(), ApplicationPaths.UbcExeFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasAvailable = updater.UbcUpdateAvailable();
            Assert.IsFalse(wasAvailable, "Ubc update should not be available");
        }

        [Test]
        public void TestUpdateUbcSoftwareWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateUbcSoftware();
            Assert.IsTrue(wasSuccess, "Update should have suceeded");
        }

        [Test]
        public void TestUpdateUbcSoftwareWithDownloadFailure()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateUbcSoftware();
            Assert.IsFalse(wasSuccess, "Update should have failed");
        }

        [Test]
        public void TestUpdateLauncherWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateLauncher();
            Assert.IsTrue(wasSuccess, "Update should have suceeded");
        }

        [Test]
        public void TestUpdateLauncherWithDownloadFailure()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateLauncher();
            Assert.IsFalse(wasSuccess, "Update should have failed");
        }

        [Test]
        public void TestUpdateFamiliarsWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateFamiliars();
            Assert.IsTrue(wasSuccess, "Update should have suceeded");
        }

        [Test]
        public void TestUpdateFamiliarsWithDownloadFailure()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateFamiliars();
            Assert.IsFalse(wasSuccess, "Update should have failed");
        }

        [Test]
        public void TestUpdateVersionFileUbcWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateVersionFile(VersionType.UbcVersion);
            Assert.IsTrue(wasSuccess, "Update should have suceeded");
        }

        [Test]
        public void TestUpdateVersionFileUbcWithDownloadFailure()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateVersionFile(VersionType.UbcVersion);
            Assert.IsFalse(wasSuccess, "Update should have failed");
        }

        [Test]
        public void TestUpdateVersionFileLauncherWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateVersionFile(VersionType.LauncherVersion);
            Assert.IsTrue(wasSuccess, "Update should have suceeded");
        }

        [Test]
        public void TestUpdateVersionFileLauncherWithDownloadFailure()
        {
            downloader.Download(Arg.Any<string>())
                .Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            Updater updater = new Updater(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter);
            var wasSuccess = updater.UpdateVersionFile(VersionType.LauncherVersion);
            Assert.IsFalse(wasSuccess, "Update should have failed");
        }
    }
}
