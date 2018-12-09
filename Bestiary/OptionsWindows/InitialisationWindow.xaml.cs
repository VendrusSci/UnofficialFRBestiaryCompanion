using Bestiary.Model;
using Bestiary.ViewModel;
using System.Windows;


namespace Bestiary
{
    /// <summary>
    /// Interaction logic for InitialisationWindow.xaml
    /// </summary>
    public partial class InitialisationWindow : Window
    {
        public InitialisationWindow(IModel model)
        {
            InitializeComponent();
            DataContext = new InitialisationViewModel(model);
        }
    }
}
