using Bestiary.ViewModel;
using System.Windows;

namespace Bestiary.OptionsWindows
{
    /// <summary>
    /// Interaction logic for ImportFromOldWindow.xaml
    /// </summary>
    public partial class ImportFromOldWindow : Window
    {
        public ImportFromOldWindow()
        {
            InitializeComponent();

            DataContext = new ImportFromOldViewModel();
        }
    }
}
