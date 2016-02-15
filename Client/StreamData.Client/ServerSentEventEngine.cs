namespace StreamData.Client
{
    using System;

    public interface ServerSentEventEngine
    {
        event Action<string> OnNewJsonData;
        event Action<string> OnNewJsonPatch;
        //Action<Action<string>> OnNewJsonData { get; }
        //Action<Action<string>> OnNewJsonPatch { get; }
        bool Start();
    }
}