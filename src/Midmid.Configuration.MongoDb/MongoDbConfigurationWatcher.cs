using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Midmid.Configuration.MongoDb
{
    public class MongoDbConfigurationWatcher
    {
        private readonly MongoDbConfigurationReader _configurationReader;
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();
        private byte[] _previousHash;

        public MongoDbConfigurationWatcher(MongoDbConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader ?? throw new ArgumentNullException(nameof(configurationReader));
        }

        public IChangeToken GetChangeToken()
        {
            return _reloadToken;
        }

        public void Start()
        {
            if (IsStopped)
            {
                IsStopped = false;
                var t = RunLoop();
            }
        }

        public void Stop()
        {
            IsStopped = true;
        }

        protected void OnReload()
        {
            var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

        private bool IsStopped { get; set; } = false;

        private async Task RunLoop()
        {
            while (!IsStopped)
            {
                await Task.Delay(PollingInterval);
                if (HasChanged())
                {
                    OnReload();
                }
            }
        }

        private static TimeSpan PollingInterval => TimeSpan.FromSeconds(30);

        private bool HasChanged()
        {
            var configData = _configurationReader.GetAllConfiguration();
            var hash = GetConfigurationHash(configData);
            if (_previousHash == null || !hash.SequenceEqual(_previousHash))
            {
                _previousHash = hash;
                return true;
            }
            return false;
        }

        private byte[] GetConfigurationHash(Dictionary<string, string> configData)
        {
            var keys = String.Join(string.Empty, configData.Keys.ToArray());
            var values = String.Join(string.Empty, configData.Values.ToArray());
            using (var sha = SHA1.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(keys + values));
            }
        }
    }
}