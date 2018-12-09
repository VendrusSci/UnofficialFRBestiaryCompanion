using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    public enum BookmarkState
    {
        Bookmarked,
        NotBookmarked
    }

    [DataContract]
    public class BookmarkedFamiliar : IFamiliarData
    {
        [DataMember]
        public int Id { get; private set; }

        public BookmarkedFamiliar(int id)
        {
            Id = id;
        }
    }
}
