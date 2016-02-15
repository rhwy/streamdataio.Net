using Marvin.JsonPatch;

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

        private static void EnsureSecretKeyPresentInProductionMode(StreamDataConfiguration configurationToVerify)
        {
            if (configurationToVerify.Mode == StreamDataConfigurationMode.PRODUCTION
                 && string.IsNullOrEmpty(configurationToVerify.SecretKey))
                throw new StreamDataConfigurationException("[SecretKey] not configured");

        }

        

        public void OnData<T>(Action<T> action)
        {
            Configuration.Engine?.OnNewJsonData?.Invoke(json =>
            {
                T value = JsonConvert.DeserializeObject<T>(json);
                action(value);
            });
        }

        public void OnPatch<T>(Action<JsonPatchDocument<T>> actionWithPatch) where T:class
        {
            Configuration.Engine?.OnNewJsonPatch?.Invoke(json =>
            {
                
            });
        }

        public T State<T>()
        {
            return default(T);
        }

        public void Start(string apiUrl)
        {
            if (!Configuration.Engine.Start())
            {
                throw new StreamDataConfigurationException("ServerSentEvents engine can not be started");
            }
        }

    }
}
