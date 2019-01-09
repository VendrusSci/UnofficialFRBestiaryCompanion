using Bestiary.Model;
using Bestiary.Services;
using Bestiary.ViewWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
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
        public Theme Theme;

        public ColiseumViewModel(IModel model, OwnershipStatus[] AvailableOwnedStatus, BondingLevels[] AvailableBondingLevels, LocationTypes[] AvailableLocationTypes, Theme theme)
        {
            m_Model = model;
            m_AvailableBondingLevels = AvailableBondingLevels;
            m_AvailableOwnedStatus = AvailableOwnedStatus;
            m_AvailableLocationTypes = AvailableLocationTypes;
            Theme = theme;

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
                .Select(f => new FamiliarViewModel(m_Model, f, m_AvailableLocationTypes, Theme));
            return new ColiseumVenue(m_Model, name, familiars);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ColiseumVenue : INotifyPropertyChanged
    {
        public BitmapImage HeaderImage { get; private set; }
        public string Name { get; private set; }

        public ObservableCollection<FamiliarViewModel> Familiars { get; private set; } = new ObservableCollection<FamiliarViewModel>();

        public int NumOwned => Familiars.Where(f => f.Info.Owned == OwnershipStatus.Owned).Count();
        public int NumFamiliars => Familiars.Count();
        public int OwnedPercentage => (NumOwned * 100) / NumFamiliars;
        private IModel m_Model;

        public ColiseumVenue(IModel model, string name, IEnumerable<FamiliarViewModel> familiars)
        {
            Name = name;
            m_Model = model;
            HeaderImage = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetViewIconDirectory(), name + ".png"));
            Familiars.CollectionChanged += OnFamiliarCollectionChanged;
            foreach(var familiar in familiars)
            {
                Familiars.Add(familiar);
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
                MainViewModel.UserActionLog.Debug($"noticed change on familiar ({familiar.Info.Familiar.Name})");
            }
            else
            {
                MainViewModel.UserActionLog.Debug($"noticed change on familiar (???)");
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumOwned"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedPercentage"));
        }

        private void UpdateAllFamiliars()
        {
            foreach(var familiar in Familiars)
            {
                familiar.Info.OwnedFamiliar = m_Model.LookupOwnedFamiliar(familiar.Info.Familiar.Id);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumOwned"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedPercentage"));
        }

        public ColiseumView Window { get; set; }

        private LambdaCommand m_OpenDataFamiliarWindow;
        public ICommand OpenDataFamiliarWindow
        {
            get
            {
                if(m_OpenDataFamiliarWindow == null)
                {
                    m_OpenDataFamiliarWindow = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            MainViewModel.UserActionLog.Info($"Familiar Data window opened: {((FamiliarViewModel)p).Info.Familiar.Name}, #{((FamiliarViewModel)p).Info.Familiar.Id}");
                            FamiliarDataWindow familiarDataWindow = new FamiliarDataWindow((FamiliarViewModel)p, m_Model);
                            familiarDataWindow.Owner = Window;
                            familiarDataWindow.ShowDialog();
                            UpdateAllFamiliars();
                        },
                        onCanExecute: (p) =>
                        {
                            return p.GetType() == typeof(FamiliarViewModel);
                        }
                    );
                }
                return m_OpenDataFamiliarWindow;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
