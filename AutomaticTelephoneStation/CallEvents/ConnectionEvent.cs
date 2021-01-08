using AutomaticTelephoneStation.ATSInfo;

namespace AutomaticTelephoneStation.CallEvents
{
    public class ConnectionEvent
    {
        public IPort Port { get; set; }

        public ConnectionEvent(IPort port)
        {
            Port = port;
        }
    }
}
