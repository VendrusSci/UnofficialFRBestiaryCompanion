using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Bestiary.Model
{
    class XmlFamliarFetcher : IFamiliarProvider
    {
        public XmlFamliarFetcher(string builtinFilepath, string userFilepath)
        {
            SaveOutTestData(builtinFilepath, userFilepath);

            m_FlightRisingInformation = TryLoadXml<BuiltinFlightRisingInformation>(builtinFilepath) ?? new BuiltinFlightRisingInformation();
            m_UserFamiliarInformation = TryLoadXml<UserFamiliarInformation>(userFilepath) ?? new UserFamiliarInformation();
        }

        public FamiliarInfo[] FetchFamiliars()
        {
            return m_FlightRisingInformation.Familiars
                .Select(f =>
                {
                    var owned = m_UserFamiliarInformation.OwnedFamiliars
                        .FirstOrDefault(o => o.Familiar.Id == f.Id);
                    return new FamiliarInfo
                    {
                        Familiar = f,
                        Owned = owned != null ? OwnershipStatus.Owned : OwnershipStatus.NotOwned,// owned != null,
                        BondLevel = owned?.BondingLevel,
                        Location = owned?.Location,
                    };
                })
                .ToArray();
        }

        private void SaveOutTestData(string builtinFilepath, string userFilepath)
        {
            SaveXml(builtinFilepath, BuiltinFlightRisingInformation.MakeTestData());
            SaveXml(userFilepath, UserFamiliarInformation.MakeTestData());
        }

        private void SaveXml<T>(string filepath, T info)
        {
            var serialiser = new DataContractSerializer(typeof(T));
            using (var stream = File.Open(filepath, FileMode.Create))
            {
                using (var writer = XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(stream)))
                {
                    serialiser.WriteObject(writer, info, new CustomResolver());
                }
            }
        }

        private T LoadXml<T>(string filepath)
        {
            var deserialiser = new DataContractSerializer(typeof(T));
            using (var stream = File.OpenRead(filepath))
            {
                using (var reader = XmlDictionaryReader.CreateDictionaryReader(XmlReader.Create(stream)))
                {
                    return (T)deserialiser.ReadObject(reader, false, new CustomResolver());
                }
            }
        }

        private T TryLoadXml<T>(string filepath) where T: class
        {
            try
            {
                return LoadXml<T>(filepath);
            }
            catch (FileNotFoundException)
            {
            }
            catch (DirectoryNotFoundException)
            {
            }

            return null;
        }

        private UserFamiliarInformation m_UserFamiliarInformation = new UserFamiliarInformation();
        private BuiltinFlightRisingInformation m_FlightRisingInformation = new BuiltinFlightRisingInformation();
    }

    class CustomResolver : DataContractResolver
    {
        public override bool TryResolveType(Type dataContractType, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            if(AcceptedTypes.Any(t => t == dataContractType))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(dataContractType.Name);
                typeNamespace = dictionary.Add(CustomNamespace);
                return true;
            }
            
            return knownTypeResolver.TryResolveType(dataContractType, declaredType, null, out typeName, out typeNamespace);
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            if (typeNamespace == CustomNamespace)
            {
                var match = AcceptedTypes.FirstOrDefault(t => t.Name == typeName);
                if (match != null)
                {
                    return match;
                }
            }
            
            return knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null);
        }

        private static readonly Type[] AcceptedTypes = new Type[]
        {
            typeof(Coliseum),
            typeof(SiteEvent),
            typeof(Festival),
            typeof(Marketplace),
            typeof(JoxarSpareInventory),
            typeof(Baldwin),
            typeof(Swipp),
            typeof(Gathering),
        };

        private static readonly string CustomNamespace = "http://example.com";
    }

    [DataContract]
    class BuiltinFlightRisingInformation
    {
        public static BuiltinFlightRisingInformation MakeTestData()
        {
            var fakeSource = new FakeFamiliarFetcher();
            var testData = fakeSource.FetchFamiliars()
                .Select(f => f.Familiar);

            var result = new BuiltinFlightRisingInformation();
            result.Familiars = testData.ToList();
            return result;
        }

        public BuiltinFlightRisingInformation()
        {
            Familiars = new List<Familiar>();
        }

        [DataMember]
        public List<Familiar> Familiars { get; private set; }
    }

    [DataContract]
    class UserFamiliarInformation
    {
        public static UserFamiliarInformation MakeTestData()
        {
            var fakeSource = new FakeFamiliarFetcher();
            var testData = fakeSource.FetchFamiliars()
                .Select(f => new OwnedFamiliar(f.Familiar));

            var result = new UserFamiliarInformation();
            result.OwnedFamiliars = testData.ToList();
            return result;
        }

        public UserFamiliarInformation()
        {
            OwnedFamiliars = new List<OwnedFamiliar>();
        }

        [DataMember]
        public List<OwnedFamiliar> OwnedFamiliars { get; private set; }
    }
}
