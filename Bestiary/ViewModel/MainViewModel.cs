using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Bestiary.Model;

namespace Bestiary.ViewModel
{
    enum SortTypes
    {
        [Description("Bond Level")]
        BondLevel,
        Alphabetical,
        HoardOrder
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
                                    return new FamiliarInfo(
                                        m_Model.LookupFamiliar(familiar.Id),
                                        owned
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
                            UserActionLog.Info("Applying sort");
                            tempFamiliars = ApplySort(tempFamiliars);
                            FilteredFamiliars = tempFamiliars
                                .Select(f => new FamiliarViewModel(m_Model, f, FamiliarParameters.AvailableLocationTypes))
                                .ToArray();
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
                                case BondingLevels bl:
                                    UserActionLog.Info("Bonding filter cleared");
                                    FamiliarParameters.SelectedBondingLevel = null;
                                    break;
                                case Sources s:
                                    UserActionLog.Info("Source filter cleared");
                                    FamiliarParameters.SelectedSource = null;
                                    break;
                                case Availabilities a:
                                    UserActionLog.Info("Availability filter cleared");
                                    FamiliarParameters.SelectedAvailability = null;
                                    break;
                                case SortTypes st:
                                    UserActionLog.Info("Sort type cleared");
                                    SelectedSortType = null;
                                    break;
                                case LocationTypes l:
                                    UserActionLog.Info("Location filter cleared");
                                    FamiliarParameters.SelectedLocationType = null;
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
                filteredFamiliars = filteredFamiliars.Where(f => f.BondLevel == FamiliarParameters.SelectedBondingLevel);
            }
            if (FamiliarParameters.SelectedOwnedStatus != null)
            {
                UserActionLog.Info($"    Filter: Ownership status {FamiliarParameters.SelectedOwnedStatus}");
                filteredFamiliars = filteredFamiliars.Where(f => f.Owned == FamiliarParameters.SelectedOwnedStatus);
            }
            if (FamiliarParameters.SelectedLocationType != null)
            {
                UserActionLog.Info($"    Filter: Location {FamiliarParameters.SelectedLocationType.Value}");
                filteredFamiliars = filteredFamiliars.Where(f => f.Location == FamiliarParameters.SelectedLocationType);
            }
            if (FamiliarParameters.SelectedAvailability != null)
            {
                UserActionLog.Info($"    Filter: Availability {FamiliarParameters.SelectedAvailability}");
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Availability == FamiliarParameters.SelectedAvailability);
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
                        }
                    );
                }
                return m_ClearAllFilters;
            }
        }

        private LambdaCommand m_openAddFamiliarWindow;
        public ICommand OpenAddFamiliarWindow
        {
            get
            {
                if(m_openAddFamiliarWindow == null)
                {
                    m_openAddFamiliarWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("Add familiar window opened");
                            FamiliarAddWindow familiarAddWindow = new FamiliarAddWindow(m_Model);
                            familiarAddWindow.Owner = Window;
                            familiarAddWindow.ShowDialog();
                        }
                    );
                }
                return m_openAddFamiliarWindow;                
            }
        }

        private LambdaCommand m_openDeleteFamiliarWindow;
        public ICommand OpenDeleteFamiliarWindow
        {
            get
            {
                if (m_openDeleteFamiliarWindow == null)
                {
                    m_openDeleteFamiliarWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("Delete familiar window opened");
                            FamiliarDeleteWindow familiarDeleteWindow = new FamiliarDeleteWindow(m_Model);
                            familiarDeleteWindow.Owner = Window;
                            familiarDeleteWindow.ShowDialog();
                        }
                    );
                }
                return m_openDeleteFamiliarWindow;
            }
        }

        private LambdaCommand m_openDataFamiliarWindow;
        public ICommand OpenDataFamiliarWindow
        {
            get
            {
                if(m_openDataFamiliarWindow == null)
                {
                    m_openDataFamiliarWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info($"Familiar Data window opened: {((FamiliarViewModel)p).Info.Familiar.Name}, #{((FamiliarViewModel)p).Info.Familiar.Id}");
                            FamiliarDataWindow familiarDataWindow = new FamiliarDataWindow((FamiliarViewModel)p, m_Model);
                            familiarDataWindow.Owner = Window;
                            familiarDataWindow.ShowDialog();
                        },
                        onCanExecute: (p) =>
                        {
                            return p.GetType() == typeof(FamiliarViewModel);
                        }
                    );
                }
                return m_openDataFamiliarWindow;
            }
        }

        private LambdaCommand m_openSupportInfoWindow;
        public ICommand OpenSupportInfoWindow
        {
            get
            {
                if (m_openSupportInfoWindow == null)
                {
                    m_openSupportInfoWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("Support window opened");
                            SupportInfoWindow supportWindow = new SupportInfoWindow();
                            supportWindow.Owner = Window;
                            supportWindow.ShowDialog();
                        }
                    );
                }
                return m_openSupportInfoWindow;
            }
        }

        private LambdaCommand m_openAboutWindow;
        public ICommand OpenAboutWindow
        {
            get
            {
                if (m_openAboutWindow == null)
                {
                    m_openAboutWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("About window opened");
                            AboutWindow aboutWindow = new AboutWindow();
                            aboutWindow.Owner = Window;
                            aboutWindow.ShowDialog();
                        }
                    );
                }
                return m_openAboutWindow;
            }
        }

        private LambdaCommand m_openFetchUpdateWindow;
        public ICommand OpenFetchUpdateWindow
        {
            get
            {
                if (m_openFetchUpdateWindow == null)
                {
                    m_openFetchUpdateWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            UserActionLog.Info("Update window opened");
                            FetchUpdateWindow updateWindow = new FetchUpdateWindow();
                            updateWindow.Owner = Window;
                            updateWindow.ShowDialog();
                        }
                    );
                }
                return m_openFetchUpdateWindow;
            }
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

        private IEnumerable<FamiliarInfo> ApplySearch(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> filteredFamiliars = familiars;
            if(SearchText != null)
            {
                if(!ExactChecked)
                {
                    filteredFamiliars = filteredFamiliars.Where(s => s.Familiar.Name.ToLower().Contains(SearchText.ToLower()));
                }
                else
                {
                    filteredFamiliars = filteredFamiliars.Where(s => s.Familiar.Name.ToLower().Equals(SearchText.ToLower()));
                }
            }
            return filteredFamiliars;
        }

        public MainViewModel(MainWindow window, IModel model)
        {
            Window = window;
            Model = model;

            UserActionLog.Info("Application opened!");
            FamiliarParameters = new FamiliarFilters();
            FetchFamiliars.Execute(null);
            UserActionLog.Info("Familiars loaded on open");

            try
            {
                Icon = new BitmapImage();
                Icon.BeginInit();
                Icon.CacheOption = BitmapCacheOption.OnLoad;
                Icon.UriSource = new Uri("Resources/bestiary.png", UriKind.RelativeOrAbsolute);
                Icon.EndInit();
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