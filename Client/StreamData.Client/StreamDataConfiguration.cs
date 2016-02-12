namespace StreamData.Client
{
    using System.Configuration;


    public class StreamDataConfiguration
    {
        public string ApiUrl { get; set; }
        public string SecretKey { get; set; }

        private ServerSentEventEngine engine;

        private string streamDataHost;

        private StreamDataConfigurationMode mode = StreamDataConfigurationMode.PRODUCTION;
        public StreamDataConfigurationMode Mode => mode;
        public ServerSentEventEngine Engine => engine;
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

        public void UseServerSentEventEngine<T>(T engine) where T : ServerSentEventEngine
        {
            this.engine = engine;
        }
        
    }
}