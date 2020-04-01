using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Communication.Sockets
{
    public class SyncClient
    {
        public void StartClient(string ip, int port, byte[] msg)
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[64];

            // Connect to a remote device.
            try
            {
                // Estabilsh the remote endpoint for the socket.
                IPHostEntry ipHostInfo = Dns.GetHostEntry(ip);
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                // Create a TCP/IP socket.
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());

                    // Send the data through the socket
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                    // Write the response to the console.
                    Console.WriteLine("Echoed test = {0}", bytesRec);
                    for(int i = 0; i < bytesRec; i++)
                    {
                        Console.WriteLine(
                            "byte[{0}]      hex: {1}    int: {2}",
                            (i.ToString().Length > 1) ? i.ToString() : "0" + i.ToString(),
                            bytes[i].ToString("X4"),
                            Convert.ToInt32(bytes[i])
                        );
                    }
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
