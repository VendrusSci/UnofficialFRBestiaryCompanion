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
        public Flights Flight { get; private set; }
        [DataMember]
        public IFamiliarSource Source { get; private set; }
        [DataMember]
        public Availabilities Availability { get; private set; }
        [DataMember]
        public string FlavourText { get; private set; }
        [DataMember]
        public string AdditionalInfo { get; private set; }
        [DataMember]
        public string ImagePath { get; private set; }
        [DataMember]
        public int Id { get; private set; }

        public Familiar(string famName, Flights famElement, IFamiliarSource famLoc, Availabilities famAvail, string famFlavText, string famInfo, int famId)
        {
            Name = famName;
            Flight = famElement;
            Source = famLoc;
            Availability = famAvail;
            FlavourText = famFlavText;
            AdditionalInfo = famInfo;
            Id = famId;
        }
    }

    [DataContract]
    class OwnedFamiliar
    {
        public OwnedFamiliar(Familiar familiar, BondingLevels bondingLevel)
        {
            Familiar = familiar;
            BondingLevel = bondingLevel;
        }

        [DataMember]
        public Familiar Familiar { get; private set; }

        [DataMember]
        public BondingLevels BondingLevel { get; set; }
    }
}
