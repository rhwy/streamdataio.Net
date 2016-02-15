namespace StreamData.Client
{
    using System;

    public interface ServerSentEventEngine
    {
        event Action<string> OnNewJsonData;
        event Action<string> OnNewJsonPatch;
        //Action<Action<string>> OnNewJsonData { get; }
        //Action<Action<string>> OnNewJsonPatch { get; }
        bool Start(string url);
        string ApiUrl { get; }
    }

    public class EventSourceServerSentEngine : ServerSentEventEngine
    {
        public event Action<string> OnNewJsonData;
        public event Action<string> OnNewJsonPatch;
        public bool Start(string url)
        {
            throw new NotImplementedException();
        }

        public string ApiUrl { get; }
    }
}