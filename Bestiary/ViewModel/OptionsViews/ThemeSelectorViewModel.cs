using Bestiary.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace Bestiary.ViewModel.OptionsViews
{
    class ThemeSelectorViewModel : INotifyPropertyChanged
    {
        public ThemeDisplayer SelectedTheme { get; set; }
        private SettingsHandler m_Settings;
        public List<ThemeDisplayer> ThemeList { get; private set; }
        private ThemeDisplayer m_CurrentTheme;
        public bool SetEnabled => m_CurrentTheme == SelectedTheme;

        public ThemeSelectorViewModel(SettingsHandler settings)
        {
            m_Settings = settings;
            ThemeList = new List<ThemeDisplayer>();
            ThemeList.Add(new ThemeDisplayer(m_Settings.Default, "Default"));
            ThemeList.Add(new ThemeDisplayer(m_Settings.Dark, "Dark"));
            ThemeList.Add(new ThemeDisplayer(m_Settings.Official, "FR-Style"));
            //ThemeList.Add(new ThemeDisplayer(m_Settings.Bluegreen, "Blue-green"));
            //ThemeList.Add(new ThemeDisplayer(m_Settings.Rainbow, "Rainbow"));
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
                            m_CurrentTheme = SelectedTheme;
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
        public Theme Theme { get; set; }
        public string ThemeName { get; set; }

        public ThemeDisplayer(Theme theme, string themeName)
        {
            Theme = theme;
            ThemeName = themeName;
        }
    }
}
