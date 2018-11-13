using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    [DataContract]
    class Familiar
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public IFamiliarSource Source { get; private set; }
        [DataMember]
        public Availabilities Availability { get; private set; }
        [DataMember]
        public string FlavourText { get; private set; }
        [DataMember]
        public int Id { get; private set; }

        public Familiar(string famName, IFamiliarSource famLoc, Availabilities famAvail, string famFlavText, int famId)
        {
            Name = famName;
            Source = famLoc;
            Availability = famAvail;
            FlavourText = famFlavText;
            Id = famId;
        }
    }

    [DataContract]
    class OwnedFamiliar
    {
        public OwnedFamiliar(Familiar familiar)
        {
            Familiar = familiar;
            BondingLevel = BondingLevels.Wary;
            Location = LocationTypes.InHoard;
        }

        [DataMember]
        public Familiar Familiar { get; private set; }

        [DataMember]
        public BondingLevels BondingLevel { get; set; }

        [DataMember]
        public LocationTypes Location { get; set; }
    }
}
