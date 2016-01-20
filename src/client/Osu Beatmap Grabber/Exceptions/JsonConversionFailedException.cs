using System;
using System.Runtime.Serialization;

namespace kcUpdater.Exceptions
{
    class JsonConversionFailedException : Exception
    {
        public JsonConversionFailedException() { }
        public JsonConversionFailedException(string message) : base(message) { }
        public JsonConversionFailedException(string message, Exception innerException) : base(message, innerException) { }
        public JsonConversionFailedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
