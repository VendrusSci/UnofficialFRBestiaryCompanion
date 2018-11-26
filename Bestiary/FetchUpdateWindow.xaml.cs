using Bestiary.ViewModel;
using System.Windows;

namespace Bestiary
{
    /// <summary>
    /// Interaction logic for FetchUpdateWindow.xaml
    /// </summary>
    public partial class FetchUpdateWindow : Window
    {
        public FetchUpdateWindow()
        {
            InitializeComponent();
            DataContext = new FetchUpdateViewModel();
        }
    }
}
