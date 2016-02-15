using System.Collections.Generic;
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
        bool Stop();
    }
}