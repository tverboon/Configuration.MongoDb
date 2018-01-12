using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Midmid.Configuration.MongoDb
{
    public class MongoDbConfigurationProvider : ConfigurationProvider
    {
        public MongoDbConfigurationProvider(MongoDbConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            //Configure changetoken

            if (Source.ReloadOnChange && Source.ConfigurationReader != null)
            {
                ChangeToken.OnChange(
                    () => Source.ConfigurationReader.Watch(),
                    () =>
                    {
                        Load(reload: true);
                    });
            }
        }

        private MongoDbConfigurationSource Source { get; }

        public override void Load()
        {
            Load(reload: false);
        }

        private void Load(bool reload)
        {
            if (reload)
            {
                Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            //Add error handling
            Data = Source.ConfigurationReader?.GetAllConfiguration();
            OnReload();
        }
    }
}