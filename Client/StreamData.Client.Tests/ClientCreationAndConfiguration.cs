namespace StreamData.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NFluent;
    using Xunit;

    public class ClientCreationAndConfiguration
    {
        [Fact]
        public void
        WHEN_i_test_my_walking_skeleton_THEN_it_should_work()
        {
            Check.That(false).IsFalse();
        }

        [Fact]
        public void
        WHEN_I_want_to_create_new_instance_THEN_i_must_use_configuration_helper()
        {
            var client = StreamDataClient.WithConfiguration(conf =>
            {
                conf.ApiUrl = "http://fakeurl";
                conf.UseSandbox();
            });
            Check.That(client.Configuration.ApiUrl).IsEqualTo("http://fakeurl");
        }
        
    }
}