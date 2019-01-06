using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Services
{
    class Settings
    {
        private Theme m_Dark = new Theme(Color.DarkGray, Color.DarkSlateGray, Color.Gray, Color.Gray, Color.Gray, Color.White);
        private Theme m_Rainbow = new Theme(Color.LightCyan, Color.LightGreen, Color.LightSkyBlue, Color.LightYellow, Color.LightPink, Color.Black);
        private Theme m_Bluegreen = new Theme(Color.Aquamarine, Color.Aqua, Color.Cyan, Color.LightCyan, Color.LightCyan, Color.DarkBlue);
        private Theme m_Official = null;

        public Theme SelectedTheme { get; set; }
        public DefaultSearch SelectedDefaultSearch { get; set; }
        public string BackupSpreadsheetId { get; set; }
    }


    class Theme
    {
        public Color BackgroundColour { get; private set; }
        public Color ResultWindowColour { get; private set; }
        public Color ResultWindowAltColour { get; private set; }
        public Color DropdownColour { get; private set; }
        public Color MenuColour { get; private set; }
        public Color TextColour { get; private set; }

        public Theme(Color backgroundColour, Color resultWindowColour, Color resultWindowAltColour,
            Color dropdownColour, Color menuColour, Color textColour)
        {
            BackgroundColour = backgroundColour;
            ResultWindowColour = resultWindowColour;
            ResultWindowAltColour = resultWindowColour;
            DropdownColour = dropdownColour;
            MenuColour = menuColour;
            TextColour = textColour;
        }
    }

    class DefaultSearch
    {
        public OwnershipStatus Ownership { get; private set; }
        public SpecialState Special { get; private set; }
        public BondingLevels BondLevel { get; private set; }
        public bool BondLevelExclude { get; private set; }
        public LocationTypes Location { get; private set; }
        public bool LocationExclude { get; private set; }
        public Availabilities Availability { get; private set; }
        public bool AvailabilityExclude { get; private set; }
        public string Source { get; private set; }
        public string VenueName { get; private set; }
        public EnemyTypes EnemyType { get; private set; }
        public MarketPlaceTypes MarketPlace { get; private set; }
        public Flights Flight { get; private set; }
        public int Year { get; private set; }
        public string EventName { get; private set; }
        public GatherTypes GatherType { get; private set; }
        public int MinLevel { get; private set; }

        public DefaultSearch(OwnershipStatus ownership, SpecialState special, BondingLevels bondLevel,
            bool bondLevelExclude, LocationTypes location, bool locationExclude, Availabilities availability,
            bool availabilityExclude, string source, string venueName, EnemyTypes enemyType,
            MarketPlaceTypes marketPlace, Flights flight, int year, string eventName,
            GatherTypes gatherType, int minLevel)
        {
            Ownership = ownership;
            Special = special;
            BondLevel = bondLevel;
            BondLevelExclude = bondLevelExclude;
            Location = location;
            LocationExclude = locationExclude;
            Availability = availability;
            AvailabilityExclude = availabilityExclude;
            Source = source;
            VenueName = venueName;
            EnemyType = enemyType;
            MarketPlace = marketPlace;
            Flight = flight;
            Year = year;
            EventName = eventName;
            GatherType = gatherType;
            MinLevel = minLevel;
        }
    }

    class BookmarkGroups
    {

    }
}
