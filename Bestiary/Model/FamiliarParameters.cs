using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    class FamiliarParameters : INotifyPropertyChanged
    {
        //Filters
        public OwnershipStatus[] AvailableOwnedStatus => ListEnumValues<OwnershipStatus>();
        public OwnershipStatus? SelectedOwnedStatus { get; set; }
        public BondingLevels[] AvailableBondingLevels => ListEnumValues<BondingLevels>();
        public BondingLevels? SelectedBondingLevel { get; set; }
        public LocationTypes[] AvailableLocationTypes => ListEnumValues<LocationTypes>();
        public LocationTypes? SelectedLocationType { get; set; }
        public Availabilities[] AvailableAvailabilities => ListEnumValues<Availabilities>();
        public Availabilities? SelectedAvailability { get; set; }
        public Sources[] AvailableSources => ListEnumValues<Sources>();
        public Sources? m_SelectedSource;
        public Sources? SelectedSource
        {
            get
            {
                return m_SelectedSource;
            }
            set
            {
                m_SelectedSource = value;
                switch (m_SelectedSource)
                {
                    case Sources.Coliseum:
                        SubFilterList = new List<IAmSubFilter>
                        {
                            new SubFilter<string, Coliseum>("Venues", AvailableVenueNames, SelectedVenueName, c => c.VenueName, onSet: v => SelectedVenueName = v),
                            new EnumSubFilter<EnemyTypes, Coliseum>("Enemy Type", AvailableEnemyTypes, SelectedEnemyType, c => c.EnemyType, onSet: t => SelectedEnemyType = t),
                        };
                        break;
                    case Sources.Event:
                        SubFilterList = new List<IAmSubFilter>
                        {
                            new SubFilter<string, SiteEvent>("Events", AvailableSiteEvents, SelectedSiteEvent, e => e.EventName, onSet: n => SelectedSiteEvent = n),
                            new SubFilter<CycleYear, SiteEvent>("Year", AvailableCycleYears, SelectedCycleYear, e => e.Year, onSet: y => SelectedCycleYear = y),
                        };
                        break;
                    case Sources.Festival:
                        SubFilterList = new List<IAmSubFilter>
                        {
                            new EnumSubFilter<Flights, Festival>("Flights", AvailableFlights, SelectedFlight, f => f.Flight, onSet: e => SelectedFlight = e),
                            new SubFilter<CycleYear, SiteEvent>("Year", AvailableCycleYears, SelectedCycleYear, f => f.Year, onSet: y => SelectedCycleYear = y)
                        };
                        break;
                    case Sources.Gathering:
                        SubFilterList = new List<IAmSubFilter>
                        {
                            new EnumSubFilter<GatherTypes, Gathering>("Gather Type", AvailableGatherTypes, SelectedGatherType, g => g.GatherType, onSet: t => SelectedGatherType = t),
                            new EnumSubFilter<Flights, Gathering>("Flight", AvailableFlights, SelectedFlight, g => g.Flight, onSet: e => SelectedFlight = e),
                            new EnumSubFilter<int, Gathering>("Level", AvailableLevels, SelectedLevel, g => g.MinLevel, (a, b) => b >= a, onSet: l => SelectedLevel = l),
                        };
                        break;
                    case Sources.Baldwin:
                        SubFilterList = new List<IAmSubFilter>
                        {
                            new EnumSubFilter<int, Baldwin>("Level", AvailableLevels, SelectedLevel, b => b.MinLevel, (a, b) => b >=a, onSet: l => SelectedLevel = l),
                        };
                        break;
                    case Sources.Marketplace:
                        SubFilterList = new List<IAmSubFilter>
                        {
                            new EnumSubFilter<MarketPlaceTypes, Marketplace>("Currency", AvailableCurrencies, SelectedCurrency, m => m.Type, onSet: c => SelectedCurrency = c),
                        };
                        break;
                    default:
                        SubFilterList = new List<IAmSubFilter>();
                        break;
                }
            }
        }
        public Dictionary<Sources, Type> SourceMap;

        public event PropertyChangedEventHandler PropertyChanged;

        //SubFilters
        public List<IAmSubFilter> SubFilterList { get; set; } = new List<IAmSubFilter>();
        public string[] AvailableVenueNames => new Venues().VenueNames;
        public string SelectedVenueName { get; set; }
        public string[] AvailableSiteEvents => new SiteEvents().EventNames;
        public string SelectedSiteEvent { get; set; }
        public Flights[] AvailableFlights => ListEnumValues<Flights>();
        public Flights? SelectedFlight { get; set; }
        public GatherTypes[] AvailableGatherTypes => ListEnumValues<GatherTypes>();
        public GatherTypes? SelectedGatherType { get; set; }
        public MarketPlaceTypes[] AvailableMarketPlaceTypes => ListEnumValues<MarketPlaceTypes>();
        public MarketPlaceTypes? SelectedMarketPlaceType { get; set; }
        public EnemyTypes[] AvailableEnemyTypes => ListEnumValues<EnemyTypes>();
        public EnemyTypes? SelectedEnemyType { get; set; }
        public CycleYear[] AvailableCycleYears
        {
            get
            {
                int years = (int)Math.Floor(DateTime.Now.Subtract(new DateTime(2013, 6, 8)).TotalDays / 365.25);
                return Enumerable.Range(1, years)
                    .Select(year => new CycleYear(year))
                    .ToArray();
            }
        }
        public CycleYear SelectedCycleYear { get; set; }
        public int[] AvailableLevels => Enumerable.Range(1, 40).ToArray();
        public int? SelectedLevel { get; set; }
        public MarketPlaceTypes[] AvailableCurrencies => ListEnumValues<MarketPlaceTypes>();
        public MarketPlaceTypes? SelectedCurrency { get; set; }

        public T[] ListEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .ToArray();
        }

        public FamiliarParameters()
        {
            SourceMap = new Dictionary<Sources, Type>
                {
                    { Sources.Coliseum, typeof(Coliseum) },
                    { Sources.Event, typeof(SiteEvent) },
                    { Sources.Festival, typeof(Festival) },
                    { Sources.Gathering, typeof(Gathering) },
                    { Sources.Joxar, typeof(JoxarSpareInventory) },
                    { Sources.Marketplace, typeof(Marketplace) },
                    { Sources.Swipp, typeof(Swipp) },
                    { Sources.Baldwin, typeof(Baldwin) },
                };
        }
    }
}
