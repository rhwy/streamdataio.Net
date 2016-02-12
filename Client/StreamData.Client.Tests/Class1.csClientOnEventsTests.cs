

namespace StreamData.Client.Tests
{
    using System;
    using Newtonsoft.Json;
    using NFluent;
    using StreamData.Client;
    using StreamData.Client.Tests.Data;
    using Xunit;

    class FakeEngine : ServerSentEventEngine
    {
        public void SendData<T>(T value) => jsonDataAction(JsonConvert.SerializeObject(value));
        public void SendPatch<T>(T value) => jsonPatchAction(JsonConvert.SerializeObject(value));
        private Action<string> jsonDataAction;
        private Action<string> jsonPatchAction;
        public void OnNewJsonData(Action<string> jsonAction) => this.jsonDataAction = jsonAction;
        public void OnNewJsonPatch(Action<string> jsonAction) => this.jsonPatchAction = jsonAction;
    }


    public class ClientOnEventsTests
    {

        [Fact]
        public void
        WHEN_client_recieves_data_THEN_it_should_call_ondata()
        {
            FakeEngine fakeEngine = new FakeEngine();
            var client = StreamDataClient.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UseServerSentEventEngine(fakeEngine);
            });

            StockMarketOrders testData = null;
            StockMarketOrders expectedMarketOrders = new StockMarketOrders();
            expectedMarketOrders.Add(new Order() { Price = 10, Title = "test1" });
            expectedMarketOrders.Add(new Order() { Price = 20, Title = "test2" });

            client.OnData<StockMarketOrders>(data => testData = data);

            client.Start("fakeurl");

            fakeEngine.SendData(expectedMarketOrders);

            Check.That(testData).IsNotNull();
            Check.That(testData.GetTotal()).IsEqualTo(30);
        }
    }
}
