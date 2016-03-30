namespace StreamData.StockMarket.Demo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Streamdata.Client;
    using Newtonsoft.Json;

    public class Program
    {
        public static void Main(params string[] args)
        {
            if (args.Length < 1) throw new Exception("please start app with 1 or 2 as argument");

            var stockMarketApiTestUrl = "http://stockmarket.streamdata.io/prices";
            var secretKey = "fill-your-secret-key-here-to-use-the-production-service";

            if (args[0] == "1")
                ShowMeHowToKeepAnUpdatedState(stockMarketApiTestUrl, secretKey);
            if (args[0] == "2")
                ShowMeHowToGetDataAndPatches(stockMarketApiTestUrl);
        }


        public static void ShowMeHowToKeepAnUpdatedState(
            string stockMarketApiTestUrl,
            string secretKey)
        {
            var client = StreamdataClient<StockMarketOrders>
                .WithConfiguration(conf => {
                    conf.SecretKey = secretKey;
                    conf.KeepStateUpdated();
                });


            int consoleWidth = 48;
            int counter = 0;

            StockMarketOrders actual = new StockMarketOrders();

            client.OnUpdatedState += (orders) => {
                Console.Clear();
                counter++;
                PrintOrders(consoleWidth, counter, orders, actual);
                Console.WriteLine("...press [esc] to quit app");
                actual = orders.Clone();
            };

            client.Start(stockMarketApiTestUrl);

            while ((Console.ReadKey().Key) != ConsoleKey.Escape)
            {
                client.Stop();
                Console.WriteLine("stopped...");
            }
        }

        public static void ShowMeHowToGetDataAndPatches(
            string stockMarketApiTestUrl)
        {
            var client = StreamdataClient<StockMarketOrders>
                .WithConfiguration(conf => {
                    conf.UseSandbox();
                });

            int consoleWidth = 48;
            int counter = 0;

            StockMarketOrders previousOrders = new StockMarketOrders(); ;
            StockMarketOrders actualOrders = new StockMarketOrders(); ;

            client.OnData(data => {
                Console.Clear();
                counter++;
                actualOrders = data;
                PrintOrders(consoleWidth, counter, data, previousOrders);
                previousOrders = data.Clone();
            });

            client.OnPatch(patch => {
                Console.Clear();
                counter++;
                patch.ApplyTo(actualOrders);
                PrintOrders(consoleWidth, counter, actualOrders, previousOrders);
                previousOrders = actualOrders.Clone();
                var json = JsonConvert.SerializeObject(patch);
                Console.WriteLine("patch:{0}",json);
            });

            client.Start(stockMarketApiTestUrl);

            while ((Console.ReadKey().Key) != ConsoleKey.Escape)
            {
                client.Stop();
                Console.WriteLine("stopped...");
            }
        }

        public static void PrintOrders(
            int width, int counter, StockMarketOrders orders,
            StockMarketOrders previous
        )
        {
            int colWidth = (width / 3) - 4;
            Console.Clear();
            var actualColor = Console.ForegroundColor;

            var lines = orders
                .Select(order => {
                    var existingOrder = previous
                            .SingleOrDefault(x => x.Title == order.Title) ?? order;

                    return new
                    {
                        order.Title,
                        order.Price,
                        Previous = (order.Price == existingOrder.Price)
                                    ? " * "
                                    : existingOrder.Price.ToString()
                    };
                })
                .Select(order =>
                 "| " + order.Title?.PadRight(colWidth)
               + " | " + order.Price.ToString().PadRight(colWidth)
               + " | " + order.Previous.PadRight(colWidth)
               + "  |");

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(
                new string('-', width - 1) + Environment.NewLine
                + "| " + "Title".PadRight(colWidth)
                + " | " + "Price".PadRight(colWidth)
                + " | " + "Previous".PadRight(colWidth)
                + "  |");

            Console.WriteLine(
                new string('-', width - 1) + Environment.NewLine
                + string.Join(Environment.NewLine, lines));

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(
                new string('-', width - 1) + Environment.NewLine
                + "| " + "Total".PadRight(colWidth)
                + " | " + orders.GetTotal().ToString().PadRight(colWidth)
                + " | " + previous.GetTotal().ToString().PadRight(colWidth)
                + "  |" + Environment.NewLine
                + new string('-', width - 1) + Environment.NewLine);

            Console.ForegroundColor = actualColor;
            Console.WriteLine("Updated {0} times at {1}",counter,DateTime.Now);
        }

    }

    public class Order
    {
        public string Title { get; set; }
        public int Price { get; set; }

        public Order Clone()
        {
            return new Order
            {
                Title = this.Title,
                Price = this.Price
            };
        }
    }

    public class StockMarketOrders : List<Order>
    {
        public int GetTotal()
        {
            return this.Select(x => x.Price).Sum();
        }

        public StockMarketOrders Clone()
        {
            var cloned = new StockMarketOrders();
            this.ForEach(order => cloned.Add(order.Clone()));
            return cloned;
        }
    }
}