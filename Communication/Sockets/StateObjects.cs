using System.Net.Sockets;

namespace Communication.Sockets
{
    public class StateObject
    {
        // Client socket
        public Socket workSocket = null;

        // Size of receive buffer.
        public const int bufferSize = 256;

        // Receive buffer.
        public byte[] buffer = new byte[bufferSize];
    }
}