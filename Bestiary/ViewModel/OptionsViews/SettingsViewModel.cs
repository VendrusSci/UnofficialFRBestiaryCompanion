﻿using Bestiary.Model;
using Bestiary.Services;
using System.ComponentModel;
using System.Windows.Input;

namespace Bestiary.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private FamiliarFilters m_Filters;
        private string m_SearchText;
        private SettingsHandler m_Settings;
        public SettingsViewModel(FamiliarFilters familiarFilters, string searchText, SettingsHandler settings)
        {
            m_Filters = familiarFilters;
            FilterDefaultButtonText = "Set Current Filter as Default";
            FilterDefaultSetAvailable = true;
            m_Settings = settings;
        }

        public string FilterDefaultButtonText { get; private set; }
        public bool FilterDefaultSetAvailable { get; private set; }
        private LambdaCommand m_SetDefaultFilter;
        public ICommand SetDefaultFilter
        {
            get
            {
                if (m_SetDefaultFilter == null)
                {
                    m_SetDefaultFilter = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            m_Settings.SelectedDefaultSearch = new DefaultSearch
                            {
                                Ownership = m_Filters.SelectedOwnedStatus,
                                Special = m_Filters.SelectedSpecialState,
                                BondLevel = m_Filters.SelectedBondingLevel,
                                BondLevelExclude = m_Filters.BondingLevelInvert,
                                Location = m_Filters.SelectedLocationType,
                                LocationExclude = m_Filters.LocationInvert,
                                Availability = m_Filters.SelectedAvailability,
                                AvailabilityExclude = m_Filters.AvailabilityInvert,
                                Source = m_Filters.SelectedSource,
                                VenueName = m_Filters.SelectedVenueName,
                                EnemyType = m_Filters.SelectedEnemyType,
                                MarketPlace = m_Filters.SelectedMarketPlaceType,
                                Flight = m_Filters.SelectedFlight,
                                Year = m_Filters.SelectedCycleYear.YearNumber,
                                EventName = m_Filters.SelectedSiteEvent,
                                GatherType = m_Filters.SelectedGatherType,
                                MinLevel = m_Filters.SelectedLevel,
                                SearchText = m_SearchText,
                            };
                            FilterDefaultButtonText = "Filter set as default";
                            FilterDefaultSetAvailable = false;
                            m_Settings.SaveSettings();
                        }
                    );
                }
                return m_SetDefaultFilter;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
