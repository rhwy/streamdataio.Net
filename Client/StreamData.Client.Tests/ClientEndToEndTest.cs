using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using Marvin.JsonPatch.Operations;

namespace StreamData.Client.Tests
{
    using NFluent;
    using StreamData.Client;
    using StreamData.Client.Tests.Data;
    using Xunit;
    using Marvin.JsonPatch;


    public class ClientEndToEndTest
    {
        private string secretKey = null;
        public ClientEndToEndTest()
        {
            secretKey = ConfigurationManager.AppSettings["streamdata:secretkey"];
            if(string.IsNullOrEmpty(secretKey))
                throw new ConfigurationErrorsException(
                    "you must fill the key [streamdata:secretkey] in the App.Config with your personal StreamData secret key to test the service end to end");
        }

        [Fact]
        public void
        WHEN_client_starts_THEN_it_should_order_engine_to_start()
        {
            var client = StreamDataClient<StockMarketOrders>.WithDefaultConfiguration();
            StockMarketOrders orders = null;
            int counter = 0;
            var testApiUrl = "http://stockmarket.streamdata.io/prices";
            client.OnData(o=> orders = o);
            client.OnPatch(p=>counter++);
            client.Start(testApiUrl);
            var expectedUrl = $"{StreamDataOfficialUrls.PRODUCTION}/{testApiUrl}?X-Sd-Token={client.Configuration.SecretKey}";
            Check.That(client.ListenUrl).IsEqualTo(expectedUrl);
            //we wait 10s, after this time, we should already
            //have some first data and some updates
            Thread.Sleep(10*1000);
            Check.That(orders).IsNotNull();
            Check.That(counter).IsGreaterThan(1);
        }
    }
}
