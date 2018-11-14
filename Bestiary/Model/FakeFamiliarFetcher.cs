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
                    Familiar = new Familiar("Agol", new SiteEvent("Anniversary", new CycleYear(2)), Availabilities.Event, "A backwards thinking creature.", 15298),
                    BondLevel = BondingLevels.Inquisitive,
                    Owned = OwnershipStatus.NotOwned,
                    Location = LocationTypes.InHoard,
                },
                new FamiliarInfo
                {
                    Familiar = new Familiar("Almandine Sturgeon", new Coliseum("Crystal Pools", EnemyTypes.Normal), Availabilities.Always, "Dwelling in the waters of the Crystal Pools has left the scales of this fish translucent and hard as a gemstone.", 13435),
                    BondLevel = BondingLevels.Inquisitive,
                    Owned = OwnershipStatus.NotOwned,
                    Location = LocationTypes.InHoard,
                },
                new FamiliarInfo
                {
                    Familiar = new Familiar("Animated Armor", new SiteEvent("Night of the Nocturne" , new CycleYear(3)), Availabilities.Event, "A blessing and a curse, donning this armor will increase one's martial abilities, but also one's taste for battle.", 20827),
                    BondLevel = BondingLevels.Awakened,
                    Owned = OwnershipStatus.NotOwned,
                    Location = LocationTypes.InHoard,
                },
                new FamiliarInfo
                {
                    Familiar = new Familiar("Alstroemeria Fox", new Coliseum("Blooming Grove", EnemyTypes.Normal), Availabilities.Always, "The happiest fox.", 23849),
                    BondLevel = BondingLevels.Awakened,
                    Owned = OwnershipStatus.Owned,
                    Location = LocationTypes.OnDragon,
                },
            };
        }
    }
}
