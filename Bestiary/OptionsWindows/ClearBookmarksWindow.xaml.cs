using Bestiary.Model;
using System.Windows;
using Bestiary.ViewModel;

namespace Bestiary.OptionsWindows
{
    /// <summary>
    /// Interaction logic for ClearBookmarksWindow.xaml
    /// </summary>
    public partial class ClearBookmarksWindow : Window
    {
        public ClearBookmarksWindow(IModel model)
        {
            InitializeComponent();
            DataContext = new ClearBookmarksViewModel(model);
        }
    }
}
