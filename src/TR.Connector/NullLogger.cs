using TR.Connectors.Api.Interfaces;

namespace TR.Connector
{
    internal sealed class NullLogger : ILogger
    {
        public void Debug(string message) { }
        public void Warn(string message) { }
        public void Error(string message) { }
    }
}
