﻿namespace StreamData.Client
{
    using System.Configuration;


    public class StreamDataConfiguration
    {
        public string ApiUrl { get; set; }
        private string streamDataHost;
        private StreamDataConfigurationMode mode = StreamDataConfigurationMode.PRODUCTION;
        public StreamDataConfigurationMode Mode => mode;
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
        
        public string SecretKey { get; set; }
    }
}