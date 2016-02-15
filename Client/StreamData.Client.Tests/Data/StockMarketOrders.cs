using System.Collections.Generic;
using System.Linq;

namespace StreamData.Client.Tests.Data
{
    public class StockMarketOrders : List<Order>
    {
        public int GetTotal()
        {
            return this.Select(x => x.Price).Sum();
        }
    }
}