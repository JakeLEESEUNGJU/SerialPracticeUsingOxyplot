using System.IO.Ports;

namespace SerialConnectPractice.Model
{
    public struct SerialConfiguration
    {
        public string PortName;
        public int BaudRate;
        public int DataBits;
        public Parity Parity;
        public Handshake handshake;
        public StopBits stopBits;
    }
}