using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BestiaryLauncher.Model
{
    [DataContract]
    public class ReleaseJson
    {
        [DataMember]
        public string url;
        [DataMember]
        public string assets_url;
        [DataMember]
        public string html_url;
        [DataMember]
        public string id;
        [DataMember]
        public string node_id;
        [DataMember]
        public string tag_name;
        [DataMember]
        public string target_commitish;
        [DataMember]
        public string name;
        [DataMember]
        public string draft;
        [DataMember]
        public Author author;
        [DataMember]
        public string prerelease;
        [DataMember]
        public string created_at;
        [DataMember]
        public string published_at;
        [DataMember]
        public List<Assets> assets;
        [DataMember]
        public string tarball_url;
        [DataMember]
        public string zipball_url;
        [DataMember]
        public string body;
    }

    [DataContract]
    public class Author
    {
        [DataMember]
        public string login;
        [DataMember]
        public string id;
        [DataMember]
        public string node_id;
        [DataMember]
        public string avatar_url;
        [DataMember]
        public string gravatar_id;
        [DataMember]
        public string url;
        [DataMember]
        public string html_url;
        [DataMember]
        public string followers_url;
        [DataMember]
        public string following_url;
        [DataMember]
        public string gists_url;
        [DataMember]
        public string starred_url;
        [DataMember]
        public string subscriptions_url;
        [DataMember]
        public string organizations_url;
        [DataMember]
        public string repos_url;
        [DataMember]
        public string events_url;
        [DataMember]
        public string received_events_url;
        [DataMember]
        public string type;
        [DataMember]
        public string site_admin;
    }

    [DataContract]
    public class Assets
    {
        [DataMember]
        public string url;
        [DataMember]
        public string id;
        [DataMember]
        public string node_id;
        [DataMember]
        public string name;
        [DataMember]
        public string label;
        [DataMember]
        public Author uploader;
    }
}
