using System;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Midmid.Configuration.MongoDb
{
    public class MongoDbConfigurationReader
    {
        private readonly IMongoDbReader _mongoDbReader;
        private readonly string _collectionName;

        public MongoDbConfigurationReader(IMongoDbReader mongoDbReader, string collectionName)
        {
            _mongoDbReader = mongoDbReader ?? throw new ArgumentNullException(nameof(mongoDbReader));
            _collectionName = collectionName ?? throw new ArgumentNullException(nameof(collectionName));
        }

        public IChangeToken Watch()
        {
            var watcher = new MongoDbConfigurationWatcher(this);
            watcher.Start();
            return watcher.GetChangeToken();
        }

        public Dictionary<string, string> GetAllConfiguration()
        {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string key, value;
            var documents = _mongoDbReader.FindAll(_collectionName);

            foreach (var document in documents)
            {
                key = document[@"Key"].AsString;
                value = document[@"Value"].AsString;

                data[key] = value;
            }
            return data;
        }
    }
}