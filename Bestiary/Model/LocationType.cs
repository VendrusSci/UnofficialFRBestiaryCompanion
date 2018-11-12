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

    enum Availabilities
    {
        Always,
        Festival,
        Event,
        Cycling,
        Retired
    }

    enum BondingLevels
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

    enum OwnershipStatus
    {
        Owned,
        NotOwned
    }

    enum LocationTypes
    {
        OnDragon,
        InHoard,
        InVault
    }

    #endregion

    interface IFamiliarSource {}

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
        public Gathering(Flights flight, GatherTypes gatherType, int minLevel)
        {
            Flight = flight;
            GatherType = gatherType;
            MinLevel = minLevel;
        }
        [DataMember]
        public Flights Flight { get; private set; }
        [DataMember]
        public GatherTypes GatherType { get; private set; }
        [DataMember]
        public int MinLevel { get; private set; }
    }

    [DataContract]
    class Baldwin : IFamiliarSource
    {
        public Baldwin(RecipeItem[] recipe)
        {
            Recipe = recipe;
        }
        [DataMember]
        public RecipeItem[] Recipe { get; set; }
    }

    [DataContract]
    class Swipp : IFamiliarSource
    {
        public Swipp(RecipeItem item1, RecipeItem item2)
        {
            Recipe[0] = item1;
            Recipe[1] = item2;
        }
        [DataMember]
        public RecipeItem[] Recipe { get; set; }
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

    [DataContract]
    class RecipeItem
    {
        public RecipeItem(string name, int count)
        {
            Name = name;
            Count = count;
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Count { get; set; }
    }

}

