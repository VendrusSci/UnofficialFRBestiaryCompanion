using Bestiary.Model;
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

namespace Bestiary.ViewModel
{
    class GatherViewModel : INotifyPropertyChanged
    {
        private IModel m_Model;
        private OwnershipStatus[] m_AvailableOwnedStatus;
        private BondingLevels[] m_AvailableBondingLevels;
        private LocationTypes[] m_AvailableLocationTypes;
        private FamiliarInfo[] m_ColiseumFamiliars;

        public event PropertyChangedEventHandler PropertyChanged;

        public GatherViewModel(IModel model, OwnershipStatus[] AvailableOwnedStatus, BondingLevels[] AvailableBondingLevels, LocationTypes[] AvailableLocationTypes)
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
                               .Where(familiar => familiar.Familiar.Source.GetType() == typeof(Gathering))
                               .ToArray();
            FetchGatherAreas();
        }

        public GatherArea[] GatherAreas { get; private set; }
        private void FetchGatherAreas()
        {
            List<GatherArea> tempList = new List<GatherArea>();
            foreach (GatherTypes type in Enum.GetValues(typeof(GatherTypes)))
            {
                tempList.Add(SetUpGatherArea(type));
            }
            GatherAreas = tempList.ToArray();
        }

        private GatherArea SetUpGatherArea(GatherTypes type)
        {
            var familiars = m_ColiseumFamiliars
                .Where(f => ((Gathering)f.Familiar.Source).GatherType == type)
                .Select(f => new FamiliarViewModel(m_Model, f, m_AvailableLocationTypes));
            return new GatherArea(m_Model, type, familiars);
        }
    }

    public class GatherArea : INotifyPropertyChanged
    {
        public BitmapImage HeaderImage { get; private set; }
        public GatherTypes Type { get; private set; }

        public ObservableCollection<FamiliarViewModel> Familiars { get; private set; } = new ObservableCollection<FamiliarViewModel>();

        public int NumOwned => Familiars.Where(f => f.Info.Owned == OwnershipStatus.Owned).Count();
        public int NumFamiliars => Familiars.Count();
        public int OwnedPercentage => (NumOwned * 100) / NumFamiliars;
        private IModel m_Model;

        public GatherArea(IModel model, GatherTypes type, IEnumerable<FamiliarViewModel> familiars)
        {
            Type = type;
            m_Model = model;
            try
            {
                HeaderImage = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetViewIconDirectory(), type.ToString() + ".png"));
            }
            catch { }
            Familiars.CollectionChanged += OnFamiliarCollectionChanged;
            foreach (var familiar in familiars)
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
            foreach (var familiar in Familiars)
            {
                familiar.Info.OwnedFamiliar = m_Model.LookupOwnedFamiliar(familiar.Info.Familiar.Id);
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumOwned"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("OwnedPercentage"));
        }

        public GatherView Window { get; set; }

        private LambdaCommand m_OpenDataFamiliarWindow;
        public ICommand OpenDataFamiliarWindow
        {
            get
            {
                if (m_OpenDataFamiliarWindow == null)
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