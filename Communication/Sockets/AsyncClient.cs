using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Communication.Sockets
{
    public class AsyncClient
    {
        // ManualResetEvent instances signal completion.  
        private readonly ManualResetEvent connectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent sendDone = new ManualResetEvent(false);
        private readonly ManualResetEvent receiveDone = new ManualResetEvent(false);

        
        private byte[] response = new byte[0];

        public async Task<byte[]> StartClient(string ip, int port, byte[] msg)
        {
            // Connect to a remote device.  
            
            // Establish the remote endpoint for the socket.
            IPAddress ipAddress = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            // Create a TCP/IP socket.  
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // Connect to the remote endpoint.  
            client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);

            if (!connectDone.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connection timed out");
                Console.ResetColor();
            }

            // Send data to the remote device.
            Send(client, msg);
            if (!sendDone.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Sending timed out");
                Console.ResetColor();
            }

            // Receive the response from the remote device.  
            Receive(client);

            if (!receiveDone.WaitOne(TimeSpan.FromSeconds(10)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Receiving timed out");
                Console.ResetColor();
            }

            // Release the socket.
            client.Shutdown(SocketShutdown.Both);
            client.Close();

            await Task.Yield();
            return response;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection.  
            client.EndConnect(ar);

            Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

            // Signal that the connection has been made.  
            connectDone.Set();
        }

        private void Receive(Socket client)
        {
            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = client;

            // Begin receiving the data from the remote device.  
            client.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the client socket from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            // Read data from the remote device.  
            int bytesRead = client.EndReceive(ar);

            // Write the response to the console.
            Console.WriteLine("Echoed test = {0}", bytesRead);

            // Resize the response array based on the length of the returned response.
            Array.Resize(ref response, bytesRead);

            // Copy the returned response into the response array
            Array.Copy(state.buffer, 0, response, 0, bytesRead);

            receiveDone.Set();
        }

        private void Send(Socket client, byte[] byteData)
        {
            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.  
            Socket client = (Socket)ar.AsyncState;

            // Complete sending the data to the remote device.  
            int bytesSent = client.EndSend(ar);
            Console.WriteLine("Sent {0} bytes to server.", bytesSent);

            // Signal that all bytes have been sent.  
            sendDone.Set();
        }
    }
}
