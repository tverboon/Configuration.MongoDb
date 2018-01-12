using Microsoft.Extensions.Configuration;

namespace Midmid.Configuration.MongoDb
{
    public class MongoDbConfigurationSource : IConfigurationSource
    {
        public MongoDbConfigurationReader ConfigurationReader { get; set; }

        public bool ReloadOnChange { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MongoDbConfigurationProvider(this);
        }
    }
}