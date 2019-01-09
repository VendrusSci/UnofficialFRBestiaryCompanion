using Bestiary.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Bestiary.ViewModel.OptionsViews
{
    class ThemeSelectorViewModel : INotifyPropertyChanged
    {
        public ThemeDisplayer SelectedTheme { get; set; }
        private SettingsHandler m_Settings;
        public List<ThemeDisplayer> ThemeList { get; private set; }

        public ThemeSelectorViewModel(SettingsHandler settings)
        {
            m_Settings = settings;
            ThemeList = new List<ThemeDisplayer>();
            ThemeList.Add(new ThemeDisplayer(m_Settings.Default, "Default"));
            ThemeList.Add(new ThemeDisplayer(m_Settings.Dark, "Dark"));
            ThemeList.Add(new ThemeDisplayer(m_Settings.Official, "FR-Style"));
            ThemeList.Add(new ThemeDisplayer(m_Settings.Bluegreen, "Blue-green"));
            ThemeList.Add(new ThemeDisplayer(m_Settings.Rainbow, "Rainbow"));
        }

        private LambdaCommand m_SaveTheme;
        public ICommand SaveTheme
        {
            get
            {
                if(m_SaveTheme == null)
                {
                    m_SaveTheme = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_Settings.SelectedTheme = SelectedTheme.Theme;
                            m_Settings.SaveSettings();
                        }
                    );
                }
                return m_SaveTheme;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ThemeDisplayer
    {
        public Theme Theme { get; private set; }
        public string ThemeName { get; private set; }

        public ThemeDisplayer(Theme theme, string themeName)
        {
            Theme = theme;
            ThemeName = themeName;
        }
    }
}
