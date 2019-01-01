using Bestiary.ViewModel;
using System.Windows;

namespace Bestiary.OtherWindows
{
    /// <summary>
    /// Interaction logic for UserGuideWindow.xaml
    /// </summary>
    public partial class UserGuideWindow : Window
    {
        public UserGuideWindow()
        {
            InitializeComponent();
            DataContext = new UserGuideViewModel();
        }
    }
}
