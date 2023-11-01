using System;
using System.Collections;
using System.Runtime.Serialization;

namespace JPP.Common.Windows.Localization
{
    public class LclFileException : Exception
    {
        public LclFileException()
        {
        }

        public LclFileException(string message) : base(message)
        {
        }

        public LclFileException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LclFileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
