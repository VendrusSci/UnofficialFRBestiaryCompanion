using BestiaryLauncher.ViewModels;
using System.Windows;
using BestiaryLauncher.Model;
using System.Reflection;

namespace BestiaryLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(new FileLoader(), new FileDownloader(), new FileUnzipper(),
                new FileManipulator(), new DirectoryManipulator(), new ProcessStarter(), new ApplicationCloser());
        }
    }
}
