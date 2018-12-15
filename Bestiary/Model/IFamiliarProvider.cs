using Bestiary.ViewModel;
using System.ComponentModel;

namespace Bestiary.Model
{
    public class FamiliarInfo : INotifyPropertyChanged
    {
        private ICRUD<Familiar> KnownFamiliar;
        private ICRUD<BookmarkedFamiliar> m_BookmarkedFamiliar;
        private ICRUD<OwnedFamiliar> m_OwnedFamiliar;

        public ICRUD<BookmarkedFamiliar> BookmarkedFamiliar
        {
            get { return m_BookmarkedFamiliar; }
            set
            {
                if(m_BookmarkedFamiliar != value)
                {
                    m_BookmarkedFamiliar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Bookmarked"));
                }
            }
        }

        public ICRUD<OwnedFamiliar> OwnedFamiliar
        {
            get { return m_OwnedFamiliar; }
            set
            {
                if(m_OwnedFamiliar != value)
                {
                    m_OwnedFamiliar = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Location"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BondLevel"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Owned"));
                }
            }
        }

        public Familiar Familiar
        {
            get
            {
                return KnownFamiliar.Fetch();
            }
            private set{}
        }
        public OwnershipStatus Owned => OwnedFamiliar != null ? OwnershipStatus.Owned : OwnershipStatus.NotOwned;
        public BondingLevels? BondLevel
        {
            get
            {
                return OwnedFamiliar?.Fetch()?.BondingLevel;
            }
            set
            {
                if (value.HasValue)
                {
                    OwnedFamiliar?.Update(f => f.BondingLevel = value.Value);
                    MainViewModel.UserActionLog.Debug($"Bonding level set as {value.Value} for {Familiar.Name} ({Familiar.Id})");
                }
            }
        }
        public LocationTypes? Location
        {
            get
            {
                return OwnedFamiliar?.Fetch()?.Location;
            }
            set
            {
                if (value.HasValue)
                {
                    OwnedFamiliar?.Update(f => f.Location = value.Value);
                    MainViewModel.UserActionLog.Debug($"Location set as {value.Value} for {Familiar.Name} ({Familiar.Id})");
                }
            }
        }
        public BookmarkState Bookmarked => BookmarkedFamiliar != null ? BookmarkState.Bookmarked : BookmarkState.NotBookmarked;

        public FamiliarInfo(ICRUD<Familiar> familiar, ICRUD<OwnedFamiliar> ownedFamiliar, ICRUD<BookmarkedFamiliar> bookmarkedFamiliar)
        {
            KnownFamiliar = familiar;
            OwnedFamiliar = ownedFamiliar;
            BookmarkedFamiliar = bookmarkedFamiliar;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    interface IFamiliarProvider
    {
        FamiliarInfo[] FetchFamiliars();
    }
}
