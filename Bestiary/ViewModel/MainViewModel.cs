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
        private IModel m_Model;
        public FamiliarParameters FamiliarParameters { get; set; }

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
                            var familiars = Model.Familiars
                                .Select(id => Model.LookupFamiliar(id).Fetch())
                                .Select(familiar =>
                                {
                                    var owned = Model.LookupOwnedFamiliar(familiar.Id);
                                    return new FamiliarInfo
                                    {
                                        Familiar = familiar,
                                        BondLevel = owned?.Fetch()?.BondingLevel,
                                        Location = owned?.Fetch()?.Location,
                                    };
                                });
                            var tempFamiliars = ApplyFilters(familiars);
                            foreach (var subFilter in FamiliarParameters.SubFilterList)
                            {
                                tempFamiliars = subFilter.Apply(tempFamiliars);
                            }
                            tempFamiliars = ApplySearch(tempFamiliars);
                            tempFamiliars = ApplySort(tempFamiliars);
                            FilteredFamiliars = tempFamiliars
                                .Select(f => new FamiliarViewModel(f, FamiliarParameters.AvailableLocationTypes))
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
                                    FamiliarParameters.SelectedOwnedStatus = null;
                                    break;
                                case BondingLevels bl:
                                    FamiliarParameters.SelectedBondingLevel = null;
                                    break;
                                case Sources s:
                                    FamiliarParameters.SelectedSource = null;
                                    break;
                                case Availabilities a:
                                    FamiliarParameters.SelectedAvailability = null;
                                    break;
                                case SortTypes st:
                                    SelectedSortType = null;
                                    break;
                                case LocationTypes l:
                                    FamiliarParameters.SelectedLocationType = null;
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

        public MainWindow Window { get; private set; }
        internal IModel Model { get => m_Model; set => m_Model = value; }

        private IEnumerable<FamiliarInfo> ApplyFilters(IEnumerable<FamiliarInfo> familiars)
        {
            IEnumerable<FamiliarInfo> filteredFamiliars = familiars;

            if (FamiliarParameters.SelectedBondingLevel != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.BondLevel == FamiliarParameters.SelectedBondingLevel);
            }
            if (FamiliarParameters.SelectedOwnedStatus != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.Owned == FamiliarParameters.SelectedOwnedStatus);
            }
            if (FamiliarParameters.SelectedAvailability != null)
            {
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Availability == FamiliarParameters.SelectedAvailability);
            }
            if (FamiliarParameters.SelectedSource != null)
            {
                var lookingFor = FamiliarParameters.SourceMap[FamiliarParameters.SelectedSource.Value];
                filteredFamiliars = filteredFamiliars.Where(f => f.Familiar.Source.GetType() == lookingFor);
            }

            return filteredFamiliars;
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
                            FamiliarAddWindow familiarAddWindow = new FamiliarAddWindow(m_Model);
                            familiarAddWindow.Owner = Window;
                            familiarAddWindow.ShowDialog();
                        }
                    );
                }
                return m_openAddFamiliarWindow;                
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
            FamiliarParameters = new FamiliarParameters();
        }
    }
}