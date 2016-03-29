# Streamdataio-DotNet

## About

A .Net client and demo for Streamdata.io service

Streamdata.io is a proxy service that queries your REST APIs from one side and sends updates to clients in real time with JsonPatch.

The service relies only on two main standards [SSE (Server Sent Events)](https://en.wikipedia.org/wiki/Server-sent_events) and [JsonPatch](https://tools.ietf.org/html/rfc6902). 
That means that you can connect to the service with your own tools and methods. This demo application provides you an easy way to use Streamdata.io service.

Streamdata.io service provides you a production end point with limited access that you need to query from your account with a SecretKey, but also a Sandbox end point that you can use to test the service without key or account (or in development).

They also provide a sample rest API with frequent updates that you can use to experiment with the service: [http://stockmarket.streamdata.io/prices](http://stockmarket.streamdata.io/prices).

## Install

The easiest way to use the service is to install the [Streamdata Client](https://www.nuget.org/packages/Streamdata.Client/).

    Install-Package Streamdata.Client

## Let's start! Once installed, you can use the client which is the only thing you need to know in the API.

The client uses strong types in order to simplify other interactions with the API. It is also built with some conventions and with a fluent configuration API for ease of use and convenience.

By default we're using :

* the production end point
* secret key loaded from app.config 
* the default provided SSE library

Note also that we do not keep track of the state.

With that in mind, we'll create the client application from the helper factory (with model from the demo API [Stock market service](http://stockmarket.streamdata.io/prices)):

        var client = StreamdataClient<StockMarketOrders>
            .WithDefaultConfiguration();

With the client, you need now to define what to do when data is available from the API and what to do when there are some updates:

        client.OnData(orders => Console.WriteLine(orders));
        client.OnPatch(patch=> Console.WriteLine(patch));

Then, you need to start listening to the API (and provide some stop mechanisms if you're in a console app):

        client.Start("http://stockmarket.streamdata.io/prices");

        while ((Console.ReadKey().Key) != ConsoleKey.Escape)
        {
            client.Stop();
            Console.WriteLine("stopped...");
        }    

Please note that the client will build the Streamdata URLs automatically for PRODUCTION or SANDBOX modes. Thus, you just have to provide your API url.

## Keeping state scenario

Let's look at another scenario with some additional configurations in order to test the "keeping state mode" with the sandbox service. 

By keeping the state, we mean that the client will keep the value provided by your API up to date by applying the patches.

By using the state, you'll have a special event that informs you that a new version of your value is available.

Let's do a simple app, that keeps the sum of the stock market prices up to date in a console app:

Configure client with sandbox end point and state:

        var client = StreamdataClient<StockMarketOrders>
                .WithConfiguration(conf=>{
                    conf.UseSandbox()
                    conf.KeepStateUpdated();
                });

Print Sum on each update:

        int counter=0;
        client.OnUpdatedState += 
            (orders)=> {
                    counter++;
                    Console.WriteLine($"Total : {orders.Sum()} (after {counter} patches)");
        };

Then, start listening:

      client.Start("http://stockmarket.streamdata.io/prices");

      while ((Console.ReadKey().Key) != ConsoleKey.Escape)
      {
          client.Stop();
          Console.WriteLine("stopped...");
      }

## More ?

Please clone the repo and test the demo applications within the Demos folder to have a better overview of the APIs, but it's really as simple and efficient as explained here, there's nothing much more.

There are actually two versions of the demo: One for VisualStudio/.Net 4+ and the other for the new DNX (.net core).

For both you'll have sample usage of:

 * the two modes: Patch and State
 * a table printed with all the values of the stock market demo service and a sum of the prices
 * an additional column is provided with previous values (changed values are shown, the unchanged values are marked `*`)

### Demo installation process for VisualStudio / .Net 4+:

Usual VS usage:

 * open the solution in [Demos/DotNet4](Demos/DotNet4)
 * restore nuget packages for solution
 * enter your secret key where needed to test it in production
 * run the app from VS

### Demo installation process for DNX:

Basically you can just copy the two files in [Demos/DnxClientDemo](Demos/DnxClientDemo): `app.cs` and `project.json`.

Once you have a folder with them:

        # restore packages:
        dnu restore

        # start the app in patch mode with Sandbox
        dnx start-patch

        # or start the app with production endpoint (after providing your key)
        dnx start-state

This was last tested with DNX 1.0.0-rc1-update1 on mono (linux/osx).

## Conclusion

Please feel free to contribute, the service is amazingly simple and efficient but we can still find ways to improve the client to have an even better developer experience ;-).

Happy coding!
