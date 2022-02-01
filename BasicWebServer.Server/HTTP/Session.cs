using BasicWebServer.Server.Common;
using System.Collections.Generic;

namespace BasicWebServer.Server.HTTP
{
    public class Session
    {
        public const string SessionCookieName = "MyWebServerSID";
        public const string SessionCurrentDateKey = "CurrentDate";
        public const string SessionUserKey = "AuthenticatedUserId";


        public Session(string id)
        {
            Guard.AgainstNull(id, nameof(id));

            this.data = new Dictionary<string, string>();

            this.Id = id;
        }

        private Dictionary<string, string> data;

        public string Id { get; init; }

        public string this[string key]{
            get => this.data[key];
            set => this.data[key] = value;
        }

        public bool ContainsKey(string key) 
            => this.data.ContainsKey(key);

        public void Clear()
            => this.data.Clear();
    }
}
