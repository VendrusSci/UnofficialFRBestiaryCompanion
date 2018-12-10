using Bestiary.Model;
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
            Venues = new ColiseumVenue[m_VenueSet.VenueNames.Count()];

            var tempFamiliars = m_Model.Familiars
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
                               .Where(familiar => familiar.Familiar.Source.GetType() == typeof(Coliseum));
            m_ColiseumFamiliars = tempFamiliars.ToArray();
            FetchVenues();
        }

        private Venues m_VenueSet = new Venues();
        public ColiseumVenue[] Venues { get; private set; }
        private void FetchVenues()
        {
            int count = 0;
            foreach(var venue in m_VenueSet.VenueNames)
            {
                Venues[count] = SetUpVenue(venue);
                count++;
            }
        }

        private ColiseumVenue SetUpVenue(string name)
        {
            FamiliarViewModel[] familiars;
            var tempFamiliars = m_ColiseumFamiliars.Where(f => ((Coliseum)f.Familiar.Source).VenueName == name);
            familiars = new FamiliarViewModel[tempFamiliars.Count()];
            int count = 0;
            foreach(var fam in tempFamiliars)
            {
                familiars[count] = new FamiliarViewModel(m_Model, tempFamiliars.ElementAt(count), m_AvailableLocationTypes);
                count++;
            }
            return new ColiseumVenue(name, familiars, m_AvailableOwnedStatus, m_AvailableBondingLevels, m_AvailableLocationTypes);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class ColiseumVenue : INotifyPropertyChanged
    {
        public BitmapImage HeaderImage { get; private set; }
        public string Name { get; private set; }
        public FamiliarViewModel[] Familiars { get; private set; }
        public int NumOwned => Familiars.Where(f => f.Info.Owned == OwnershipStatus.Owned).Count();
        public int NumFamiliars => Familiars.Count();
        public int OwnedPercentage => NumOwned / NumFamiliars;
        public OwnershipStatus[] AvailableOwnedStatus { get; private set; }
        public BondingLevels[] AvailableBondingLevels { get; private set; }
        public LocationTypes[] AvailableLocationTypes { get; private set; }

        public ColiseumVenue(string name, FamiliarViewModel[] familiars, OwnershipStatus[] availableOwnedStatus, BondingLevels[] availableBondingLevels, LocationTypes[] availableLocationTypes)
        {
            AvailableLocationTypes = availableLocationTypes;
            AvailableOwnedStatus = availableOwnedStatus;
            AvailableBondingLevels = availableBondingLevels;
            Name = name;
            Familiars = familiars;
            HeaderImage = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetViewIconDirectory(), name + ".png"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
