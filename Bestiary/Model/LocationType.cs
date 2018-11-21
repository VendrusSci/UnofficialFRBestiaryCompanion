using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    #region Enums
    enum Flights
    {
        Earth,
        Fire,
        Water,
        Wind,
        Ice,
        Light,
        Shadow,
        Lightning,
        Plague,
        Nature,
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
        Event,
        Cycling,
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
        NotOwned
    }

    public enum LocationTypes
    {
        OnDragon,
        InHoard,
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
    class CycleYear
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
            VenueNames =  File.ReadAllLines("Venues.txt");
        }
        [DataMember]
        public string[] VenueNames { get; set; }
    }

    [DataContract]
    class SiteEvents
    {
        public SiteEvents()
        {
            EventNames = File.ReadAllLines("Events.txt");
        }
        [DataMember]
        public string[] EventNames { get; set; }
    }
}

