using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bestiary.ViewModel;

namespace Bestiary.Model
{
    class FilterWrapper
    {
        public FilterWrapper(IAmSubFilter filter)
        {
            Filter = filter;
        }

        public IAmSubFilter Filter { get; private set; }
    }

    class FamiliarAddParameters : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public List<object> ParameterSelectorList { get; set; } = new List<object>();

        public GatherControlViewModel GatherControl { get; private set; } = new GatherControlViewModel();
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
                        ParameterSelectorList = new List<object>
                        {
                            new FilterWrapper(new SubFilter<string, Coliseum>("Venues", AvailableVenueNames, SelectedVenueName, c => c.VenueName, onSet: v => SelectedVenueName = v)),
                            new FilterWrapper(new EnumSubFilter<EnemyTypes, Coliseum>("Enemy Type", AvailableEnemyTypes, SelectedEnemyType, c => c.EnemyType, onSet: t => SelectedEnemyType = t)),
                        };
                        break;
                    case Sources.Event:
                        ParameterSelectorList = new List<object>
                        {
                            new FilterWrapper(new SubFilter<string, SiteEvent>("Events", AvailableSiteEvents, SelectedSiteEvent, e => e.EventName, onSet: n => SelectedSiteEvent = n)),
                            new FilterWrapper(new SubFilter<CycleYear, SiteEvent>("Year", AvailableCycleYears, SelectedCycleYear, e => e.Year, onSet: y => SelectedCycleYear = y)),
                        };
                        break;
                    case Sources.Festival:
                        ParameterSelectorList = new List<object>
                        {
                            new FilterWrapper(new EnumSubFilter<Flights, Festival>("Flights", AvailableFlights, SelectedFlight, f => f.Flight, onSet: e => SelectedFlight = e)),
                            new FilterWrapper(new SubFilter<CycleYear, SiteEvent>("Year", AvailableCycleYears, SelectedCycleYear, f => f.Year, onSet: y => SelectedCycleYear = y)),
                        };
                        break;
                    case Sources.Gathering:
                        ParameterSelectorList = new List<object>
                        {
                            new FilterWrapper(new EnumSubFilter<GatherTypes, Gathering>("Gather Type", AvailableGatherTypes, SelectedGatherType, g => g.GatherType, onSet: t => SelectedGatherType = t)),
                            new FilterWrapper(new EnumSubFilter<int, Gathering>("Level", AvailableLevels, SelectedLevel, g => g.MinLevel, (a, b) => b >= a, onSet: l => SelectedLevel = l)),
                            GatherControl,
                        };
                        break;
                    case Sources.Baldwin:
                        ParameterSelectorList = new List<object>
                        {
                            new FilterWrapper(new EnumSubFilter<int, Baldwin>("Level", AvailableLevels, SelectedLevel, b => b.MinLevel, (a, b) => b >=a, onSet: l => SelectedLevel = l)),
                        };
                        break;
                    case Sources.Marketplace:
                        ParameterSelectorList = new List<object>
                        {
                            new FilterWrapper(new EnumSubFilter<MarketPlaceTypes, Marketplace>("Currency", AvailableCurrencies, SelectedCurrency, m => m.Type, onSet: c => SelectedCurrency = c)),
                        };
                        break;
                    default:
                        ParameterSelectorList = new List<object>();
                        break;
                }
            }
        }


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
                int years = (int)Math.Ceiling(DateTime.Now.Subtract(new DateTime(2013, 6, 8)).TotalDays / 365.25);
                return Enumerable.Range(1, years+1)
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
    }
}
