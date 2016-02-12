namespace StreamData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;


    public class StreamDataClient
    {
        public StreamDataConfiguration Configuration { get; }
        private StreamDataClient(StreamDataConfiguration config)
        {
            Configuration = config;
        }
        public static StreamDataClient WithDefaultConfiguration() => WithConfiguration(c => { });

        public static StreamDataClient WithConfiguration(Action<StreamDataConfiguration> configure)
        {
            var configuration = StreamDataConfiguration.Default;
            configure(configuration);
            EnsureSecretKeyPresentInProductionMode(configuration);
            return new StreamDataClient(configuration);
        }

        private static void EnsureSecretKeyPresentInProductionMode(StreamDataConfiguration configuration)
        {
            if (configuration.Mode == StreamDataConfigurationMode.PRODUCTION
                 && string.IsNullOrEmpty(configuration.SecretKey))
                throw new StreamDataConfigurationException("[SecretKey] not configured");

        }


        
        
    }
}
