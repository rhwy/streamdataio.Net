using System;

namespace Streamdata.Client
{
    public class StreamdataConfigurationException : Exception
    {
        public StreamdataConfigurationException(string message = null) : base(message)
        {

        }
    }
}