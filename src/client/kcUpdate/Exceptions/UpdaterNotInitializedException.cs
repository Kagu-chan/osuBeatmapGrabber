using System;
using System.Runtime.Serialization;

namespace kcUpdater.Exceptions
{
    class UpdaterNotInitializedException : Exception
    {
        public UpdaterNotInitializedException() { }
        public UpdaterNotInitializedException(string message) : base(message) { }
        public UpdaterNotInitializedException(string message, Exception innerException) : base(message, innerException) { }
        public UpdaterNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}