namespace StreamData.Client
{
    using System;

    public interface ServerSentEventEngine
    {
        Action<Action<string>> OnNewJsonData { get; }
        Action<Action<string>> OnNewJsonPatch { get; }
        //void OnNewJsonData(Action<string> jsonDataAction);
        //void OnNewJsonPatch(Action<string> jsonPatchDataAction);
        bool Start();
    }
}