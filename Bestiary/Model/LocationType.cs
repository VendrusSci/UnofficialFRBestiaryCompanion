
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Bestiary.Model
{
    #region Enums
    enum Flights
    {
        [Description("Earth")]
        Earth,
        [Description("Fire")]
        Fire,
        [Description("Water")]
        Water,
        [Description("Wind")]
        Wind,
        [Description("Ice")]
        Ice,
        [Description("Light")]
        Light,
        [Description("Shadow")]
        Shadow,
        [Description("Lightning")]
        Lightning,
        [Description("Plague")]
        Plague,
        [Description("Nature")]
        Nature,
        [Description("Arcane")]
        Arcane
    }

    enum Sources
    {
        Coliseum,
        Marketplace,
        Joxar,
        Event,
        Festival,
        Gathering,
        Baldwin,
        Swipp
    }

    public enum Availabilities
    {
        Always,
        Festival,
        [Description("Night of the Nocturne")]
        Notn,
        Cycling,
        [Description("Joxar's Spare Inventory")]
        JoxarBox,
        Retired
    }

    public enum BondingLevels
    {
        Wary,
        Tolerant,
        Relaxed,
        Inquisitive,
        Companion,
        Loyal,
        Awakened
    }

    enum GatherTypes
    {
        Hunting,
        Fishing,
        [Description("Insect Catching")]
        InsectCatching,
        Foraging,
        Digging,
        Scavenging
    }

    enum MarketPlaceTypes
    {
        Treasure,
        Gem
    }

    enum EnemyTypes
    {
        Boss,
        Normal
    }

    public enum OwnershipStatus
    {
        Owned,
        [Description("Not Owned")]
        NotOwned
    }

    public enum LocationTypes
    {
        [Description("On Dragon")]
        OnDragon,
        [Description("In Hoard")]
        InHoard,
        [Description("In Vault")]
        InVault
    }

    #endregion

    public interface IFamiliarSource {}

    [DataContract]
    class Coliseum : IFamiliarSource
    {
        public Coliseum(string venueName, EnemyTypes enemyType)
        {
            VenueName = venueName;
            EnemyType = enemyType;
        }
        [DataMember]
        public string VenueName { get; private set; }
        [DataMember]
        public EnemyTypes EnemyType { get; private set; }
    }

    [DataContract]
    class Marketplace : IFamiliarSource
    {
        public Marketplace(MarketPlaceTypes type)
        {
            Type = type;
        }
        [DataMember]
        public MarketPlaceTypes Type { get; private set; }
    }

    [DataContract]
    class Festival : IFamiliarSource
    {
        public Festival(Flights flight, CycleYear year)
        {
            Flight = flight;
            Year = year;
        }
        [DataMember]
        public Flights Flight { get; private set; }
        [DataMember]
        public CycleYear Year { get; private set; }
    }

    [DataContract]
    class JoxarSpareInventory : IFamiliarSource {}

    [DataContract]
    class SiteEvent : IFamiliarSource
    {
        public SiteEvent(string eventName, CycleYear year)
        {
            EventName = eventName;
            Year = year;
        }
        [DataMember]
        public string EventName { get; private set; }
        [DataMember]
        public CycleYear Year { get; private set; }
    }

    [DataContract]
    class Gathering : IFamiliarSource
    {
        public Gathering(List<Flights> flights, GatherTypes gatherType, int minLevel)
        {
            Flights = flights;
            GatherType = gatherType;
            MinLevel = minLevel;
        }
        [DataMember]
        public List<Flights> Flights { get; private set; }
        [DataMember]
        public GatherTypes GatherType { get; private set; }
        [DataMember]
        public int MinLevel { get; private set; }
    }

    [DataContract]
    class Baldwin : IFamiliarSource
    {
        public Baldwin(int minLevel)
        {
            MinLevel = minLevel;
        }
        [DataMember]
        public int MinLevel { get; private set; }
    }

    [DataContract]
    class Swipp : IFamiliarSource
    {
    }

    [DataContract]
    public class CycleYear
    {
        public CycleYear( int yearNumber)
        {
            YearNumber = yearNumber;
        }
        [DataMember]
        public int YearNumber { get; private set; }
        public override string ToString()
        {
            return YearNumber.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            var other = obj as CycleYear;
            if (other == null) return false;
            return YearNumber == other.YearNumber;
        }
    }

    [DataContract]
    class Venues
    {
        public Venues()
        {
            VenueNames =  File.ReadAllLines(Path.Combine(ApplicationPaths.GetResourcesDirectory(), "Venues.txt"));
        }
        [DataMember]
        public string[] VenueNames { get; set; }
    }

    [DataContract]
    class SiteEvents
    {
        public SiteEvents()
        {
            EventNames = File.ReadAllLines(Path.Combine(ApplicationPaths.GetResourcesDirectory(), "Events.txt"));
        }
        [DataMember]
        public string[] EventNames { get; set; }
    }
}

