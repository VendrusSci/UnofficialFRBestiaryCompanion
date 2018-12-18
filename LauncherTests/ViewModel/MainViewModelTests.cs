using BestiaryLauncher.Model;
using BestiaryLauncher.ViewModels;
using NSubstitute;
using NUnit.Framework;
using System.IO;
using System.Text;

namespace LauncherTests.ViewModel
{
    public class MainViewModelTests
    {
        private ILoadFiles loader;
        private IDownloadFiles downloader;
        private IUnzipFiles unzipper;
        private IManipulateFiles fileManipulator;
        private IManipulateDirectories directoryManipulator;
        private IStartProcesses processStarter;
        private ICloseApplications applicationCloser;

        [SetUp]
        public void CreateSubstitutes()
        {
            loader = Substitute.For<ILoadFiles>();
            downloader = Substitute.For<IDownloadFiles>();
            unzipper = Substitute.For<IUnzipFiles>();
            fileManipulator = Substitute.For<IManipulateFiles>();
            directoryManipulator = Substitute.For<IManipulateDirectories>();
            processStarter = Substitute.For<IStartProcesses>();
            applicationCloser = Substitute.For<ICloseApplications>();
        }

        [Test]
        public void TestUpdateLauncherExecuteWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            viewModel.UpdateLauncher.Execute(null);
            Assert.IsTrue(viewModel.UpdateStatusText == "Launcher update complete", "Update should have completed");
        }

        [Test]
        public void TestUpdateLauncherExecuteWithFailure()
        {
            downloader.Download(Arg.Any<string>()).Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            viewModel.UpdateLauncher.Execute(null);
            Assert.IsFalse(viewModel.UpdateStatusText == "Launcher update complete", "Update should have completed");
        }

        [Test]
        public void TestUpdateLauncherOnExecuteWithUpdateAvailable()
        {
            loader.Load(ApplicationPaths.LauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is new"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            var isAvailable = viewModel.UpdateLauncher.CanExecute(null);
            Assert.IsTrue(isAvailable, "Update should be available");
        }

        [Test]
        public void TestUpdateLauncherOnExecuteWithNoUpdateAvailable()
        {
            loader.Load(ApplicationPaths.LauncherVersionFile).Returns(Encoding.ASCII.GetBytes("this is old"));
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is old"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            var isAvailable = viewModel.UpdateLauncher.CanExecute(null);
            Assert.IsFalse(isAvailable, "Update should not be available");
        }

        [Test]
        public void TestUpdateSoftwareExecuteWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            viewModel.UpdateSoftware.Execute(null);
            Assert.IsTrue(viewModel.UpdateStatusText == "Update Successful", "Update should have completed");
        }

        [Test]
        public void TestUpdateSoftwareExecuteWithFailure()
        {
            downloader.Download(Arg.Any<string>()).Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            viewModel.UpdateSoftware.Execute(null);
            Assert.IsTrue(viewModel.UpdateStatusText == "Update Failed, check connection or report bug", "Update should have completed");
        }

        [Test]
        public void TestUpdateSoftwareOnExecuteWithUpdateAvailable()
        {
            loader.Load(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is new"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            var isAvailable = viewModel.UpdateSoftware.CanExecute(null);
            Assert.IsTrue(isAvailable, "Update should be available");
        }

        [Test]
        public void TestUpdateSoftwareOnExecuteWithNoUpdateAvailable()
        {
            loader.Load(Path.Combine(ApplicationPaths.GetDataDirectory(), ApplicationPaths.UbcExeFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is old"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            var isAvailable = viewModel.UpdateSoftware.CanExecute(null);
            Assert.IsFalse(isAvailable, "Update should not be available");
        }

        [Test]
        public void TestUpdateFamiliarsExecuteWithSuccess()
        {
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            viewModel.UpdateFamiliars.Execute(null);
            Assert.IsTrue(viewModel.UpdateStatusText == "Update Successful", "Update should have completed");
        }

        [Test]
        public void TestUpdateFamiliarsExecuteWithFailure()
        {
            downloader.Download(Arg.Any<string>()).Returns(_ => null);
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            viewModel.UpdateFamiliars.Execute(null);
            Assert.IsTrue(viewModel.UpdateStatusText == "Update Failed, check connection or report bug", "Update should have completed");
        }

        [Test]
        public void TestUpdateFamiliarsOnExecuteWithUpdateAvailable()
        {
            loader.Load(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile)).Returns(Encoding.ASCII.GetBytes("this is old"));
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is new"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            var isAvailable = viewModel.UpdateFamiliars.CanExecute(null);
            Assert.IsTrue(isAvailable, "Update should be available");
        }

        [Test]
        public void TestUpdateFamiliarsOnExecuteWithNoUpdateAvailable()
        {
            loader.Load(Path.Combine(ApplicationPaths.GetResourcesDirectory(), ApplicationPaths.FRDataFile))
                .Returns(Encoding.ASCII.GetBytes("this is old"));
            downloader.Download(Arg.Any<string>()).Returns(Encoding.ASCII.GetBytes("this is old"));
            directoryManipulator.Exists(Arg.Any<string>()).Returns(false);
            MainViewModel viewModel = new MainViewModel(loader, downloader, unzipper, fileManipulator, directoryManipulator, processStarter, applicationCloser);
            var isAvailable = viewModel.UpdateFamiliars.CanExecute(null);
            Assert.IsFalse(isAvailable, "Update should not be available");
        }
    }
}
