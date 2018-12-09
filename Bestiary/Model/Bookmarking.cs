using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bestiary.Model
{
    public enum BookmarkState
    {
        [Description("Bookmark")]
        Bookmarked,
        [Description("No Bookmark")]
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
