using Bestiary.Model;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Bestiary.ViewModel.Dataviews
{
    class FRBestiaryViewModel : INotifyPropertyChanged
    {
        private IModel m_Model;
        private int[] m_SortedFamiliarIds;
        private OwnershipStatus[] m_AvailableOwnedStatus;
        private BondingLevels[] m_AvailableBondingLevels;
        private LocationTypes[] m_AvailableLocationTypes;
        public FRBestiaryViewModel(IModel model, OwnershipStatus[] AvailableOwnedStatus, BondingLevels[] AvailableBondingLevels, LocationTypes[] AvailableLocationTypes)
        {
            m_Model = model;
            m_AvailableBondingLevels = AvailableBondingLevels;
            m_AvailableOwnedStatus = AvailableOwnedStatus;
            m_AvailableLocationTypes = AvailableLocationTypes;
            m_PageCount = 0;

            LeftArrow = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetResourcesDirectory(), "ViewIcons", "arrow_left.png"));
            RightArrow = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetResourcesDirectory(), "ViewIcons", "arrow_right.png"));

            Familiars = new BestiaryViewInfo[8];
            CultureInfo culture = new CultureInfo("de", false);
            m_SortedFamiliarIds = m_Model.Familiars.OrderBy(f => m_Model.LookupFamiliar(f).Fetch().Name, StringComparer.Create(culture, false)).ToArray();

            LoadFamiliars();
        }

        public int PageCount
        {
            get
            {
                return m_PageCount + 1;
            }
            set
            {
                m_PageCount = value - 1;
            }
        }
        private int m_PageCount;
        private int m_MaxPages => (int)Math.Ceiling((double)m_Model.Familiars.Count() / 8);
        private LambdaCommand m_ChangePage;
        public ICommand ChangePage
        {
            get
            {
                if(m_ChangePage == null)
                {
                    m_ChangePage = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            int change = (string)p == m_increment ? 1 : -1;
                            if(m_PageCount + change < 0)
                            {
                                m_PageCount = 0;
                            }
                            else if(m_PageCount + change >= m_MaxPages)
                            {
                                m_PageCount = m_MaxPages-1;
                            }
                            else
                            {
                                m_PageCount += change;
                                LoadFamiliars();
                            }
                            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PageCount)));
                        }
                    );
                }
                return m_ChangePage;
            }
        }
        private string m_increment = "inc";

        private LambdaCommand m_GoToPage;
        public ICommand GoToPage
        {
            get
            {
                if(m_GoToPage == null)
                {
                    m_GoToPage = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            LoadFamiliars();
                        }
                    );
                }
                return m_GoToPage;
            }
        }

        public BitmapImage LeftArrow { get; private set; }
        public BitmapImage RightArrow { get; private set; }

        public BestiaryViewInfo[] Familiars { get; set; }
        private void LoadFamiliars()
        {
            for(int position = 0; position < 8; position++)
            {
                try
                {
                    Familiars[position] = new BestiaryViewInfo(m_Model, LoadFamiliar(position), m_AvailableOwnedStatus, m_AvailableBondingLevels, m_AvailableLocationTypes);
                }
                catch
                {
                    Familiars[position] = null;
                }
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Familiars)));
        }

        private FamiliarInfo LoadFamiliar(int position)
        {
            int Id = m_SortedFamiliarIds[m_PageCount * 8 + position];
            return new FamiliarInfo(m_Model.LookupFamiliar(Id), m_Model.LookupOwnedFamiliar(Id), m_Model.LookupBookmarkedFamiliar(Id));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    class BestiaryViewInfo : INotifyPropertyChanged
    {
        public FamiliarInfo FamiliarInfo { get; set; }
        public OwnershipStatus[] AvailableOwnedStatus { get; private set; }
        public BondingLevels[] AvailableBondingLevels { get; private set; }
        public LocationTypes[] AvailableLocationTypes { get; private set; }
        public BitmapImage Image { get; private set; }
        public string OwnedButtonText { get; set; }
        private IModel m_Model;

        public BestiaryViewInfo(IModel model, FamiliarInfo familiarInfo, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes)
        {
            m_Model = model;
            FamiliarInfo = familiarInfo;
            AvailableLocationTypes = availableLocationTypes;
            AvailableOwnedStatus = availableOwnedStatus;
            AvailableBondingLevels = availableBondingLevels;

            Image = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetResourcesDirectory(), "Images", FamiliarInfo.Familiar.Id + ".png"));
            OwnedButtonText = FamiliarInfo.OwnedFamiliar != null ? m_setNotOwned : m_setOwned;
        }

        private LambdaCommand m_ToggleOwnedStatus;
        public ICommand ToggleOwnedStatus
        {
            get
            {
                if(m_ToggleOwnedStatus == null)
                {
                    m_ToggleOwnedStatus = new LambdaCommand(
                        onExecute: (p) =>
                        {
                            if(FamiliarInfo.OwnedFamiliar == null)
                            {
                                m_Model.AddOwnedFamiliar(new OwnedFamiliar(FamiliarInfo.Familiar.Id, BondingLevels.Wary, LocationTypes.InHoard));
                                FamiliarInfo.OwnedFamiliar = m_Model.LookupOwnedFamiliar(FamiliarInfo.Familiar.Id);
                                OwnedButtonText = m_setNotOwned;
                            }
                            else
                            {
                                FamiliarInfo.OwnedFamiliar.Delete();
                                FamiliarInfo.OwnedFamiliar = null;
                                OwnedButtonText = m_setOwned;
                            }
                        }
                    );
                }
                return m_ToggleOwnedStatus;
            }
        }

        private string m_setOwned = "Set as Owned";
        private string m_setNotOwned = "Set as Not Owned";

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
