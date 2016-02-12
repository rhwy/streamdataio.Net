using System.Collections.Generic;
using Marvin.JsonPatch;
using Marvin.JsonPatch.Operations;
using NFluent;
using StreamData.Client.Tests.Data;
using Xunit;
using Newtonsoft.Json;
using System.Linq;

namespace StreamData.Client.Tests
{
    public class EnsureJsonPatchWorksWithTheModel
    {
        [Fact]
        public void EnsureTestDataOriginalValues()
        {
            var basket = JsonConvert.DeserializeObject<StockMarketOrders>(TestData.ORIGINAL_DATA);
            var sum = basket.Select(x => x.Price).Sum();
            Check.That(sum).IsEqualTo(718);
        }

        [Fact]
        public void WHEN_Patch1_is_applied_THEN_sum_change_to_correct_value()
        {
            var basket = JsonConvert.DeserializeObject<StockMarketOrders>(TestData.ORIGINAL_DATA);
            var patch = JsonConvert.DeserializeObject<List<Operation<StockMarketOrders>>>(TestData.PATCH_1);
            JsonPatchDocument<StockMarketOrders> doc = new JsonPatchDocument<StockMarketOrders>(patch);
            doc.ApplyTo(basket);
            var sum = basket.Select(x => x.Price).Sum();
            var expectedSum = 718 - 60 + 95;
            //on patch one only one value changed from 60 to 95
            Check.That(sum).IsEqualTo(expectedSum);
        }

        [Fact]
        public void WHEN_Patch2_is_applied_with_multiples_changes_THEN_sum_change_to_correct_value()
        {
            var basket = JsonConvert.DeserializeObject<StockMarketOrders>(TestData.ORIGINAL_DATA);
            var patch = JsonConvert.DeserializeObject<List<Operation<StockMarketOrders>>>(TestData.PATCH_2);
            JsonPatchDocument<StockMarketOrders> doc = new JsonPatchDocument<StockMarketOrders>(patch);
            doc.ApplyTo(basket);
            var sum = basket.Select(x => x.Price).Sum();
            //expected = original sum + sum(- item source price + item new price)
            var expectedSum = 718 + (-78 + 0) + (-77 + 13) + (-22 + 27);
            Check.That(sum).IsEqualTo(expectedSum);//=581
        }

        [Fact]
        public void WHEN_Two_patches_are_applied_THEN_sum_change_to_correct_value()
        {
            var basket = JsonConvert.DeserializeObject<StockMarketOrders>(TestData.ORIGINAL_DATA);
            var patch1 = JsonConvert.DeserializeObject<List<Operation<StockMarketOrders>>>(TestData.PATCH_1);
            var patch2 = JsonConvert.DeserializeObject<List<Operation<StockMarketOrders>>>(TestData.PATCH_2);
            JsonPatchDocument<StockMarketOrders> documentPatcher1 = new JsonPatchDocument<StockMarketOrders>(patch1);
            JsonPatchDocument<StockMarketOrders> documentPatcher2 = new JsonPatchDocument<StockMarketOrders>(patch2);
            documentPatcher1.ApplyTo(basket);
            documentPatcher2.ApplyTo(basket);
            var sum = basket.Select(x => x.Price).Sum();
            Check.That(sum).IsEqualTo(616);
        }


    }
}

