using Bestiary.Model;
using Bestiary.Services;
using Bestiary.ViewModel;
using System.Windows;

namespace Bestiary.OptionsWindows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(FamiliarFilters familiarFilters, string searchText, SettingsHandler settings)
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(familiarFilters, searchText, settings);
        }
    }
}
