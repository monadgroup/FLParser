using System;

namespace Monad.FLParser
{
    public class FlParseException : Exception
    {
        public long StreamPosition { get; }

        public FlParseException(string message, long streamPosition) : base(message)
        {
            StreamPosition = streamPosition;
        }

        public FlParseException(string message, long streamPosition, Exception innerException) : base(message, innerException)
        {
            StreamPosition = streamPosition;
        }
    }
}
