using System;
using System.Runtime.Serialization;

namespace kcUpdater.Exceptions
{
    class WebRequestFailedException : Exception {
        public WebRequestFailedException() { }
        public WebRequestFailedException(string message) : base(message) { }
        public WebRequestFailedException(string message, Exception innerException) : base(message, innerException) { }
        public WebRequestFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
