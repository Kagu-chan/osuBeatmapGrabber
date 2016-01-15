using System;
using System.Runtime.Serialization;

namespace kcUpdater.Exceptions
{
    class ConfigurationNotHandableException : Exception
    {
        public Enums.ConfigurationIssueType IssueType = Enums.ConfigurationIssueType.Unknown;

        public ConfigurationNotHandableException() { }
        public ConfigurationNotHandableException(string message) : base(message) { }
        public ConfigurationNotHandableException(string message, Exception innerException) : base(message, innerException) { }
        public ConfigurationNotHandableException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public ConfigurationNotHandableException(Enums.ConfigurationIssueType issueType)
        {
            IssueType = issueType;
        }
    }
}
