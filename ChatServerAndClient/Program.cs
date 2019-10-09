using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChatServerAndClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ExecuteServer();
        }

        private static void ExecuteServer()
        {
            IPAddress iPAddress;
            iPAddress = Dns.GetHostEntry("localhost").AddressList[1];
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 12345);

            Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(iPEndPoint);
                listener.Listen(10);
                while (true)
                {
                    Console.WriteLine("Waiting connection...");

                    Socket clientSocket = listener.Accept();

                    byte[] clientBytes = new byte[1024];
                    string data = null;

                    Console.WriteLine("Client connected");

                    while (true)
                    {
                        data = null;
                        int numByte = clientSocket.Receive(clientBytes);

                        data += Encoding.ASCII.GetString(clientBytes, 0, numByte);

                        if(data.IndexOf("<EOF>") > -1) { break; }

                        Console.WriteLine("Message {0}", data);
                        byte[] message = Encoding.ASCII.GetBytes("");

                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static ThreadStart HandleClient()
        {
            throw new NotImplementedException();
        }
    }
}
