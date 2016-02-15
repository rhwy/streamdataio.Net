using System.Collections.Generic;
using Marvin.JsonPatch.Operations;

namespace StreamData.Client.Tests
{
    using System;
    using Newtonsoft.Json;
    using NFluent;
    using StreamData.Client;
    using StreamData.Client.Tests.Data;
    using Xunit;
    using Marvin.JsonPatch;

    class FakeEngine : ServerSentEventEngine
    {
        //[{"op":"replace","path":"/0/price","value":19},{"op":"replace","path":"/1/price","value":61},{"op":"replace","path":"/5/price","value":37},{"op":"replace","path":"/12/price","value":82}]

        public void SendData<T>(T value) => jsonDataAction(JsonConvert.SerializeObject(value));
        public void SendPatch<T>(JsonPatchDocument<T> doc) where T :class
        {
            jsonPatchAction(JsonConvert.SerializeObject(doc));
        }

        private bool isStarted = false;
        public bool IsStarted => isStarted;
        private Action<string> jsonDataAction = (_)=> {};
        private Action<string> jsonPatchAction = (_) => { };
        public void OnNewJsonData2(Action<string> jsonAction) => this.jsonDataAction = jsonAction;
        public void OnNewJsonPatch2(Action<string> jsonAction) => this.jsonPatchAction = jsonAction;
        public Action<Action<string>> OnNewJsonData { get; }
        public Action<Action<string>> OnNewJsonPatch { get; }

        public FakeEngine()
        {
            OnNewJsonData = jsonAction => this.jsonDataAction = jsonAction;
            OnNewJsonPatch = jsonAction => this.jsonPatchAction = jsonAction;
        }
        public bool Start()
        {
            isStarted = true;
            return true;
        }
    }


    public class ClientOnEventsTests
    {
        [Fact]
        public void
        WHEN_client_starts_THEN_it_should_order_engine_to_start()
        {
            FakeEngine fakeEngine = new FakeEngine();
            var client = StreamDataClient.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UseServerSentEventEngine(fakeEngine);
            });

            client.Start("fakeurl");
            
            Check.That(fakeEngine.IsStarted).IsTrue();
        }
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

        [Fact]
        public void
        WHEN_client_recieves_patch_THEN_it_should_call_onpatch_and_updateddata()
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
            client.OnPatch<StockMarketOrders>(patch => patch.ApplyTo(testData));
            client.Start("fakeurl");

            fakeEngine.SendData(expectedMarketOrders);
            expectedMarketOrders.Add(new Order() { Price = 30, Title = "test3" });
            fakeEngine.SendData(expectedMarketOrders);

            Check.That(testData).IsNotNull();
			Check.That(testData).HasSize(3);
            Check.That(testData.GetTotal()).IsEqualTo(60);
        }

        [Fact]
        public void
       WHEN_engine_receives_data_without_ondata_configured_THEN_it_should_not_throw_exception()
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

            client.Start("fakeurl");

            Check.ThatCode(()=> fakeEngine.SendData(expectedMarketOrders)).DoesNotThrow();
            
        }


        [Fact]
        public void
        WHEN_client_has_no_data_THEN_state_is_default_and_dont_throw_exception()
        {
            FakeEngine fakeEngine = new FakeEngine();
            var client = StreamDataClient.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UseServerSentEventEngine(fakeEngine);
            });

            client.Start("fakeurl");
            StockMarketOrders actualState = null;
            Check.ThatCode(() => actualState = client.State<StockMarketOrders>())
                .DoesNotThrow();

            Check.That(actualState).IsEqualTo(default(StockMarketOrders));
        }


        [Fact]
        public void
        WHEN_client_has_data_and_patch_THEN_state_is_updated()
        {
            FakeEngine fakeEngine = new FakeEngine();
            var client = StreamDataClient.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UseServerSentEventEngine(fakeEngine);
                conf.KeepStateUpdated();
            });
            StockMarketOrders expectedMarketOrders = new StockMarketOrders();
            expectedMarketOrders.Add(new Order() { Price = 10, Title = "test1" });
            expectedMarketOrders.Add(new Order() { Price = 20, Title = "test2" });

            client.Start("fakeurl");
            client.OnData<StockMarketOrders>(x=> {});
            client.OnPatch<StockMarketOrders>(x => { });
            fakeEngine.SendData(expectedMarketOrders);
            var patch = new JsonPatchDocument<StockMarketOrders>(new List<Operation<StockMarketOrders>>
            {
                new Operation<StockMarketOrders>("replace","0/price","0/price",20)
            });
            fakeEngine.SendPatch(patch);
            StockMarketOrders actualState = client.State<StockMarketOrders>();
            Check.That(actualState).IsNotNull();
            Check.That(actualState.GetTotal()).IsEqualTo(40);
        }
    }
}
