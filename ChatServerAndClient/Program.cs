using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime;

namespace ChatServerAndClient {

    class Program {
        public static ManualResetEvent done = new ManualResetEvent(false);

        static void Main(string[] args) {
            ExecuteServer();
        }

        private static void ExecuteServer() {
            IPAddress iPAddress;
            iPAddress = Dns.GetHostEntry("localhost").AddressList[1];
            IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, 12345);

            Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try {
                listener.Bind(iPEndPoint);
                listener.Listen(100);
                while (true) {
                    done.Reset();
                    Console.WriteLine("Waiting connection...");

                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    done.WaitOne();
                    //Socket clientSocket = listener.Accept();

                    //byte[] clientBytes = new byte[1024];
                    //string data = null;

                    //Console.WriteLine("Client connected");

                    //while (true)
                    //{
                    //    data = null;
                    //    int numByte = clientSocket.Receive(clientBytes);

                    //    data += Encoding.ASCII.GetString(clientBytes, 0, numByte);

                    //    if(data.IndexOf("<EOF>") > -1) { break; }

                    //    Console.WriteLine("Message {0}", data);
                    //    byte[] message = Encoding.ASCII.GetBytes("");

                    //}
                }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }

        private static void AcceptCallback(IAsyncResult ar) {
            done.Set();


            Socket listener = (Socket)ar.AsyncState;

            Socket handler = listener.EndAccept(ar);

            StateObject state = new StateObject();
            state.workSocket = handler;
            Console.WriteLine("User connected");
            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }

        private static void ReadCallback(IAsyncResult ar) {
            String content = null;
            StateObject state = (StateObject)ar.AsyncState;

            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0) {
                state.stringBuilder.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));

                content = state.stringBuilder.ToString();
                if (content.IndexOf("<EOF>") > -1) {
                    Console.WriteLine("Read {0} bytes from Socket \n Data : {1}", content.Length, content);
                    Send(handler, content);
                } else {
                    handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    Console.WriteLine(content.ToString());
                }
            }
        }

        private static void Send(Socket handler, string content) {
            byte[] byteData = Encoding.ASCII.GetBytes(content);

            handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {

            Socket handler = (Socket) ar.AsyncState;

            int bytesSent = handler.EndSend(ar);
            Console.WriteLine("Sent {0} bytes", bytesSent);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
            } catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        private static ThreadStart HandleClient() {
            throw new NotImplementedException();
        }
    }
    class StateObject {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder stringBuilder = new StringBuilder();
    }
}
