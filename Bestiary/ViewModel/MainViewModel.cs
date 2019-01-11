using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bestiary.Model;
using Bestiary.ViewWindows;
using Bestiary.OptionsWindows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Bestiary.OtherWindows;
using Bestiary.Services;

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
        public FamiliarFilters FamiliarParameters { get; set; }
        public BitmapImage Icon { get; private set; }

        //Sorting
        public SortTypes[] AvailableSortTypes => FamiliarParameters.ListEnumValues<SortTypes>();
        public SortTypes? SelectedSortType { get; set; }

        //Search
        public string SearchText { get; set; }
        public bool ExactChecked {get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<FamiliarViewModel> FilteredFamiliars { get; private set; }
        public Theme Theme { get; private set; }

        private LambdaCommand m_FetchFamiliars;
        private List<int> m_NewFams;

        private IModel m_Model;
        private SettingsHandler m_Settings;
        private string m_Version;
        private string m_FRDataPath;
        public MainViewModel(MainWindow window, IModel model, string FRDataPath, SettingsHandler settings)
        {
            Window = window;
            Model = model;
            m_FRDataPath = FRDataPath;

            m_Version = FetchVersion();
            try
            {
                m_NewFams = File.ReadAllLines(ApplicationPaths.GetNewFamsPath()).Select(int.Parse).ToList();
            }
            catch
            {
                m_NewFams = new List<int>();
            }

            UserActionLog.Info("Application opened!");
            FamiliarParameters = new FamiliarFilters();

            m_Settings = settings;
            m_Settings.FetchSettings();
            ApplyDefaultSearch();
            Theme = m_Settings.SelectedTheme;

            FetchFamiliars.Execute(null);
            SelectedSortType = SortTypes.Alphabetical;
            SortResults.Execute(null);
            UserActionLog.Info("Familiars loaded on open");
        }


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
                            var tempFamiliarViewModels = tempFamiliars
                                .Select(f => new FamiliarViewModel(m_Model, f, FamiliarParameters.AvailableLocationTypes, Theme)).ToArray();
                            tempFamiliarViewModels = ApplySort(tempFamiliarViewModels);

                            FilteredFamiliars = new ObservableCollection<FamiliarViewModel>();
                            FilteredFamiliars.CollectionChanged += OnFamiliarCollectionChanged;
                            foreach (var familiar in tempFamiliarViewModels)
                            {
                                FilteredFamiliars.Add(familiar);
                            }

                            ResultCount = FilteredFamiliars.Count();
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedCount"));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AwakenedCount"));
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
                                case SpecialState bs:
                                    UserActionLog.Info("Special filter cleared");
                                    FamiliarParameters.SelectedSpecialState = null;
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
                                case SpecialState bs:
                                    return FamiliarParameters.SelectedSpecialState != null;
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
                            var tempFamiliars = ApplySort(FilteredFamiliars.ToArray());
                            FilteredFamiliars = new ObservableCollection<FamiliarViewModel>();
                            FilteredFamiliars.CollectionChanged += OnFamiliarCollectionChanged;
                            foreach (var familiar in tempFamiliars)
                            {
                                FilteredFamiliars.Add(familiar);
                            }
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedCount"));
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AwakenedCount"));
                        }
                    );
                }
                return m_SortResults;
            }
        }

        public MainWindow Window { get; private set; }
        internal IModel Model { get => m_Model; set => m_Model = value; }

        public int ResultCount { get; set; }
        public int OwnedCount => FilteredFamiliars.Where(f => f.Info.Owned == OwnershipStatus.Owned).Count();
        public int AwakenedCount => FilteredFamiliars.Where(f => f.Info.BondLevel == BondingLevels.Awakened).Count(); 
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
            if (FamiliarParameters.SelectedSpecialState != null)
            {
                UserActionLog.Info($"    Filter: Special state {FamiliarParameters.SelectedSpecialState}");
                if(FamiliarParameters.SelectedSpecialState == SpecialState.Bookmarked)
                {
                    filteredFamiliars = filteredFamiliars.Where(f => f.Bookmarked == BookmarkState.Bookmarked);
                }
                else if (FamiliarParameters.SelectedSpecialState == SpecialState.NotBookmarked)
                {
                    filteredFamiliars = filteredFamiliars.Where(f => f.Bookmarked == BookmarkState.NotBookmarked);
                }
                else if (FamiliarParameters.SelectedSpecialState == SpecialState.New)
                {
                    filteredFamiliars = filteredFamiliars.Where(f => m_NewFams.Contains(f.Familiar.Id));
                }
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
                            FamiliarParameters.SelectedSpecialState = null;
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
                        p => new FamiliarDataWindow((FamiliarViewModel)p, m_Model, Theme),
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
                    m_openAboutWindow = new OpenDialogCommand<AboutWindow>(Window, _ => new AboutWindow(m_Version));
                }
                return m_openAboutWindow;
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
                        _ => new ColiseumView(m_Model, FamiliarParameters.AvailableOwnedStatus, FamiliarParameters.AvailableBondingLevels, FamiliarParameters.AvailableLocationTypes, Theme)
                    );
                }
                return m_OpenColiseumView;
            }
        }

        private BaseCommand m_OpenClearBookmarksWindow;
        public ICommand OpenClearBookmarksWindow
        {
            get
            {
                if(m_OpenClearBookmarksWindow == null)
                {
                    m_OpenClearBookmarksWindow = new OpenDialogCommand<ClearBookmarksWindow>(
                        Window,
                        _ => new ClearBookmarksWindow(m_Model),
                        afterClosed: _ => FetchFamiliars.Execute(null)
                    );
                }
                return m_OpenClearBookmarksWindow;
            }
        }

        private LambdaCommand m_OpenUserGuideWindow;
        public ICommand OpenUserGuideWindow
        {
            get
            {
                if(m_OpenUserGuideWindow == null)
                {
                    m_OpenUserGuideWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserGuideWindow userGuide = new UserGuideWindow();
                            userGuide.Show();
                        }
                    );
                }
                return m_OpenUserGuideWindow;
            }
        }

        private BaseCommand m_OpenFolderImportWindow;
        public ICommand OpenFolderImportWindow
        {
            get
            {
                if(m_OpenFolderImportWindow == null)
                {
                    m_OpenFolderImportWindow = new OpenDialogCommand<ImportFromOldWindow>(
                        Window,
                        _ => new ImportFromOldWindow()
                    );
                }
                return m_OpenFolderImportWindow;
            }
        }

        private BaseCommand m_OpenSettingsWindow;
        public ICommand OpenSettingsWindow
        {
            get
            {
                if(m_OpenSettingsWindow == null)
                {
                    m_OpenSettingsWindow = new OpenDialogCommand<SettingsWindow>(
                        Window,
                        _ => new SettingsWindow(FamiliarParameters, SearchText, m_Settings),
                        afterClosed: _ =>
                        {
                            m_Settings.FetchSettings();
                            Theme = m_Settings.SelectedTheme;
                            FetchFamiliars.Execute(null);
                        }
                    );
                }
                return m_OpenSettingsWindow;
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
            return filteredFamiliars;
        }

        private string FetchVersion()
        {
            try
            {
                var manifestText = File.ReadAllText(ApplicationPaths.GetVersionPath());
                var manifestItems = manifestText.Split(',');
                var versionInfoArr = manifestItems.First(i => i.Contains("Version")).Split('"');;
                string version = versionInfoArr.First(i => i.Contains("v"));
                return version;
            }
            catch
            {
                return "";
            }
        }

        private void OnFamiliarCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //MainViewModel.UserActionLog.Debug($"Change event raised ({e.Action})");
            if (e.NewItems != null)
            {
                foreach (var newFamiliar in e.NewItems.OfType<FamiliarViewModel>())
                {
                    //MainViewModel.UserActionLog.Debug($"now listening for changes to ({newFamiliar.Info.Familiar.Name})");
                    newFamiliar.PropertyChanged += OnSingleFamiliarChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var oldFamiliar in e.OldItems.OfType<FamiliarViewModel>())
                {
                    //MainViewModel.UserActionLog.Debug($"no longer listening for changes to ({oldFamiliar.Info.Familiar.Name})");
                    oldFamiliar.PropertyChanged -= OnSingleFamiliarChanged;
                }
            }
        }

        private void OnSingleFamiliarChanged(object sender, PropertyChangedEventArgs e)
        {
            var familiar = sender as FamiliarViewModel;
            if (familiar != null)
            {
                UserActionLog.Debug($"noticed change on familiar ({familiar.Info.Familiar.Name})");
            }
            else
            {
                UserActionLog.Debug($"noticed change on familiar (???)");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedCount"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AwakenedCount"));
            FetchFamiliars.Execute(null);
        }

        private LambdaCommand m_SetAwakened;
        public ICommand SetAwakened
        {
            get
            {
                if(m_SetAwakened == null)
                {
                    m_SetAwakened = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach(var familiar in FilteredFamiliars)
                            {
                                if(familiar.Info.Owned == OwnershipStatus.Owned)
                                {
                                    familiar.Info.BondLevel = BondingLevels.Awakened;
                                }
                            }
                        }
                    );
                }
                return m_SetAwakened;
            }
        }

        private LambdaCommand m_SetWary;
        public ICommand SetWary
        {
            get
            {
                if (m_SetWary == null)
                {
                    m_SetWary = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach (var familiar in FilteredFamiliars)
                            {
                                if (familiar.Info.Owned == OwnershipStatus.Owned)
                                {
                                    familiar.Info.BondLevel = BondingLevels.Wary;
                                }
                            }
                        }
                    );
                }
                return m_SetWary;
            }
        }

        private LambdaCommand m_SetOwned;
        public ICommand SetOwned
        {
            get
            {
                if (m_SetOwned == null)
                {
                    m_SetOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach (var familiar in FilteredFamiliars)
                            {
                                if (familiar.Info.Owned != OwnershipStatus.Owned)
                                {
                                    familiar.SetOwned.Execute(null);
                                }
                            }
                        }
                    );
                }
                return m_SetOwned;
            }
        }

        private LambdaCommand m_SetNotOwned;
        public ICommand SetNotOwned
        {
            get
            {
                if (m_SetNotOwned == null)
                {
                    m_SetNotOwned = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            foreach (var familiar in FilteredFamiliars)
                            {
                                if (familiar.Info.Owned == OwnershipStatus.Owned)
                                {
                                    familiar.Info.OwnedFamiliar.Delete();
                                }
                            }
                            FetchFamiliars.Execute(null);
                        }
                    );
                }
                return m_SetNotOwned;
            }
        }

        public void ApplyDefaultSearch()
        {
            SearchText = m_Settings.SelectedDefaultSearch.SearchText;
            FamiliarParameters.SelectedAvailability = m_Settings.SelectedDefaultSearch.Availability;
            FamiliarParameters.SelectedBondingLevel = m_Settings.SelectedDefaultSearch.BondLevel;
            FamiliarParameters.SelectedCycleYear = new CycleYear(m_Settings.SelectedDefaultSearch.Year != null ? m_Settings.SelectedDefaultSearch.Year.Value : 0);
            FamiliarParameters.SelectedEnemyType = m_Settings.SelectedDefaultSearch.EnemyType;
            FamiliarParameters.SelectedFlight = m_Settings.SelectedDefaultSearch.Flight;
            FamiliarParameters.SelectedGatherType = m_Settings.SelectedDefaultSearch.GatherType;
            FamiliarParameters.SelectedLevel = m_Settings.SelectedDefaultSearch.MinLevel != null ? m_Settings.SelectedDefaultSearch.MinLevel.Value : 0;
            FamiliarParameters.SelectedLocationType = m_Settings.SelectedDefaultSearch.Location;
            FamiliarParameters.SelectedMarketPlaceType = m_Settings.SelectedDefaultSearch.MarketPlace;
            FamiliarParameters.SelectedOwnedStatus = m_Settings.SelectedDefaultSearch.Ownership;
            FamiliarParameters.SelectedSiteEvent = m_Settings.SelectedDefaultSearch.EventName;
            FamiliarParameters.SelectedSource = m_Settings.SelectedDefaultSearch.Source;
            FamiliarParameters.SelectedSpecialState = m_Settings.SelectedDefaultSearch.Special;
            FamiliarParameters.SelectedVenueName = m_Settings.SelectedDefaultSearch.VenueName;
        }


        public static readonly log4net.ILog UserActionLog = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }
}