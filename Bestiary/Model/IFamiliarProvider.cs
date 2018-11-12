using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    class FamiliarInfo
    {
        public Familiar Familiar { get; set; }
        public OwnershipStatus Owned { get; set; }
        public BondingLevels? BondLevel { get; set; }
        public LocationTypes? Location { get; set; }
    }

    interface IFamiliarProvider
    {
        FamiliarInfo[] FetchFamiliars();
    }
}
