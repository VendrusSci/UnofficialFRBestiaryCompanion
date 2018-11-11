using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    class FakeFamiliarFetcher : IFamiliarProvider
    {
        public FamiliarInfo[] FetchFamiliars()
        {
            return new FamiliarInfo[]
            {
                new FamiliarInfo
                {
                    Familiar = new Familiar("Oliver", Flights.Lightning, new SiteEvent("Anniversary", new CycleYear(2)), Availabilities.Always, "The bestest", "Gives great hugs", 0),
                    BondLevel = BondingLevels.Inquisitive,
                    Owned = OwnershipStatus.Owned,
                },
                new FamiliarInfo
                {
                    Familiar = new Familiar("Oliver1", Flights.Lightning, new Coliseum("Rainsong Forest", EnemyTypes.Boss), Availabilities.Always, "The bestest", "Gives great hugs", 3),
                    BondLevel = BondingLevels.Inquisitive,
                    Owned = OwnershipStatus.NotOwned,
                },
                new FamiliarInfo
                {
                    Familiar = new Familiar("Oliver2", Flights.Lightning, new Festival(Flights.Lightning, new CycleYear(1)), Availabilities.Always, "The bestest", "Gives great hugs", 2),
                    BondLevel = BondingLevels.Awakened,
                    Owned = OwnershipStatus.Owned,
                }
            };
        }
    }
}
