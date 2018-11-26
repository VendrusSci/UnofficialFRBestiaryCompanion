using Bestiary.Model;
using Bestiary.ViewModel;
using System.IO;
using System.Windows;

namespace Bestiary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var resourcesDirectory = Directory.GetCurrentDirectory();
            var frDataPath = Path.Combine(resourcesDirectory, "Resources/FRData.xml");
            var userDataPath = Path.Combine(resourcesDirectory, "User Data/UserData.xml");
            DataContext = new MainViewModel(this, new XmlModelStorage(frDataPath, userDataPath));
        }
    }
}
