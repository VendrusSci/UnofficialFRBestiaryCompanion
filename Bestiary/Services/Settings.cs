using Bestiary.Model;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Windows.Media;

namespace Bestiary.Services
{
    public class SettingsHandler
    {
        public Theme Dark = new Theme(Brushes.DimGray, (SolidColorBrush)(new BrushConverter().ConvertFrom("#444444")), (SolidColorBrush)(new BrushConverter().ConvertFrom("#525252")), Brushes.Gray, Brushes.DarkGray, Brushes.Gray, Brushes.White, Brushes.LightGray, (SolidColorBrush)(new BrushConverter().ConvertFrom("#444444")));
        public Theme Rainbow = new Theme(Brushes.PaleTurquoise, Brushes.LightCyan, Brushes.LightSkyBlue, Brushes.LightYellow, Brushes.LightSalmon, Brushes.LightPink, Brushes.Green, Brushes.Purple, Brushes.GreenYellow);
        public Theme Bluegreen = new Theme(Brushes.Aquamarine, Brushes.LightBlue, Brushes.Cyan, Brushes.LightCyan, Brushes.LightGreen, Brushes.LightCyan, Brushes.DarkBlue, Brushes.DarkBlue, Brushes.SeaGreen);
        public Theme Default = new Theme(Brushes.White, Brushes.White, (SolidColorBrush)(new BrushConverter().ConvertFrom("#FBE9D9")), (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0")), 
            Brushes.LightGray, (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0")), Brushes.Black, Brushes.Black, Brushes.Gray);
        public Theme Official = new Theme((SolidColorBrush)(new BrushConverter().ConvertFrom("#F4F4F4")), (SolidColorBrush)(new BrushConverter().ConvertFrom("#dbd6c7")), 
            Brushes.White, Brushes.White, (SolidColorBrush)(new BrushConverter().ConvertFrom("#DECB9C")), (SolidColorBrush)(new BrushConverter().ConvertFrom("#6E1C02")), Brushes.Black, 
            (SolidColorBrush)(new BrushConverter().ConvertFrom("#DECB9C")), (SolidColorBrush)(new BrushConverter().ConvertFrom("#6E1C02")));

        public Theme SelectedTheme { get; set; }
        public DefaultSearch SelectedDefaultSearch { get; set; }
        public string BackupSpreadsheetId { get; set; }

        public SettingsHandler(Theme theme = null, DefaultSearch defaultSearch = null)
        {
            if(theme == null)
            {
                SelectedTheme = Default;
            }

            if(defaultSearch == null)
            {
                SelectedDefaultSearch = new DefaultSearch
                {
                    BondLevelExclude = false,
                    LocationExclude = false,
                    AvailabilityExclude = false,
                    Year = 0,
                    MinLevel = 0,
                };
            }
        }

        public void FetchSettings()
        {
            if(File.Exists(ApplicationPaths.GetSettingsPath()))
            {
                string settingsText = File.ReadAllText(ApplicationPaths.GetSettingsPath());
                var settings = JsonConvert.DeserializeObject<Settings>(settingsText);
                BackupSpreadsheetId = settings.BackupSpreadsheetId;
                SelectedDefaultSearch = settings.SelectedDefaultSearch;
                SelectedTheme = settings.SelectedTheme;
            }
        }

        public void SaveSettings()
        {
            var settings = new Settings
            {
                BackupSpreadsheetId = BackupSpreadsheetId,
                SelectedDefaultSearch = SelectedDefaultSearch,
                SelectedTheme = SelectedTheme
            };

            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllBytes(ApplicationPaths.GetSettingsPath(), Encoding.ASCII.GetBytes(json));
        }
    }

    public class Settings
    {
        public Theme SelectedTheme { get; set; }
        public DefaultSearch SelectedDefaultSearch { get; set; }
        public string BackupSpreadsheetId { get; set; }
    }


    public class Theme
    {
        public SolidColorBrush BackgroundColour { get; private set; }
        public SolidColorBrush ResultWindowColour { get; private set; }
        public SolidColorBrush ResultWindowAltColour { get; private set; }
        public SolidColorBrush ControlColour { get; private set; }
        public SolidColorBrush ControlHoverColour { get; private set; }
        public SolidColorBrush MenuColour { get; private set; }
        public SolidColorBrush TextColour { get; private set; }
        public SolidColorBrush MenuTextColour { get; private set; }
        public SolidColorBrush BorderColour { get; private set; }

        public Theme(SolidColorBrush backgroundColour, SolidColorBrush resultWindowColour, SolidColorBrush resultWindowAltColour,
            SolidColorBrush controlColour, SolidColorBrush controlHoverColour, SolidColorBrush menuColour, SolidColorBrush textColour, SolidColorBrush menuTextColour, SolidColorBrush borderColour)
        {
            BackgroundColour = backgroundColour;
            ResultWindowColour = resultWindowColour;
            ResultWindowAltColour = resultWindowAltColour;
            ControlColour = controlColour;
            ControlHoverColour = controlHoverColour;
            MenuColour = menuColour;
            TextColour = textColour;
            MenuTextColour = menuTextColour;
            BorderColour = borderColour;
        }
    }

    public class DefaultSearch
    {
        public OwnershipStatus? Ownership;
        public SpecialState? Special;
        public BondingLevels? BondLevel;
        public bool BondLevelExclude;
        public LocationTypes? Location;
        public bool LocationExclude;
        public Availabilities? Availability;
        public bool AvailabilityExclude;
        public Sources? Source;
        public string VenueName;
        public EnemyTypes? EnemyType;
        public MarketPlaceTypes? MarketPlace;
        public Flights? Flight;
        public int? Year;
        public string EventName;
        public GatherTypes? GatherType;
        public int? MinLevel;
        public string SearchText;
    }

    class BookmarkGroups
    {

    }
}
