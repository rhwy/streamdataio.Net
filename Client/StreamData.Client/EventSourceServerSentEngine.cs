using System;
using System.Threading;
using EventSource4Net;
using slf4net;
using slf4net.Resolvers;

namespace Streamdata.Client
{
    public class EventSourceServerSentEngine : ServerSentEventEngine
    {
        public event Action<string> OnNewJsonData = (_)=> {};
        public event Action<string> OnNewJsonPatch = (_) => {};
        private CancellationTokenSource cancellationTokenSource;
        private EventSource es;
        private bool started = false;
        public bool Start(string url)
        {
            cancellationTokenSource = new CancellationTokenSource();
            listenUrl = url;
            es = new EventSource(new Uri(listenUrl), 1000);
            
            es.EventReceived += new EventHandler<ServerSentEventReceivedEventArgs>((o, e) =>
            {
                if (e.Message.EventType == "data")
                {
                    OnNewJsonData(e.Message.Data);
                }
                else if (e.Message.EventType == "patch")
                {
                    OnNewJsonPatch(e.Message.Data);
                }
                else
                {
                    Console.WriteLine(e.Message.ToString());
                }
            });
            es.Start(cancellationTokenSource.Token);
            started = true;
            return true;
        }

        public bool Stop()
        {
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel(false);
            }
            return true;
        }

        public EventSourceServerSentEngine()
        {
            var defaultFactoryResolver = NOPLoggerFactoryResolver.Instance;
            LoggerFactory.SetFactoryResolver(defaultFactoryResolver);
        }

        ~EventSourceServerSentEngine()
        {
            if (es != null 
                && started 
                && cancellationTokenSource != null 
                && !cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Cancel(false);
                cancellationTokenSource.Dispose();
            }
        }
        private string listenUrl;
        public string Url => listenUrl;
    }
}