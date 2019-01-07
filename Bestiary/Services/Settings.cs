using Bestiary.Model;
using Newtonsoft.Json;
using System.Drawing;
using System.IO;
using System.Text;

namespace Bestiary.Services
{
    public class SettingsHandler
    {
        private static Theme m_Dark = new Theme(Color.DarkGray, Color.DarkSlateGray, Color.Gray, Color.Gray, Color.Gray, Color.White, Color.White);
        private static Theme m_Rainbow = new Theme(Color.LightCyan, Color.LightGreen, Color.LightSkyBlue, Color.LightYellow, Color.LightPink, Color.Black, Color.Black);
        private static Theme m_Bluegreen = new Theme(Color.Aquamarine, Color.Aqua, Color.Cyan, Color.LightCyan, Color.LightCyan, Color.DarkBlue, Color.DarkBlue);
        private static Theme m_Default = new Theme(Color.White, Color.White, ColorTranslator.FromHtml("#FBE9D9"), 
            ColorTranslator.FromHtml("#F0F0F0"), ColorTranslator.FromHtml("#F0F0F0"), Color.Black, Color.Black);
        private static Theme m_Official = new Theme(ColorTranslator.FromHtml("#6E1C02"), ColorTranslator.FromHtml("#DECB9C"), Color.White, Color.White, 
            ColorTranslator.FromHtml("#6E1C02"), Color.Black, ColorTranslator.FromHtml("#DECB9C"));

        public Theme SelectedTheme { get; set; }
        public DefaultSearch SelectedDefaultSearch { get; set; }
        public string BackupSpreadsheetId { get; set; }

        public SettingsHandler(Theme theme = null, DefaultSearch defaultSearch = null)
        {
            if(theme == null)
            {
                SelectedTheme = m_Default;
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
        public Color BackgroundColour { get; private set; }
        public Color ResultWindowColour { get; private set; }
        public Color ResultWindowAltColour { get; private set; }
        public Color ControlColour { get; private set; }
        public Color MenuColour { get; private set; }
        public Color TextColour { get; private set; }
        public Color MenuTextColour { get; private set; }

        public Theme(Color backgroundColour, Color resultWindowColour, Color resultWindowAltColour,
            Color controlColour, Color menuColour, Color textColour, Color menuTextColour)
        {
            BackgroundColour = backgroundColour;
            ResultWindowColour = resultWindowColour;
            ResultWindowAltColour = resultWindowColour;
            ControlColour = controlColour;
            MenuColour = menuColour;
            TextColour = textColour;
            MenuTextColour = menuTextColour;
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
