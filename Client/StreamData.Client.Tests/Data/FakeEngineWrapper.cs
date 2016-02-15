using System;
using Marvin.JsonPatch;
using Newtonsoft.Json;

namespace StreamData.Client.Tests.Data
{
    class FakeEngineWrapper : ServerSentEventEngine
    {
        public void SendData<T>(T value)
        {
            string data = JsonConvert.SerializeObject(value);
            OnNewJsonData?.Invoke(data);
        } 
        public void SendPatch<T>(JsonPatchDocument<T> doc) where T :class
        {
            string data = JsonConvert.SerializeObject(doc);
            OnNewJsonPatch?.Invoke(data);
        }

        private bool isStarted = false;
        public bool IsStarted => isStarted;

        public event Action<string>  OnNewJsonData;
        public event Action<string> OnNewJsonPatch;

        public static FakeEngineWrapper Instance { get; }= new FakeEngineWrapper();

        private FakeEngineWrapper()
        {
            OnNewJsonData += (_) => { };
            OnNewJsonPatch += (_) => { };
        }

        private string url;
        public string Url => url;
        public bool Start(string url)
        {
            isStarted = true;
            this.url = url;
            return true;
        }

    }
}