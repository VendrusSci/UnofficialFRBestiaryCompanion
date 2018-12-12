using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Bestiary.ViewModel.Dataviews
{
    public class ColiseumViewModel : INotifyPropertyChanged
    {
        private IModel m_Model;
        private OwnershipStatus[] m_AvailableOwnedStatus;
        private BondingLevels[] m_AvailableBondingLevels;
        private LocationTypes[] m_AvailableLocationTypes;
        private FamiliarInfo[] m_ColiseumFamiliars;

        public ColiseumViewModel(IModel model, OwnershipStatus[] AvailableOwnedStatus, BondingLevels[] AvailableBondingLevels, LocationTypes[] AvailableLocationTypes)
        {
            m_Model = model;
            m_AvailableBondingLevels = AvailableBondingLevels;
            m_AvailableOwnedStatus = AvailableOwnedStatus;
            m_AvailableLocationTypes = AvailableLocationTypes;

            m_ColiseumFamiliars = m_Model.Familiars
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
                               })
                               .Where(familiar => familiar.Familiar.Source.GetType() == typeof(Coliseum))
                               .ToArray();

            FetchVenues();
        }

        private Venues m_VenueSet = new Venues();
        public ColiseumVenue[] Venues { get; private set; }
        private void FetchVenues()
        {
            Venues = m_VenueSet.VenueNames
                .Select(name => SetUpVenue(name))
                .ToArray();
        }

        private ColiseumVenue SetUpVenue(string name)
        {
            var familiars = m_ColiseumFamiliars
                .Where(f => ((Coliseum)f.Familiar.Source).VenueName == name)
                .Select(f => new FamiliarViewModel(m_Model, f, m_AvailableLocationTypes));
            return new ColiseumVenue(name, familiars, m_AvailableOwnedStatus, m_AvailableBondingLevels, m_AvailableLocationTypes);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ColiseumVenue : INotifyPropertyChanged
    {
        public BitmapImage HeaderImage { get; private set; }
        public string Name { get; private set; }
        //public FamiliarViewModel[] Familiars { get; private set; }
        public ObservableCollection<FamiliarViewModel> Familiars { get; private set; } = new ObservableCollection<FamiliarViewModel>();
        public int NumOwned => Familiars.Where(f => f.Info.Owned == OwnershipStatus.Owned).Count();
        public int NumFamiliars => Familiars.Count();
        public int OwnedPercentage => (NumOwned * 100) / NumFamiliars;
        public OwnershipStatus[] AvailableOwnedStatus { get; private set; }
        public BondingLevels[] AvailableBondingLevels { get; private set; }
        public LocationTypes[] AvailableLocationTypes { get; private set; }

        public ColiseumVenue(string name, IEnumerable<FamiliarViewModel> familiars, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes)
        {
            AvailableLocationTypes = availableLocationTypes;
            AvailableOwnedStatus = availableOwnedStatus;
            AvailableBondingLevels = availableBondingLevels;
            Name = name;
            HeaderImage = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetViewIconDirectory(), name + ".png"));

            Familiars.CollectionChanged += OnFamiliarCollectionChanged;
            foreach(var familiar in familiars)
            {
                Familiars.Add(familiar);
            }
        }

        private void OnFamiliarCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MainViewModel.UserActionLog.Debug($"Change event raised ({e.Action})");
            if (e.NewItems != null)
            {
                foreach (var newFamiliar in e.NewItems.OfType<FamiliarViewModel>())
                {
                    MainViewModel.UserActionLog.Debug($"now listening for changes to ({newFamiliar.Info.Familiar.Name})");
                    newFamiliar.PropertyChanged += OnSingleFamiliarChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (var oldFamiliar in e.OldItems.OfType<FamiliarViewModel>())
                {
                    MainViewModel.UserActionLog.Debug($"no longer listening for changes to ({oldFamiliar.Info.Familiar.Name})");
                    oldFamiliar.PropertyChanged -= OnSingleFamiliarChanged;
                }
            }
        }

        private void OnSingleFamiliarChanged(object sender, PropertyChangedEventArgs e)
        {
            var familiar = sender as FamiliarViewModel;
            if (familiar != null)
            {
                MainViewModel.UserActionLog.Debug($"noticed change on familiar ({familiar.Info.Familiar.Name})");
            }
            else
            {
                MainViewModel.UserActionLog.Debug($"noticed change on familiar (???)");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumOwned"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedPercentage"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
