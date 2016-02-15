using System;

namespace StreamData.Client
{
    using System.Configuration;


    public class StreamDataConfiguration
    {
        public string ApiUrl { get; set; }
        public string SecretKey { get; set; }

        private Tuple<Type,object[]> engineType = Tuple.Create<Type, object[]>(typeof(EventSourceServerSentEngine),null);
        
        private bool keepState = false;
        public bool KeepState => keepState;
        private string streamDataHost;

        private StreamDataConfigurationMode mode = StreamDataConfigurationMode.PRODUCTION;
        public StreamDataConfigurationMode Mode => mode;
        public Tuple<Type, object[]> EngineType => engineType;

        public static StreamDataConfiguration Default
            => new StreamDataConfiguration()
            {
                ApiUrl = ConfigurationManager.AppSettings["streamdata:apiurl"],
                SecretKey = ConfigurationManager.AppSettings["streamdata:secretkey"]
            };

        public StreamDataConfiguration()
        {
            UseProduction();
        }

        public void UseSandbox()
        {
            mode = StreamDataConfigurationMode.SANDBOX;
            streamDataHost = StreamDataOfficialUrls.SANDBOX;
        }

        public void UseProduction()
        {
            mode = StreamDataConfigurationMode.PRODUCTION;
            streamDataHost = StreamDataOfficialUrls.PRODUCTION;
        }

        public void UserServerSentEventEngine<T>(params object[] constructorArgs) where T:ServerSentEventEngine
        {
            this.engineType = Tuple.Create(typeof (T),constructorArgs);
        }
        public void KeepStateUpdated()
        {
            keepState = true;
        }
    }
}