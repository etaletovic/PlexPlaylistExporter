using System.Runtime.Serialization;

namespace PlexPlaylistExporter.SecretsManager.Abstractions.Contracts
{
    public class SecretsManagerException : Exception
    {
        public SecretsManagerException()
        {
        }

        public SecretsManagerException(string? message) : base(message)
        {
        }

        public SecretsManagerException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SecretsManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
