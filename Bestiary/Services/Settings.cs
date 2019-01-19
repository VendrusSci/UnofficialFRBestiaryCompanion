using Bestiary.Model;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace Bestiary.Services
{
    public class SettingsHandler
    {
        public Theme Dark = new Theme()
        {
            BackgroundColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#555555")),
            ResultWindowColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#444444")),
            ResultWindowAltColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#525252")),
            ControlColour = Brushes.Gray,
            ControlHoverColour = Brushes.DarkGray,
            MenuColour = Brushes.Gray,
            TextColour = Brushes.White,
            MenuTextColour = Brushes.White,
            BorderColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#444444")),
            TextDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#A1A1A1")),
            BorderDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#444444")),
            ControlDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#666666"))
        };          

        public Theme Default = new Theme()
        {
            BackgroundColour = Brushes.White,
            ResultWindowColour = Brushes.White,
            ResultWindowAltColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FBE9D9")),
            ControlColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0")),
            ControlHoverColour = Brushes.LightGray,
            MenuColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F0F0F0")),
            TextColour = Brushes.Black,
            MenuTextColour = Brushes.Black,
            BorderColour = Brushes.Gray,
            TextDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#A3A3A3")),
            BorderDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#A3A3A3")),
            ControlDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#D6D6D6"))
        };

        public Theme Official = new Theme()
        {
            BackgroundColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#dbd6c7")),
            ResultWindowColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#f1eee4")),
            ResultWindowAltColour = Brushes.White,
            ControlColour = Brushes.White,
            ControlHoverColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DECB9C")),
            MenuColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6E1C02")),
            TextColour = Brushes.Black,
            MenuTextColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DECB9C")),
            BorderColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6E1C02")),
            TextDisabledColour = Brushes.DarkGray,
            BorderDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#BFBFBF")),
            ControlDisabledColour = (SolidColorBrush)(new BrushConverter().ConvertFrom("#EFECE6"))
        };


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
                    Year = null,
                    MinLevel = null,
                };
            }
        }

        public void FetchSettings()
        {
            if(File.Exists(ApplicationPaths.GetSettingsPath()))
            {
                string settingsText = File.ReadAllText(ApplicationPaths.GetSettingsPath());

                Settings settings = new Settings();
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(settingsText));
                DataContractJsonSerializer ser = new DataContractJsonSerializer(settings.GetType());
                settings = ser.ReadObject(ms) as Settings;
                ms.Close();

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

            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(Settings));
            ser.WriteObject(ms, settings);
            File.WriteAllBytes(ApplicationPaths.GetSettingsPath(), ms.ToArray());
            ms.Close();
        }
    }

    [DataContract]
    public class Settings
    {
        [DataMember]
        public Theme SelectedTheme { get; set; }
        [DataMember]
        public DefaultSearch SelectedDefaultSearch { get; set; }
        [DataMember]
        public string BackupSpreadsheetId { get; set; }
    }

    [DataContract]
    [KnownType(typeof(SolidColorBrush))]
    [KnownType(typeof(System.Windows.Media.MatrixTransform))]
    public class Theme
    {
        [DataMember]
        public SolidColorBrush BackgroundColour { get; set; }
        [DataMember]
        public SolidColorBrush ResultWindowColour { get; set; }
        [DataMember]
        public SolidColorBrush ResultWindowAltColour { get; set; }
        [DataMember]
        public SolidColorBrush ControlColour { get; set; }
        [DataMember]
        public SolidColorBrush ControlHoverColour { get; set; }
        [DataMember]
        public SolidColorBrush MenuColour { get; set; }
        [DataMember]
        public SolidColorBrush TextColour { get; set; }
        [DataMember]
        public SolidColorBrush MenuTextColour { get; set; }
        [DataMember]
        public SolidColorBrush BorderColour { get; set; }
        [DataMember]
        public SolidColorBrush TextDisabledColour { get; set; }
        [DataMember]
        public SolidColorBrush BorderDisabledColour { get; set; }
        [DataMember]
        public SolidColorBrush ControlDisabledColour { get; set; }
    }

    [DataContract]
    public class DefaultSearch
    {
        [DataMember]
        public OwnershipStatus? Ownership { get; set; }
        [DataMember]
        public SpecialState? Special { get; set; }
        [DataMember]
        public BondingLevels? BondLevel { get; set; }
        [DataMember]
        public bool BondLevelExclude { get; set; }
        [DataMember]
        public LocationTypes? Location { get; set; }
        [DataMember]
        public bool LocationExclude { get; set; }
        [DataMember]
        public Availabilities? Availability { get; set; }
        [DataMember]
        public bool AvailabilityExclude { get; set; }
        [DataMember]
        public Sources? Source { get; set; }
        [DataMember]
        public string VenueName { get; set; }
        [DataMember]
        public EnemyTypes? EnemyType { get; set; }
        [DataMember]
        public MarketPlaceTypes? MarketPlace { get; set; }
        [DataMember]
        public Flights? Flight { get; set; }
        [DataMember]
        public int? Year { get; set; }
        [DataMember]
        public string EventName { get; set; }
        [DataMember]
        public GatherTypes? GatherType { get; set; }
        [DataMember]
        public int? MinLevel { get; set; }
        [DataMember]
        public string SearchText { get; set; }
    }

    class BookmarkGroups
    {

    }
}
