using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public OwnershipStatus[] AvailableOwnedStatus => ListEnumValues< OwnershipStatus>();
        public OwnershipStatus? SelectedOwnedStatus { get; set; }
        public BondingLevels[] AvailableBondingLevels => ListEnumValues<BondingLevels>();
        public BondingLevels? SelectedBondingLevel { get; set; }
        public Sources[] AvailableSources => ListEnumValues<Sources>();
        public Sources? SelectedSource { get; set; }
        public Availabilities[] AvailableAvailabilities => ListEnumValues<Availabilities>();
        public Availabilities? SelectedAvailabilities { get; set; }
        public SortTypes[] AvailableSortTypes => ListEnumValues<SortTypes>();
        public SortTypes? SelectedSortType { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public FamiliarInfo[] FilteredFamiliars { get; private set; }

        private LambdaCommand m_FetchFamiliars;

        public ICommand FetchFamiliars
        {
            get
            {
                if(m_FetchFamiliars == null)
                {
                    m_FetchFamiliars = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            var tempFamiliars = ApplyFilters(m_familiarFetcher.FetchFamiliars());
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
                if(m_ClearOption == null)
                {
                    m_ClearOption = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            switch(p)
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

            if(SelectedBondingLevel != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.BondLevel == SelectedBondingLevel);
            }
            if(SelectedOwnedStatus != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.Owned == SelectedOwnedStatus);
            }
            if(SelectedAvailabilities != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Availability == SelectedAvailabilities);
            }
            if(SelectedSource != null)
            {
                var map = new Dictionary<Sources, Type>
                {
                    {  Sources.Coliseum, typeof(Coliseum) },
                    { Sources.Event, typeof(SiteEvent) },
                    { Sources.Festival, typeof(Festival) },
                    { Sources.Gathering, typeof(Gathering) },
                    { Sources.Joxar, typeof(JoxarSpareInventory) },
                    { Sources.Marketplace, typeof(Marketplace) }
                };
                var lookingFor = map[SelectedSource.Value];
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Source.GetType() == lookingFor);
            }

            return filteredFamiliars;
        }

        private IEnumerable<FamiliarInfo> ApplySort(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> sortedFamiliars = familiars;

            switch(SelectedSortType)
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
    }
}
