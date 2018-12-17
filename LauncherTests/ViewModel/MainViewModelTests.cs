using BestiaryLauncher.Model;
using BestiaryLauncher.ViewModels;
using NSubstitute;
using NUnit.Framework;
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
    }
}
