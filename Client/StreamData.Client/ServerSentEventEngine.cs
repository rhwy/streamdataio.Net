namespace StreamData.Client
{
    using System;

    public interface ServerSentEventEngine
    {
        void OnNewJsonData(Action<string> jsonDataAction);
        void OnNewJsonPatch(Action<string> jsonPatchDataAction);
    }
}