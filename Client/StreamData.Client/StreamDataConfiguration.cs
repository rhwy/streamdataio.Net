using System;
using System.Configuration;

namespace Streamdata.Client
{
    public class StreamdataConfiguration
    {
        public string ApiUrl { get; set; }
        public string SecretKey { get; set; }

        private Tuple<Type,object[]> engineType = Tuple.Create<Type, object[]>(typeof(EventSourceServerSentEngine),null);
        
        private bool keepState = false;
        public bool KeepState => keepState;
        private string streamDataHost;
        public string StreamDataHost => streamDataHost;
        private StreamdataConfigurationMode mode = StreamdataConfigurationMode.PRODUCTION;
        public StreamdataConfigurationMode Mode => mode;
        public Tuple<Type, object[]> EngineType => engineType;

        public static StreamdataConfiguration Default
            => new StreamdataConfiguration()
            {
                ApiUrl = ConfigurationManager.AppSettings["streamdata:apiurl"],
                SecretKey = ConfigurationManager.AppSettings["streamdata:secretkey"]
            };

        public StreamdataConfiguration()
        {
            UseProduction();
        }

        public void UseSandbox()
        {
            mode = StreamdataConfigurationMode.SANDBOX;
            streamDataHost = StreamdataOfficialUrls.SANDBOX;
        }

        public void UseProduction()
        {
            mode = StreamdataConfigurationMode.PRODUCTION;
            streamDataHost = StreamdataOfficialUrls.PRODUCTION;
        }

        public void UserServerSentEventEngine<T>(params object[] constructorArgs) where T:ServerSentEventEngine
        {
            this.engineType = Tuple.Create(typeof (T),constructorArgs);
        }
        public void KeepStateUpdated()
        {
            keepState = true;
        }

        public string BuildUrl(string url)
        {
            return $"{streamDataHost}/{url}?X-Sd-Token={SecretKey}";
        }
    }
}