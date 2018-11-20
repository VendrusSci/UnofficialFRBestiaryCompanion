using System.ComponentModel;

namespace Bestiary.Model
{
    public class FamiliarInfo : INotifyPropertyChanged
    {
        private ICRUD<Familiar> KnownFamiliar;
        public ICRUD<OwnedFamiliar> OwnedFamiliar;

        public Familiar Familiar
        {
            get
            {
                return KnownFamiliar.Fetch();
            }
            private set{}
        }
        public OwnershipStatus Owned { get; set; }
        public BondingLevels? BondLevel
        {
            get
            {
                return OwnedFamiliar?.Fetch()?.BondingLevel;
            }
            set
            {
                OwnedFamiliar.Update(f => f.BondingLevel = value.Value);
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
                OwnedFamiliar.Update(f=> f.Location = value.Value);
            }
        }

        public FamiliarInfo(ICRUD<Familiar> familiar, ICRUD<OwnedFamiliar> ownedFamiliar)
        {
            KnownFamiliar = familiar;
            OwnedFamiliar = ownedFamiliar;
            Owned = ownedFamiliar != null ? OwnershipStatus.Owned : OwnershipStatus.NotOwned;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    interface IFamiliarProvider
    {
        FamiliarInfo[] FetchFamiliars();
    }
}
