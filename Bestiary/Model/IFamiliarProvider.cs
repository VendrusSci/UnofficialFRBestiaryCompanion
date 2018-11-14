using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    class FamiliarInfo : INotifyPropertyChanged
    {
        public Familiar Familiar { get; set; }
        public OwnershipStatus Owned { get; set; }
        public BondingLevels? BondLevel { get; set; }
        public LocationTypes? Location { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    interface IFamiliarProvider
    {
        FamiliarInfo[] FetchFamiliars();
    }
}
