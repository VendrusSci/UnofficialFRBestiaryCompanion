using Bestiary.ViewModel;
using System.Windows;


namespace Bestiary
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(string version)
        {
            InitializeComponent();
            DataContext = new AboutViewModel(version);
        }
    }
}
