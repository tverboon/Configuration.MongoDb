using System;
using Midmid.Configuration.MongoDb;

namespace Microsoft.Extensions.Configuration
{
    public static class MongoDbConfigurationExtensions
    {
        public static IConfigurationBuilder AddMongoDb(this IConfigurationBuilder configurationBuilder, string connectionString, bool reloadOnChange)
        {
            return AddMongoDb(configurationBuilder, new DefaultMongoDbReader(connectionString), "AppSettings");
        }

        public static IConfigurationBuilder AddMongoDb(this IConfigurationBuilder configurationBuilder, string connectionString, string collectionName)
        {
            return AddMongoDb(configurationBuilder, new DefaultMongoDbReader(connectionString), collectionName);
        }

        public static IConfigurationBuilder AddMongoDb(this IConfigurationBuilder configurationBuilder, IMongoDbReader mongoDbReader)
        {
            return AddMongoDb(configurationBuilder, mongoDbReader, "AppSettings", reloadOnChange);
        }

        public static IConfigurationBuilder AddMongoDb(
            this IConfigurationBuilder configurationBuilder,
            IMongoDbReader mongoDbReader,
            string collectionName,
            bool reloadOnChange)
        {
            if (mongoDbReader == null)
            {
                throw new ArgumentNullException(nameof(mongoDbReader));
            }

            if (collectionName == null)
            {
                throw new ArgumentNullException(nameof(collectionName));
            }
            var source = new MongoDbConfigurationSource
            {
                ReloadOnChange = reloadOnChange,
                ConfigurationReader = new MongoDbConfigurationReader(mongoDbReader, collectionName)
            };
            configurationBuilder.Add(source);
            return configurationBuilder;
        }

        public static IConfigurationBuilder AddMongoDb(this IConfigurationBuilder builder, Action<MongoDbConfigurationSource> configureSource)
            => builder.Add(configureSource);
    }
}
