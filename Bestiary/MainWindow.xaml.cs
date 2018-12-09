using Bestiary.Model;
using Bestiary.ViewModel;
using System.Reflection;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
            var bookmarkDataPath = Path.Combine(dataDirectory, "User Data/BookmarkData.xml");
            DataContext = new MainViewModel(this, new XmlModelStorage(frDataPath, userDataPath, bookmarkDataPath));
        }

        public void ChangeSelectedRow(object Sender, MouseButtonEventArgs e)
        {
            var dgrow = (DataGridRow)Sender;
            DependencyObject parent = VisualTreeHelper.GetParent((Visual)e.OriginalSource);
            while(parent as DataGrid == null && parent != null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            if(parent as DataGrid != null)
            {
                var dataGrid = (DataGrid)parent;
                dataGrid.SelectedIndex = dgrow.GetIndex();
            }
        }
    }
}
