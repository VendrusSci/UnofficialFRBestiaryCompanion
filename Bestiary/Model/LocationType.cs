using System;
using System.Collections.Generic;
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
        Gathering
    }

    enum Availabilities
    {
        Always,
        Festival,
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
    class CycleYear
    {
        public CycleYear( int yearNumber)
        {
            YearNumber = yearNumber;
        }
        [DataMember]
        public int YearNumber { get; private set; }
    }
}

