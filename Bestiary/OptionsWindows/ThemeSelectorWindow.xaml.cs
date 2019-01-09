using Bestiary.Services;
using Bestiary.ViewModel.OptionsViews;
using System.Windows;

namespace Bestiary.OptionsWindows
{
    /// <summary>
    /// Interaction logic for ThemeSelectorWindow.xaml
    /// </summary>
    public partial class ThemeSelectorWindow : Window
    {
        public ThemeSelectorWindow(SettingsHandler settings)
        {
            InitializeComponent();
            DataContext = new ThemeSelectorViewModel(settings);
        }
    }
}
