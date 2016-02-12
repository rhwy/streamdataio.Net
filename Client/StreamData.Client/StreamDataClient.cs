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
        public static StreamDataClient WithConfiguration(Action<StreamDataConfiguration> configure)
        {
            var configuration = StreamDataConfiguration.Default;
            configure(configuration);
            return new StreamDataClient(configuration);
        }
        public static StreamDataClient WithDefaultConfiguration() => WithConfiguration(c => { });

        private StreamDataClient(StreamDataConfiguration config)
        {
            Configuration = config;
        }
        
    }
}
