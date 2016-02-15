using System.Collections.Generic;
using Marvin.JsonPatch.Operations;

namespace StreamData.Client.Tests
{
    using NFluent;
    using StreamData.Client;
    using StreamData.Client.Tests.Data;
    using Xunit;
    using Marvin.JsonPatch;


    public class ClientOnEventsTests
    {
        [Fact]
        public void
        WHEN_client_starts_THEN_it_should_order_engine_to_start()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            client.Start("fakeurl");
            
            Check.That(FakeEngineWrapper.Instance.IsStarted).IsTrue();
        }
        [Fact]
        public void
        WHEN_client_starts_THEN_engine_apiurl_should_be_set()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            client.Start("fakeurl");

            Check.That(client.Configuration.ApiUrl).IsEqualTo("fakeurl");
        }
        [Fact]
        public void
        WHEN_client_recieves_data_THEN_it_should_call_ondata()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            StockMarketOrders testData = null;
            StockMarketOrders expectedMarketOrders = new StockMarketOrders();
            expectedMarketOrders.Add(new Order() { Price = 10, Title = "test1" });
            expectedMarketOrders.Add(new Order() { Price = 20, Title = "test2" });

            client.OnData(data => testData = data);

            client.Start("fakeurl");

            FakeEngineWrapper.Instance.SendData(expectedMarketOrders);

            Check.That(testData).IsNotNull();
            Check.That(testData.GetTotal()).IsEqualTo(30);
        }

        [Fact]
        public void
        WHEN_client_recieves_patch_THEN_it_should_call_onpatch_and_updateddata()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            StockMarketOrders testData = null;
            StockMarketOrders expectedMarketOrders = new StockMarketOrders();
            expectedMarketOrders.Add(new Order() { Price = 10, Title = "test1" });
            expectedMarketOrders.Add(new Order() { Price = 20, Title = "test2" });

            client.OnData(data => testData = data);
            client.OnPatch(patch => patch.ApplyTo(testData));
            client.Start("fakeurl");

            FakeEngineWrapper.Instance.SendData(expectedMarketOrders);
            var patchDoc = new JsonPatchDocument<StockMarketOrders>(new List<Operation<StockMarketOrders>>
            {
                new Operation<StockMarketOrders>("replace","0/price","0/price",30)
            });
            FakeEngineWrapper.Instance.SendPatch(patchDoc);
            Check.That(testData).IsNotNull();
			Check.That(testData).HasSize(2);
            Check.That(testData.GetTotal()).IsEqualTo(50);
        }

        [Fact]
        public void
       WHEN_engine_receives_data_without_ondata_configured_THEN_it_should_not_throw_exception()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            StockMarketOrders expectedMarketOrders = new StockMarketOrders();
            expectedMarketOrders.Add(new Order() { Price = 10, Title = "test1" });
            expectedMarketOrders.Add(new Order() { Price = 20, Title = "test2" });

            client.Start("fakeurl");

            Check.ThatCode(()=> FakeEngineWrapper.Instance.SendData(expectedMarketOrders)).DoesNotThrow();
            
        }


        [Fact]
        public void
        WHEN_client_has_no_data_THEN_state_is_default_and_dont_throw_exception()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            client.Start("fakeurl");
            StockMarketOrders actualState = null;
            Check.ThatCode(() => actualState = client.State)
                .DoesNotThrow();

            Check.That(actualState).IsEqualTo(default(StockMarketOrders));
        }


        [Fact]
        public void
        WHEN_client_has_data_and_patch_THEN_state_is_updated()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
                conf.KeepStateUpdated();
            });
            StockMarketOrders expectedMarketOrders = new StockMarketOrders();
            expectedMarketOrders.Add(new Order() { Price = 10, Title = "test1" });
            expectedMarketOrders.Add(new Order() { Price = 20, Title = "test2" });
            StockMarketOrders actualState=null;

            client.Start("fakeurl");
            client.OnUpdatedState += state => actualState = state;
            FakeEngineWrapper.Instance.SendData(expectedMarketOrders);
            var patch = new JsonPatchDocument<StockMarketOrders>(new List<Operation<StockMarketOrders>>
            {
                new Operation<StockMarketOrders>("replace","0/price","0/price",20)
            });
            FakeEngineWrapper.Instance.SendPatch(patch);
            Check.That(actualState).IsNotNull();
            Check.That(actualState.GetTotal()).IsEqualTo(40);
        }

        [Fact]
        public void
        WHEN_client_is_stopped_THEN_ensure_engine_is_stopped_before_destruction()
        {
            var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
            {
                conf.UseSandbox();
                conf.UserServerSentEventEngine<FakeEngine>();
            });

            client.Start("fakeurl");
            Check.ThatCode(() => client.Stop()).DoesNotThrow();

            Check.That(FakeEngineWrapper.Instance.IsStarted).IsFalse();
        }

        [Fact]
        public void
        WHEN_I_stop_client_THEN_ensure_engine_does_not_throw()
        {
            Check.ThatCode(() =>
            {
                var client = StreamDataClient<StockMarketOrders>.WithConfiguration(conf =>
                {
                    conf.UseSandbox();
                    conf.UserServerSentEventEngine<FakeEngine>();
                });
                client.Stop();

            }).DoesNotThrow();
        }
    }
}
