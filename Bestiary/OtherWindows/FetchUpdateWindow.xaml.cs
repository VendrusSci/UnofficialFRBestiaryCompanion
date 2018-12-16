using Bestiary.Model;
using Bestiary.ViewModel;
using System.Windows;

namespace Bestiary
{
    /// <summary>
    /// Interaction logic for FetchUpdateWindow.xaml
    /// </summary>
    public partial class FetchUpdateWindow : Window
    {
        public FetchUpdateWindow(IModel model, string FRDataPath)
        {
            InitializeComponent();
            DataContext = new FetchUpdateViewModel(model, FRDataPath);
        }
    }
}
