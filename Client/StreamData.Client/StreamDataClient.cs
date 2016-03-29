using System;
using System.Collections.Generic;
using Marvin.JsonPatch;
using Marvin.JsonPatch.Operations;
using Newtonsoft.Json;

namespace Streamdata.Client
{
    public class StreamdataClient<T> where T : class
    {
        public StreamdataConfiguration Configuration { get; }
        T state = default(T);
        public T State => state;
        private ServerSentEventEngine engine;
        public string ListenUrl => engine.Url;
        private StreamdataClient(StreamdataConfiguration config)
        {
            Configuration = config;
            engine = Activator.CreateInstance(
                Configuration.EngineType.Item1,
                Configuration.EngineType.Item2) as ServerSentEventEngine;

            if (Configuration.KeepState)
            {
                engine.OnNewJsonData += (data) =>
                {
                    T value = JsonConvert.DeserializeObject<T>(data);
                    state = value;
                    OnUpdatedState(state);
                };
                engine.OnNewJsonPatch += (patch) =>
                {
                    var operations = JsonConvert.DeserializeObject<List<Operation<T>>>(patch);
                    var patchDocumentOperations = new JsonPatchDocument<T>(operations);
                    patchDocumentOperations.ApplyTo(state);
                    OnUpdatedState(state);
                };
            }

        }
        public static StreamdataClient<T> WithDefaultConfiguration() 
            => WithConfiguration(c => { });

        public static StreamdataClient<T> WithConfiguration(Action<StreamdataConfiguration> configure)
        {
            var configuration = StreamdataConfiguration.Default;
            configure(configuration);
            EnsureSecretKeyPresentInProductionMode(configuration);
            return new StreamdataClient<T>(configuration);
        }

        private static void EnsureSecretKeyPresentInProductionMode(StreamdataConfiguration configurationToVerify)
        {
            if (configurationToVerify.Mode == StreamdataConfigurationMode.PRODUCTION
                 && string.IsNullOrEmpty(configurationToVerify.SecretKey))
                throw new StreamdataConfigurationException("[SecretKey] not configured");
        }


        public void OnData(Action<T> action)
        {
            engine.OnNewJsonData += (data) =>
            {
                T value = JsonConvert.DeserializeObject<T>(data);
                action(value);
            };
        }

        public void OnPatch(Action<JsonPatchDocument<T>> actionWithPatch) 
        {
            engine.OnNewJsonPatch += (patch) =>
            {
                var operations = JsonConvert.DeserializeObject<List<Operation<T>>>(patch);
                var patchDocumentOperations = new JsonPatchDocument<T>(operations);
                actionWithPatch(patchDocumentOperations);
            };
        }

        public event Action<T> OnUpdatedState = s => { };
        
        public void Start(string apiUrl=null)
        {
            Configuration.ApiUrl = apiUrl ?? Configuration.ApiUrl;
            var url = Configuration.BuildUrl(Configuration.ApiUrl);
            if (!engine.Start(url))
            {
                throw new StreamdataConfigurationException("ServerSentEvents engine can not be started");
            }
        }

        public void Stop()
        {
            if(engine != null && !engine.Stop())
                throw new StreamdataConfigurationException("there was a problem while stopping engine");
        }
    }
}
