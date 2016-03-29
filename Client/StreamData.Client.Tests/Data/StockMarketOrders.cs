using System.Collections.Generic;
using System.Linq;

namespace Streamdata.Client.Tests.Data
{
    public class StockMarketOrders : List<Order>
    {
        public int GetTotal()
        {
            return this.Select(x => x.Price).Sum();
        }
    }
}