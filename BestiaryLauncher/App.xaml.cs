using BestiaryLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BestiaryLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Dispatcher.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            MainViewModel.UserActionLog.Fatal("Unobserved task exception:", e.Exception);
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MainViewModel.UserActionLog.Fatal("Dispatcher unhandled exception:", e.Exception);
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MainViewModel.UserActionLog.Fatal($"Unhandled exception: {e.ExceptionObject}");
        }

        private void ExitAfterFatalError()
        {
            MessageBox.Show("An error has occurred, please contact Vendrus#5247 on Discord", "Fatal Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            Environment.Exit(1);
        }
    }
}
