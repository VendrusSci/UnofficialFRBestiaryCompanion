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
#if DEBUG
            var resourcesDirectory = "../../Resources/";
#else
            var resourcesDirectory = Directory.GetCurrentDirectory();
#endif
            var frDataPath = Path.Combine(resourcesDirectory, "FRData.xml");
            var userDataPath = Path.Combine(resourcesDirectory, "UserData.xml");
            DataContext = new MainViewModel(this, new XmlModelStorage(frDataPath, userDataPath));
        }
    }
}
