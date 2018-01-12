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

            if (Source.ReloadOnChange && Source.ConfigurationReader != null)
            {
                ChangeToken.OnChange(
                    () => Source.ConfigurationReader.Watch(),
                    () =>
                    {
                        Load(reload: true);
                    });
                //Should be started in Watch()
                Source.ConfigurationReader.ForceReload();
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
            Data = Source.ConfigurationReader?.GetAllConfiguration();

            OnReload();
        }
    }
}