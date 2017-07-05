using System;

namespace Fiver.Azure.NoSql
{
    public class AzureNoSqlSettings
    {
        public AzureNoSqlSettings(string endpoint, string authKey,
                                  string databaseId, string collectionId)
        {
            if (string.IsNullOrEmpty(endpoint))
                throw new ArgumentNullException("Endpoint");

            if (string.IsNullOrEmpty(authKey))
                throw new ArgumentNullException("AuthKey");

            if (string.IsNullOrEmpty(databaseId))
                throw new ArgumentNullException("DatabaseId");

            if (string.IsNullOrEmpty(collectionId))
                throw new ArgumentNullException("CollectionId");

            this.Endpoint = endpoint;
            this.AuthKey = authKey;
            this.DatabaseId = databaseId;
            this.CollectionId = collectionId;
        }

        public string Endpoint { get; }
        public string AuthKey { get; }
        public string DatabaseId { get; }
        public string CollectionId { get; }
    }
}
