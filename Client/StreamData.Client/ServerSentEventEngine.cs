using System.Collections.Generic;
using System.Threading;
using EventSource4Net;
using Marvin.JsonPatch;
using Newtonsoft.Json;

namespace StreamData.Client
{
    using System;

    public interface ServerSentEventEngine
    {
        event Action<string> OnNewJsonData;
        event Action<string> OnNewJsonPatch;
        bool Start(string url);
        string Url { get; }
    }

    public class EventSourceServerSentEngine : ServerSentEventEngine
    {
        public event Action<string> OnNewJsonData = (_)=> {};
        public event Action<string> OnNewJsonPatch = (_) => {};
        private CancellationTokenSource cancellationTokenSource;
        private EventSource es;
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
            return true;
        }

        private string listenUrl;
        public string Url => listenUrl;
    }
}