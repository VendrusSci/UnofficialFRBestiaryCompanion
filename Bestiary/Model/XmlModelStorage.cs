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
    public interface IModel
    {
        ICRUD<Familiar> LookupFamiliar(int id);
        ICRUD<OwnedFamiliar> LookupOwnedFamiliar(int id);
        IEnumerable<int> Familiars { get; }
        IEnumerable<int> OwnedFamiliars { get; }
        void AddFamiliar(Familiar familiar);
        void AddOwnedFamiliar(OwnedFamiliar ownedFamiliar);
    }

    class XmlCRUD<ValueType> : ICRUD<ValueType>
    {
        public XmlCRUD(XmlModelStorage storage, List<ValueType> collection, ValueType value)
        {
            m_Collection = collection;
            m_Storage = storage;
            m_Value = value;
        }

        public void Delete()
        {
            m_Collection.Remove(m_Value);
            m_Storage.Save();
        }

        public ValueType Fetch()
        {
            return m_Value;
        }

        public void Update(Action<ValueType> update)
        {
            update(m_Value);
            m_Storage.Save();
        }

        ValueType m_Value;
        XmlModelStorage m_Storage;
        List<ValueType> m_Collection;
    }

    class XmlModelStorage : IModel
    {
        public XmlModelStorage(string path)
        {
            m_Filepath = path;
            m_Data = TryLoadXml<XmlData>(path) ?? new XmlData();
        }

        public ICRUD<Familiar> LookupFamiliar(int id)
        {
            return LookupGeneric(Data.Familiars, familiar => familiar.Id == id);
        }

        public ICRUD<OwnedFamiliar> LookupOwnedFamiliar(int id)
        {
            return LookupGeneric(Data.OwnedFamiliars, owned => owned.Id == id);
        }

        public void AddFamiliar(Familiar familiar)
        {
            Data.Familiars.Add(familiar);
            Save();
        }

        public void AddOwnedFamiliar(OwnedFamiliar ownedFamiliar)
        {
            Data.OwnedFamiliars.Add(ownedFamiliar);
            Save();
        }

        public XmlData Data => m_Data;

        public IEnumerable<int> Familiars => Data.Familiars.Select(familiar => familiar.Id);

        public IEnumerable<int> OwnedFamiliars => Data.OwnedFamiliars.Select(owned => owned.Id);

        public void Save()
        {
            SaveXml(m_Filepath, m_Data);
        }

        private ICRUD<ValueType> LookupGeneric<ValueType>(List<ValueType> collection, Func<ValueType, bool> predicate)
        {
            var value = collection.FirstOrDefault(predicate);

            if(value == null)
            {
                return null;
            }

            return new XmlCRUD<ValueType>(this, collection, value);
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
        
        private XmlData m_Data;
        private string m_Filepath;
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
    class XmlData
    {
        public XmlData()
        {
            Familiars = new List<Familiar>();
            OwnedFamiliars = new List<OwnedFamiliar>();
        }

        [DataMember]
        public List<Familiar> Familiars { get; private set; }

        [DataMember]
        public List<OwnedFamiliar> OwnedFamiliars { get; private set; }
    }
}
