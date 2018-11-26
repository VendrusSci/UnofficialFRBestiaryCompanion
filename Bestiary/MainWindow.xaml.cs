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

            var dataDirectory = ApplicationPaths.GetDataDirectory();
            var frDataPath = Path.Combine(dataDirectory, "Resources/FRData.xml");
            var userDataPath = Path.Combine(dataDirectory, "User Data/UserData.xml");
            DataContext = new MainViewModel(this, new XmlModelStorage(frDataPath, userDataPath));
        }
    }
}
