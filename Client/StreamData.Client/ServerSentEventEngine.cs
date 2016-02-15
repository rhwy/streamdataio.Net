namespace StreamData.Client
{
    using System;

    public interface ServerSentEventEngine
    {
        Action<Action<string>> OnNewJsonData { get; }
        Action<Action<string>> OnNewJsonPatch { get; }
        bool Start();
    }
}