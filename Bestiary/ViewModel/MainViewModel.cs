using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bestiary.Model;

namespace Bestiary.ViewModel
{
    enum SortTypes
    {
        BondLevel,
        Alphabetical,
        HoardOrder
    }

    class MainViewModel : INotifyPropertyChanged
    {
        private IFamiliarProvider m_familiarFetcher;

        //Filters
        public OwnershipStatus[] AvailableOwnedStatus => ListEnumValues<OwnershipStatus>();
        public OwnershipStatus? SelectedOwnedStatus { get; set; }
        public BondingLevels[] AvailableBondingLevels => ListEnumValues<BondingLevels>();
        public BondingLevels? SelectedBondingLevel { get; set; }
        public LocationTypes[] AvailableLocationTypes => ListEnumValues<LocationTypes>();
        public LocationTypes? SelectedLocationType { get; set; }
        public Availabilities[] AvailableAvailabilities => ListEnumValues<Availabilities>();
        public Availabilities? SelectedAvailabilities { get; set; }
        public Sources[] AvailableSources => ListEnumValues<Sources>();
        private Sources? m_SelectedSource;
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
                            new SubFilter<string, Coliseum>("Venues", AvailableVenueNames, SelectedVenueName, c => c.VenueName),
                            new EnumSubFilter<EnemyTypes, Coliseum>("Enemy Type", AvailableEnemyTypes, SelectedEnemyType, c => c.EnemyType),
                        };
                        break;
                }
            }
        }

        //SubFilters
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
        public int[] AvailableCycleYears
        {
            get
            {
                return AvailableCycleYears;
            }
            set
            {
                int years = (int)Math.Floor(DateTime.Now.Subtract(new DateTime(2013, 6, 8)).TotalDays / 365.25);
                AvailableCycleYears = Enumerable.Range(1, years).ToArray();
            }
        }

        //Sorting
        public SortTypes[] AvailableSortTypes => ListEnumValues<SortTypes>();
        public SortTypes? SelectedSortType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public FamiliarInfo[] FilteredFamiliars { get; private set; }

        private LambdaCommand m_FetchFamiliars;

        public ICommand FetchFamiliars
        {
            get
            {
                if (m_FetchFamiliars == null)
                {
                    m_FetchFamiliars = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            var tempFamiliars = ApplyFilters(m_familiarFetcher.FetchFamiliars());
                            foreach (var subFilter in SubFilterList)
                            {
                                tempFamiliars = subFilter.Apply(tempFamiliars);
                            }
                            FilteredFamiliars = ApplySort(tempFamiliars).ToArray();
                        }
                    );
                }
                return m_FetchFamiliars;
            }
        }

        private LambdaCommand m_ClearOption;
        public ICommand ClearOption
        {
            get
            {
                if (m_ClearOption == null)
                {
                    m_ClearOption = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            switch (p)
                            {
                                case bool b:
                                    SelectedOwnedStatus = null;
                                    break;
                                case BondingLevels bl:
                                    SelectedBondingLevel = null;
                                    break;
                                case Sources s:
                                    SelectedSource = null;
                                    break;
                                case Availabilities a:
                                    SelectedAvailabilities = null;
                                    break;
                                case SortTypes st:
                                    SelectedSortType = null;
                                    break;
                                default:
                                    break;
                            }
                        }
                    );
                }
                return m_ClearOption;
            }
        }

        private IEnumerable<FamiliarInfo> ApplyFilters(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> filteredFamiliars = familiars;

            if (SelectedBondingLevel != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.BondLevel == SelectedBondingLevel);
            }
            if (SelectedOwnedStatus != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.Owned == SelectedOwnedStatus);
            }
            if (SelectedAvailabilities != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Availability == SelectedAvailabilities);
            }
            if (SelectedSource != null)
            {
                var map = new Dictionary<Sources, Type>
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
                var lookingFor = map[SelectedSource.Value];
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Source.GetType() == lookingFor);
            }

            return filteredFamiliars;
        }

        private IEnumerable<FamiliarInfo> ApplySubFilters(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> filteredFamiliars = familiars;

            switch (SelectedSource)
            {
                case Sources.Coliseum:
                    if (SelectedVenueName != null)
                    {
                        filteredFamiliars = filteredFamiliars.Where(f => ((Coliseum)f.Familiar.Source).VenueName == SelectedVenueName);
                    }
                    if (SelectedEnemyType != null)
                    {
                        filteredFamiliars = filteredFamiliars.Where(f => ((Coliseum)f.Familiar.Source).EnemyType == SelectedEnemyType);
                    }
                    break;
                case Sources.Event:
                    if (SelectedSiteEvent != null)
                    {
                        filteredFamiliars = filteredFamiliars.Where(f => ((SiteEvent)f.Familiar.Source).EventName == SelectedSiteEvent);
                    }
                    break;
            }

            return filteredFamiliars;
        }

        private IEnumerable<FamiliarInfo> ApplySort(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> sortedFamiliars = familiars;

            switch (SelectedSortType)
            {
                case SortTypes.Alphabetical:
                    sortedFamiliars = sortedFamiliars.OrderBy(f => f.Familiar.Name);
                    break;
                case SortTypes.BondLevel:
                    sortedFamiliars = sortedFamiliars.OrderBy(f => f.BondLevel);
                    break;
                case SortTypes.HoardOrder:
                    sortedFamiliars = sortedFamiliars.OrderBy(f => f.Familiar.Id);
                    break;
            }
            return sortedFamiliars;
        }

        public MainViewModel(IFamiliarProvider familiarFetcher)
        {
            m_familiarFetcher = familiarFetcher;
        }

        public MainViewModel()
        {
        }

        private T[] ListEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .ToArray();
        }


        public List<IAmSubFilter> SubFilterList { get; set; } = new List<IAmSubFilter>();
    }

    interface IAmSubFilter
    {
        IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter);
    }

    class SubFilter<T, SourceType> : IAmSubFilter
        where T : class
        where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }
        public T SelectedOption { get; set; }

        public SubFilter(string name, T[] availableOptions, T selectedOption, Func<SourceType, T> getKey)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
        }

        public IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter)
        {
            if (SelectedOption == null)
            {
                return toFilter;
            }

            return toFilter
                .Where(f =>
                {
                    var source = f.Familiar.Source as SourceType;
                    if (source == null)
                    {
                        return false;
                    }

                    var key = m_GetKey(source);
                    var matches = key.Equals(SelectedOption);

                    return matches;
                });
        }

        private Func<SourceType, T> m_GetKey;
    }

    class EnumSubFilter<T, SourceType> : IAmSubFilter
        where T : struct
        where SourceType : class
    {
        public string Name { get; set; }
        public T[] AvailableOptions { get; set; }
        public T? SelectedOption { get; set; }

        public EnumSubFilter(string name, T[] availableOptions, T? selectedOption, Func<SourceType, T> getKey)
        {
            Name = name;
            AvailableOptions = availableOptions;
            SelectedOption = selectedOption;
            m_GetKey = getKey;
        }

        public IEnumerable<FamiliarInfo> Apply(IEnumerable<FamiliarInfo> toFilter)
        {
            if (SelectedOption == null)
            {
                return toFilter;
            }

            return toFilter
                .Where(f =>
                {
                    var source = f.Familiar.Source as SourceType;
                    if (source == null)
                    {
                        return false;
                    }

                    var key = m_GetKey(source);
                    var matches = key.Equals(SelectedOption);

                    return matches;
                });
        }

        private Func<SourceType, T> m_GetKey;
    }
}