using System;

namespace StreamData.Client.Tests.Data
{
    class FakeEngine : ServerSentEventEngine
    {
        public event Action<string> OnNewJsonData;
        public event Action<string> OnNewJsonPatch;
        public bool Start(string apiUrl)
        {
            return FakeEngineWrapper.Instance.Start(apiUrl);
        }

        public string ApiUrl => FakeEngineWrapper.Instance.ApiUrl;
        public FakeEngine()
        {
            FakeEngineWrapper.Instance.OnNewJsonData += s => OnNewJsonData?.Invoke(s);
            FakeEngineWrapper.Instance.OnNewJsonPatch += s => OnNewJsonPatch?.Invoke(s);
        }
    }
}