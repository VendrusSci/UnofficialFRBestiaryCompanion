using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace Bestiary.Model
{
    public interface IModel
    {
        ICRUD<Familiar> LookupFamiliar(int id);
        ICRUD<OwnedFamiliar> LookupOwnedFamiliar(int id);
        ICRUD<BookmarkedFamiliar> LookupBookmarkedFamiliar(int id);
        IEnumerable<int> Familiars { get; }
        IEnumerable<int> OwnedFamiliars { get; }
        IEnumerable<int> BookmarkedFamiliars { get; }
        void AddFamiliar(Familiar familiar);
        void AddOwnedFamiliar(OwnedFamiliar ownedFamiliar);
        void AddBookmarkedFamiliar(BookmarkedFamiliar bookmarkedFamiliar);
    }

    class XmlCRUD<ValueType> : ICRUD<ValueType>
    {
        public XmlCRUD(Action saveFunc, List<ValueType> collection, ValueType value)
        {
            m_Collection = collection;
            m_SaveFunc = saveFunc;
            m_Value = value;
        }

        public void Delete()
        {
            m_Collection.Remove(m_Value);
            m_SaveFunc();
        }

        public ValueType Fetch()
        {
            return m_Value;
        }

        public void Update(Action<ValueType> update)
        {
            update(m_Value);
            m_SaveFunc();
        }

        ValueType m_Value;
        Action m_SaveFunc;
        List<ValueType> m_Collection;
    }

    class XmlModelStorage : IModel
    {
        public XmlModelStorage(string familiarPath, string ownedFamiliarPath, string bookmarkedPath)
        {
            m_FamiliarPath = familiarPath;
            m_OwnedFamiliarPath = ownedFamiliarPath;
            m_BookmarkedPath = bookmarkedPath;

            m_FRData = TryLoadXml<XmlData<Familiar>>(familiarPath) ?? new XmlData<Familiar>();
            if(!String.IsNullOrEmpty(ownedFamiliarPath))
            {
                m_UserData = TryLoadXml<XmlData<OwnedFamiliar>>(ownedFamiliarPath) ?? new XmlData<OwnedFamiliar>();
            }
            if (!String.IsNullOrEmpty(bookmarkedPath))
            {
                m_BookmarkData = TryLoadXml<XmlData<BookmarkedFamiliar>>(bookmarkedPath) ?? new XmlData<BookmarkedFamiliar>();
            }
        }

        public ICRUD<Familiar> LookupFamiliar(int id)
        {
            return LookupGeneric(SaveFrData, FRData.Values, familiar => familiar.Id == id);
        }

        public ICRUD<OwnedFamiliar> LookupOwnedFamiliar(int id)
        {
            return LookupGeneric(SaveUserData, UserData.Values, owned => owned.Id == id);
        }

        public ICRUD<BookmarkedFamiliar> LookupBookmarkedFamiliar(int id)
        {
            return LookupGeneric(SaveBookmarkData, BookmarkData.Values, bookmarked => bookmarked.Id == id);
        }

        public void AddFamiliar(Familiar familiar)
        {
            FRData.Values.Add(familiar);
            SaveFrData();
        }

        public void AddOwnedFamiliar(OwnedFamiliar ownedFamiliar)
        {
            UserData.Values.Add(ownedFamiliar);
            SaveUserData();
        }

        public void AddBookmarkedFamiliar(BookmarkedFamiliar bookmarkedFamiliar)
        {
            BookmarkData.Values.Add(bookmarkedFamiliar);
            SaveBookmarkData();
        }

        public XmlData<Familiar> FRData => m_FRData;
        public XmlData<OwnedFamiliar> UserData => m_UserData;
        public XmlData<BookmarkedFamiliar> BookmarkData => m_BookmarkData;

        public IEnumerable<int> Familiars => FRData.Values.Select(familiar => familiar.Id);
        public IEnumerable<int> OwnedFamiliars => UserData.Values.Select(owned => owned.Id);
        public IEnumerable<int> BookmarkedFamiliars => BookmarkData.Values.Select(bookmarked => bookmarked.Id);

        public void SaveFrData()
        {
            SaveXml(m_FamiliarPath, m_FRData);
        }

        public void SaveUserData()
        {
            SaveXml(m_OwnedFamiliarPath, m_UserData);
        }

        public void SaveBookmarkData()
        {
            SaveXml(m_BookmarkedPath, m_BookmarkData);
        }

        private ICRUD<ValueType> LookupGeneric<ValueType>(Action saveFunc, List<ValueType> collection, Func<ValueType, bool> predicate)
        {
            var value = collection.FirstOrDefault(predicate);

            if(value == null)
            {
                return null;
            }

            return new XmlCRUD<ValueType>(saveFunc, collection, value);
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
        
        private XmlData<Familiar> m_FRData;
        private XmlData<OwnedFamiliar> m_UserData;
        private XmlData<BookmarkedFamiliar> m_BookmarkData;
        private string m_FamiliarPath;
        private string m_OwnedFamiliarPath;
        private string m_BookmarkedPath;
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
    class XmlData<DataType>
    {
        public XmlData()
        {
            Values = new List<DataType>();
        }

        [DataMember]
        public List<DataType> Values { get; private set; }
    }
}
