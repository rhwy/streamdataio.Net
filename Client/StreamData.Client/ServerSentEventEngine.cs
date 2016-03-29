using System;

namespace Streamdata.Client
{
    public interface ServerSentEventEngine
    {
        event Action<string> OnNewJsonData;
        event Action<string> OnNewJsonPatch;
        bool Start(string url);
        string Url { get; }
        bool Stop();
    }
}