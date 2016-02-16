# StreamData-DotNet

## About

A .Net client and demo for StreamData.io service

StreamData.io is a proxy service that queries your REST apis from one side and 
send to clients in real time the updates with JsonPatch.

The service relies on 2 main standards [SSE (Server Sent Events)](https://en.wikipedia.org/wiki/Server-sent_events)
and [JsonPatch](https://tools.ietf.org/html/rfc6902) and nothing else. 
That means that you can connect to the service with your own tools and methods.
This client is only here to provide you an easier way to use the service.

StreamData.io service provides you a Production endpoint with limited access 
that you need to query from your account with a SecretKey, but also a Sandbox
endpoint that you can use to test the service without key or account (or in development).

They also provide a sample rest api with frequent updates that you can use to play with the service: 
[http://stockmarket.streamdata.io/prices](http://stockmarket.streamdata.io/prices).

## Install

The easiest way to use the service is to install the [StreamData Client](https://www.nuget.org/packages/StreamData.Client/).


    Install-Package StreamData.Client
    
 
 ## Let's start!
 Once installed, you can use the client which is the only thing you need to know in the api.
 
 The client is strongly types in order to simplify other interactions with the api.
 It is also build with some conventions for easy use and with a fluent
 configuration api. 
 
 By default we're using :
 
 * production endpoint
 * load secretkey from app.config
 * do not keep track of the state
 * use the default SSE library provided
 
 With that in mind, we'll create the client like this from the helper factory (with model from the demo [Stock market service](http://stockmarket.streamdata.io/prices)):
 
        var client = StreamDataClient<StockMarketOrders>
                  .WithDefaultConfiguration();
      
 
 With the client, you need now to define what to do when data is available from the api and what to do when there is some updates:
 
        client.OnData(orders => Console.WriteLine(orders));
        client.OnPatch(patch=> Console.WriteLine(patch));
  
  Then, you need to start listening the api (and provide some stop mechanism is you're in a console app):
  
        client.Start("http://stockmarket.streamdata.io/prices");
            
        while ((Console.ReadKey().Key) != ConsoleKey.Escape)
        {
            client.Stop();
            Console.WriteLine("stopped...");
        }    
 
 Please note that the client will build the StreamData urls automatically for PRODUCTION or SANDBOX modes and that you just have to provide your API url.
 
 ## Keeping state scenario
 
 Let's see another scenario with some additional configuration in order to test
 the keeping state mode with the sandbox service. By keeping state, we mean that client will keep the value provided by your api up to date by applying the patches.
 
 By Using the state, you'll have a special event that informs you that a new version of your value is available.
 
 Let's do a simple app, that keep the sum of the stock market prices up to date in a console app:
 
 Configure client with sandbox endpoint and state:
 
        var client = StreamDataClient<StockMarketOrders>
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
  
  Please clone the repo and test the demo applications withing the Demos folder to have a better overview of the apis, but it's really as simple and efficient as explained here, there's nothing much more.
  
  There is actually 2 demos (with same basically the same content):
  
  For both you'll have a sample usage of:
  
  * the 2 modes: Patch and State
  * a table printed with all the values of the stock market demo service and a sum of the prices
  * an additional column is provided with previous values (changed ones are showned, the sames are `*`)


  ### One for VisualStudio / .Net 4+:
  
  Usual VS usage:
  
    * Just open the solution in [Demos/DotNet4](Demos/DotNet4)
    * restore nuget packages for solution
    * enter your secretkey where needed to test it in production
    * Run the app from VS
    
    
  ### One for the new DNX (.net core):
  
  Basically you can just copy the 2 files in [Demos/DnxClientDemo](Demos/DnxClientDemo): `app.cs` and `project.json`.
  
  Once you have a folder with them:
  
        # restore packages:
        dnu restore
        
        # start the app in patch mode with Sandbox
        dnx start-patch
    
        # or start the app with production endpoint (after providing your key)
        dnx start-state
  
  This was last tested with `DNX 1.0.0-rc1-update1` on `mono (linux/osx)`.
    
   ## End Word
   
   Please feel free to contribute, the service is amazingly simple and effecient but we can still find ways to improve the client to have an event better developer experience ;-).
   
   Happy code!