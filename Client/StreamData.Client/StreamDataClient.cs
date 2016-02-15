using Marvin.JsonPatch;
using Marvin.JsonPatch.Operations;

namespace StreamData.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;


    public class StreamDataClient<T> where T : class
    {
        public StreamDataConfiguration Configuration { get; }
        T state = default(T);
        public T State => state;

        private StreamDataClient(StreamDataConfiguration config)
        {
            Configuration = config;
            if (Configuration.KeepState)
            {
                Configuration.Engine.OnNewJsonData += (data) =>
                {
                    T value = JsonConvert.DeserializeObject<T>(data);
                    state = value;
                };
                Configuration.Engine.OnNewJsonPatch += (patch) =>
                {
                    var operations = JsonConvert.DeserializeObject<List<Operation<T>>>(patch);
                    var patchDocumentOperations = new JsonPatchDocument<T>(operations);
                    patchDocumentOperations.ApplyTo(state);
                };
            }
        }
        public static StreamDataClient<T> WithDefaultConfiguration() 
            => WithConfiguration(c => { });

        public static StreamDataClient<T> WithConfiguration(Action<StreamDataConfiguration> configure)
        {
            var configuration = StreamDataConfiguration.Default;
            configure(configuration);
            EnsureSecretKeyPresentInProductionMode(configuration);
            return new StreamDataClient<T>(configuration);
        }

        private static void EnsureSecretKeyPresentInProductionMode(StreamDataConfiguration configurationToVerify)
        {
            if (configurationToVerify.Mode == StreamDataConfigurationMode.PRODUCTION
                 && string.IsNullOrEmpty(configurationToVerify.SecretKey))
                throw new StreamDataConfigurationException("[SecretKey] not configured");

        }


        public void OnData<T>(Action<T> action)
        {
            Configuration.Engine.OnNewJsonData += (data) =>
            {
                T value = JsonConvert.DeserializeObject<T>(data);
                action(value);
            };
        }

        public void OnPatch<T>(Action<JsonPatchDocument<T>> actionWithPatch) where T:class
        {
            Configuration.Engine.OnNewJsonPatch += (patch) =>
            {
                var operations = JsonConvert.DeserializeObject<List<Operation<T>>>(patch);
                var patchDocumentOperations = new JsonPatchDocument<T>(operations);
                actionWithPatch(patchDocumentOperations);
            };

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
