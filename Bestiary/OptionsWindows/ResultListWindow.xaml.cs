using Bestiary.Model;
using Bestiary.ViewModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace Bestiary
{
    /// <summary>
    /// Interaction logic for ResultListWindow.xaml
    /// </summary>
    public partial class ResultListWindow : Window
    {
        public ResultListWindow(ObservableCollection<FamiliarViewModel> familiars)
        {
            InitializeComponent();
            DataContext = new ResultListViewModel(familiars);
        }
    }
}
