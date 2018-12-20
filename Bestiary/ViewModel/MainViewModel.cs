using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bestiary.Model;
using Bestiary.ViewWindows;

namespace Bestiary.ViewModel
{
    enum SortTypes
    {
        [Description("Bond Level")]
        BondLevel,
        [Description("Reverse Bond Level")]
        ReverseBondLevel,
        Alphabetical,
        [Description("Reverse Alphabetical")]
        ReverseAlphabetical,
        [Description("Hoard Order")]
        HoardOrder,
        [Description("Reverse Hoard Order")]
        ReverseHoardOrder
    }

    class MainViewModel : INotifyPropertyChanged
    {
        private IModel m_Model;
        public FamiliarFilters FamiliarParameters { get; set; }
        public BitmapImage Icon { get; private set; }

        //Sorting
        public SortTypes[] AvailableSortTypes => FamiliarParameters.ListEnumValues<SortTypes>();
        public SortTypes? SelectedSortType { get; set; }

        //Search
        public string SearchText { get; set; }
        public bool ExactChecked {get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public FamiliarViewModel[] FilteredFamiliars { get; private set; }

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
                            UserActionLog.Info("Familiar retrieval initiated - creating FamiliarInfo instances");
                            var familiars = m_Model.Familiars
                                .Select(id => m_Model.LookupFamiliar(id).Fetch())
                                .Select(familiar =>
                                {
                                    var owned = m_Model.LookupOwnedFamiliar(familiar.Id);
                                    var bookmarked = m_Model.LookupBookmarkedFamiliar(familiar.Id);
                                    return new FamiliarInfo(
                                        m_Model.LookupFamiliar(familiar.Id),
                                        owned,
                                        bookmarked
                                    );
                                });
                            UserActionLog.Info("Applying filters");
                            var tempFamiliars = ApplyFilters(familiars);
                            UserActionLog.Info("Applying subfilters");
                            foreach (var subFilter in FamiliarParameters.SubFilterList)
                            {
                                tempFamiliars = subFilter.Apply(tempFamiliars);
                            }
                            UserActionLog.Info("Applying search");
                            tempFamiliars = ApplySearch(tempFamiliars);
                            FilteredFamiliars = tempFamiliars
                                .Select(f => new FamiliarViewModel(m_Model, f, FamiliarParameters.AvailableLocationTypes))
                                .ToArray();
                            FilteredFamiliars = ApplySort(FilteredFamiliars);
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
                                case OwnershipStatus b:
                                    UserActionLog.Info("Owned filter cleared");
                                    FamiliarParameters.SelectedOwnedStatus = null;
                                    break;
                                case BookmarkState bs:
                                    UserActionLog.Info("Bookmark filter cleared");
                                    FamiliarParameters.SelectedBookmarkState = null;
                                    break;
                                case BondingLevels bl:
                                    UserActionLog.Info("Bonding filter cleared");
                                    FamiliarParameters.SelectedBondingLevel = null;
                                    FamiliarParameters.BondingLevelInvert = false;
                                    break;
                                case Sources s:
                                    UserActionLog.Info("Source filter cleared");
                                    FamiliarParameters.SelectedSource = null;
                                    break;
                                case Availabilities a:
                                    UserActionLog.Info("Availability filter cleared");
                                    FamiliarParameters.SelectedAvailability = null;
                                    FamiliarParameters.AvailabilityInvert = false;
                                    break;
                                case LocationTypes l:
                                    UserActionLog.Info("Location filter cleared");
                                    FamiliarParameters.SelectedLocationType = null;
                                    FamiliarParameters.LocationInvert = false;
                                    break;
                                default:
                                    break;
                            }
                            UserActionLog.Info("All subfilters cleared");
                            FamiliarParameters.SelectedCurrency = null;
                            FamiliarParameters.SelectedCycleYear = null;
                            FamiliarParameters.SelectedEnemyType = null;
                            FamiliarParameters.SelectedFlight = null;
                            FamiliarParameters.SelectedGatherType = null;
                            FamiliarParameters.SelectedLevel = null;
                            FamiliarParameters.SelectedMarketPlaceType = null;
                            FamiliarParameters.SelectedSiteEvent = null;
                            FamiliarParameters.SelectedVenueName = null;
                        },
                        onCanExecute: (p) =>
                        {
                            switch (p)
                            {
                                case OwnershipStatus b:
                                    return FamiliarParameters.SelectedOwnedStatus != null;
                                case BookmarkState bs:
                                    return FamiliarParameters.SelectedBookmarkState != null;
                                case BondingLevels bl:
                                    return FamiliarParameters.SelectedBondingLevel != null;
                                case Sources s:
                                    return FamiliarParameters.SelectedSource != null;
                                case Availabilities a:
                                    return FamiliarParameters.SelectedAvailability != null;
                                case SortTypes st:
                                    return SelectedSortType != null;
                                case LocationTypes l:
                                    return FamiliarParameters.SelectedLocationType != null;
                                default:
                                    return false;
                            }
                        }
                    );
                }
                return m_ClearOption;
            }
        }

        private LambdaCommand m_SortResults;
        public ICommand SortResults
        {
            get
            {
                if(m_SortResults == null)
                {
                    m_SortResults = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("Applying sort");
                            FilteredFamiliars = ApplySort(FilteredFamiliars);
                        }
                    );
                }
                return m_SortResults;
            }
        }

        public MainWindow Window { get; private set; }
        internal IModel Model { get => m_Model; set => m_Model = value; }

        public int ResultCount { get; set; }
        public int OwnedCount { get; set; }
        public int AwakenedCount { get; set; }
        private IEnumerable<FamiliarInfo> ApplyFilters(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> filteredFamiliars = familiars;
            UserActionLog.Info("Filtering executed:");
            if (FamiliarParameters.SelectedBondingLevel != null)
            {
                UserActionLog.Info($"    Filter: Bond level {FamiliarParameters.SelectedBondingLevel}");
                filteredFamiliars = FamiliarParameters.BondingLevelInvert ? filteredFamiliars.Where(f => f.BondLevel != FamiliarParameters.SelectedBondingLevel) : filteredFamiliars.Where(f => f.BondLevel == FamiliarParameters.SelectedBondingLevel);
            }
            if (FamiliarParameters.SelectedOwnedStatus != null)
            {
                UserActionLog.Info($"    Filter: Ownership status {FamiliarParameters.SelectedOwnedStatus}");
                filteredFamiliars = filteredFamiliars.Where(f => f.Owned == FamiliarParameters.SelectedOwnedStatus);
            }
            if (FamiliarParameters.SelectedBookmarkState != null)
            {
                UserActionLog.Info($"    Filter: Bookmark state {FamiliarParameters.SelectedBookmarkState}");
                filteredFamiliars = filteredFamiliars.Where(f => f.Bookmarked == FamiliarParameters.SelectedBookmarkState);
            }
            if (FamiliarParameters.SelectedLocationType != null)
            {
                UserActionLog.Info($"    Filter: Location {FamiliarParameters.SelectedLocationType.Value}");
                filteredFamiliars = FamiliarParameters.LocationInvert ? filteredFamiliars.Where(f => f.Location != FamiliarParameters.SelectedLocationType) : filteredFamiliars.Where(f => f.Location == FamiliarParameters.SelectedLocationType);
            }
            if (FamiliarParameters.SelectedAvailability != null)
            {
                UserActionLog.Info($"    Filter: Availability {FamiliarParameters.SelectedAvailability}");
                filteredFamiliars = FamiliarParameters.AvailabilityInvert ? filteredFamiliars.Where(f => f.Familiar.Availability != FamiliarParameters.SelectedAvailability) : filteredFamiliars.Where(f => f.Familiar.Availability == FamiliarParameters.SelectedAvailability);
            }
            if (FamiliarParameters.SelectedSource != null)
            {
                UserActionLog.Info($"    Filter: Source {FamiliarParameters.SelectedSource.Value}");
                var lookingFor = FamiliarParameters.SourceMap[FamiliarParameters.SelectedSource.Value];
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Source.GetType() == lookingFor);
            }

            ResultCount = filteredFamiliars.Count();
            OwnedCount = filteredFamiliars.Count(f => f.Owned == OwnershipStatus.Owned);
            AwakenedCount = filteredFamiliars.Count(f => f.BondLevel == BondingLevels.Awakened);

            return filteredFamiliars;
        }

        private LambdaCommand m_ClearAllFilters;
        public ICommand ClearAllFilters
        {
            get
            {
                if(m_ClearAllFilters == null)
                {
                    m_ClearAllFilters = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("All filters cleared");
                            FamiliarParameters.SelectedOwnedStatus = null;
                            FamiliarParameters.SelectedBookmarkState = null;
                            FamiliarParameters.SelectedBondingLevel = null;
                            FamiliarParameters.SelectedSource = null;
                            FamiliarParameters.SelectedAvailability = null;  
                            FamiliarParameters.SelectedLocationType = null;

                            FamiliarParameters.SelectedCurrency = null;
                            FamiliarParameters.SelectedCycleYear = null;
                            FamiliarParameters.SelectedEnemyType = null;
                            FamiliarParameters.SelectedFlight = null;
                            FamiliarParameters.SelectedGatherType = null;
                            FamiliarParameters.SelectedLevel = null;
                            FamiliarParameters.SelectedMarketPlaceType = null;
                            FamiliarParameters.SelectedSiteEvent = null;
                            FamiliarParameters.SelectedVenueName = null;

                            FamiliarParameters.AvailabilityInvert = false;
                            FamiliarParameters.BondingLevelInvert = false;
                            FamiliarParameters.LocationInvert = false;
                        }
                    );
                }
                return m_ClearAllFilters;
            }
        }

        private BaseCommand m_openAddFamiliarWindow;
        public ICommand OpenAddFamiliarWindow
        {
            get
            {
                if(m_openAddFamiliarWindow == null)
                {
                    UserActionLog.Info("Add Familiar window opened");
                    m_openAddFamiliarWindow = new OpenDialogCommand<FamiliarAddWindow>(
                        Window, 
                        _ => new FamiliarAddWindow(m_Model),
                        afterClosed: _ => FetchFamiliars.Execute(null)
                    );
                }
                return m_openAddFamiliarWindow;                
            }
        }

        private BaseCommand m_openDeleteFamiliarWindow;
        public ICommand OpenDeleteFamiliarWindow
        {
            get
            {
                UserActionLog.Info("Delete Familiar window opened");
                if (m_openDeleteFamiliarWindow == null)
                {
                    m_openDeleteFamiliarWindow = new OpenDialogCommand<FamiliarDeleteWindow>(
                        Window, 
                        _ => new FamiliarDeleteWindow(m_Model),
                        afterClosed: _ => FetchFamiliars.Execute(null)
                    );
                }
                return m_openDeleteFamiliarWindow;
            }
        }


        private BaseCommand m_openDataFamiliarWindow;
        public ICommand OpenDataFamiliarWindow
        {
            get
            {
                if(m_openDataFamiliarWindow == null)
                {
                    m_openDataFamiliarWindow = new OpenDialogCommand<FamiliarDataWindow>(
                        Window,
                        p => new FamiliarDataWindow((FamiliarViewModel)p, m_Model),
                        canExecute: p => p.GetType() == typeof(FamiliarViewModel),
                        afterClosed: _ => FetchFamiliars.Execute(null)
                    );
                }
                return m_openDataFamiliarWindow;
            }
        }

        private BaseCommand m_openSupportInfoWindow;
        public ICommand OpenSupportInfoWindow
        {
            get
            {
                if (m_openSupportInfoWindow == null)
                {
                    UserActionLog.Info("Support window opened");
                    m_openSupportInfoWindow = new OpenDialogCommand<SupportInfoWindow>(Window, _ => new SupportInfoWindow());
                }
                return m_openSupportInfoWindow;
            }
        }

        private BaseCommand m_openAboutWindow;
        public ICommand OpenAboutWindow
        {
            get
            {
                if (m_openAboutWindow == null)
                {
                    UserActionLog.Info("About window opened");
                    m_openAboutWindow = new OpenDialogCommand<AboutWindow>(Window, _ => new AboutWindow());
                }
                return m_openAboutWindow;
            }
        }

        private BaseCommand m_openFetchUpdateWindow;
        public ICommand OpenFetchUpdateWindow
        {
            get
            {
                if (m_openFetchUpdateWindow == null)
                {
                    UserActionLog.Info("Update window opened");
                    m_openFetchUpdateWindow = new OpenDialogCommand<FetchUpdateWindow>(Window, _ => new FetchUpdateWindow(m_Model, m_FRDataPath));
                }
                return m_openFetchUpdateWindow;
            }
        }

        private BaseCommand m_OpenInitialisationWindow;
        public ICommand OpenInitialisationWindow
        {
            get
            {
                if(m_OpenInitialisationWindow == null)
                {
                    UserActionLog.Info("Initialisation window opened");
                    m_OpenInitialisationWindow = new OpenDialogCommand<InitialisationWindow>(
                        Window, 
                        _ => new InitialisationWindow(m_Model),
                        afterClosed: _ => FetchFamiliars.Execute(null)
                    );
                }
                return m_OpenInitialisationWindow;
            }
        }

        private BaseCommand m_OpenResultListWindow;
        public ICommand OpenResultListWindow
        {
            get
            {
                if(m_OpenResultListWindow == null)
                {
                    UserActionLog.Info("Results List window opened");
                    m_OpenResultListWindow = new OpenDialogCommand<ResultListWindow>(
                        Window,
                        _ => new ResultListWindow(FilteredFamiliars)
                    );
                }
                return m_OpenResultListWindow;
            }
        }

        private BaseCommand m_OpenFRBestiaryView;
        public ICommand OpenFRBestiaryView
        {
            get
            {
                if(m_OpenFRBestiaryView == null)
                {
                    UserActionLog.Info("FR Bestiary view opened");
                    m_OpenFRBestiaryView = new OpenDialogCommand<FRBestiaryView>(
                        Window,
                        _ => new FRBestiaryView(m_Model, FamiliarParameters.AvailableOwnedStatus, FamiliarParameters.AvailableBondingLevels, FamiliarParameters.AvailableLocationTypes) 
                    );
                }
                return m_OpenFRBestiaryView;
            }
        }

        private BaseCommand m_OpenColiseumView;
        public ICommand OpenColiseumView
        {
            get
            {
                if(m_OpenColiseumView == null)
                {
                    m_OpenColiseumView = new OpenDialogCommand<ColiseumView>(
                        Window,
                        _ => new ColiseumView(m_Model, FamiliarParameters.AvailableOwnedStatus, FamiliarParameters.AvailableBondingLevels, FamiliarParameters.AvailableLocationTypes)
                    );
                }
                return m_OpenColiseumView;
            }
        }

        private FamiliarViewModel[] ApplySort(FamiliarViewModel[] familiars)
        {
            FamiliarViewModel[] sortedFamiliars = familiars;

            switch (SelectedSortType)
            {
                case SortTypes.Alphabetical:
                    sortedFamiliars = sortedFamiliars.OrderBy(f => f.Info.Familiar.Name, StringComparer.Ordinal).ToArray();
                    break;
                case SortTypes.ReverseAlphabetical:
                    sortedFamiliars = sortedFamiliars.OrderByDescending(f => f.Info.Familiar.Name, StringComparer.Ordinal).ToArray();
                    break;
                case SortTypes.BondLevel:
                    sortedFamiliars = sortedFamiliars.OrderBy(f => f.Info.BondLevel).ToArray();
                    break;
                case SortTypes.ReverseBondLevel:
                    sortedFamiliars = sortedFamiliars.OrderByDescending(f => f.Info.BondLevel).ToArray();
                    break;
                case SortTypes.HoardOrder:
                    sortedFamiliars = sortedFamiliars.OrderBy(f => f.Info.Familiar.Id).ToArray();
                    break;
                case SortTypes.ReverseHoardOrder:
                    sortedFamiliars = sortedFamiliars.OrderByDescending(f => f.Info.Familiar.Id).ToArray();
                    break;
            }
            return sortedFamiliars;
        }

        private IEnumerable<FamiliarInfo> ApplySearch(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> filteredFamiliars = familiars;
            if(!string.IsNullOrEmpty(SearchText))
            {
                var searchText = SearchText;
                if(!ExactChecked)
                {
                    var tempFamiliars = filteredFamiliars.Where(s => s.Familiar.Name.ToLower().Contains(searchText.ToLower()));
                    if(tempFamiliars.Count() == 0)
                    {
                        filteredFamiliars = filteredFamiliars.Where(s => EditDistance.GetEditDistance(s.Familiar.Name.ToLower(), searchText.ToLower()) <= 3);
                    }
                    else
                    {
                        filteredFamiliars = tempFamiliars;
                    }
                }
                else
                {
                    var tempFamiliars = filteredFamiliars.Where(s => s.Familiar.Name.ToLower().Equals(searchText.ToLower()));
                    if (tempFamiliars.Count() == 0)
                    {
                        filteredFamiliars = filteredFamiliars.Where(s => EditDistance.GetEditDistance(s.Familiar.Name.ToLower(), searchText.ToLower()) <= 3);
                    }
                    else
                    {
                        filteredFamiliars = tempFamiliars;
                    }
                }
            }
            SearchText = "";
            return filteredFamiliars;
        }

        private string m_FRDataPath;
        public MainViewModel(MainWindow window, IModel model, string FRDataPath)
        {
            Window = window;
            Model = model;
            m_FRDataPath = FRDataPath;

            UserActionLog.Info("Application opened!");
            FamiliarParameters = new FamiliarFilters();
            FetchFamiliars.Execute(null);
            SelectedSortType = SortTypes.Alphabetical;
            SortResults.Execute(null);
            UserActionLog.Info("Familiars loaded on open");

            try
            {
                var bestiaryIconPath = Path.Combine(ApplicationPaths.GetResourcesDirectory(), "bestiary.png");
                Icon = ImageLoader.LoadImage(bestiaryIconPath);
            }
            catch (FileNotFoundException)
            {
                UserActionLog.Error("Bestiary file not found");
                // :(
            }

        }

        public static readonly log4net.ILog UserActionLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}