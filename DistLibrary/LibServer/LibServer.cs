using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using LibData;
using System.Text;

namespace LibServer
{
    // Note: Do not change this class.
    public class Setting
    {
        public int ServerPortNumber { get; set; }
        public int BookHelperPortNumber { get; set; }
        public int UserHelperPortNumber { get; set; }
        public string ServerIPAddress { get; set; }
        public string BookHelperIPAddress { get; set; }
        public string UserHelperIPAddress { get; set; }
        public int ServerListeningQueue { get; set; }
    }


    // Note: Complete the implementation of this class. You can adjust the structure of this class. 
    public class SequentialServer
    {
        public Setting settings;
        public IPAddress iPAddress;
        public int portNumber;
        public string configFile = @"../../../ClientServerConfig.json";

        public SequentialServer()
        {
            //todo: implement the body. Add extra fields and methods to the class if it is needed
        }

        public void start()
        {
            string data = null;
            byte[] buffer = new byte[1000];
            byte[] msg = Encoding.ASCII.GetBytes("From server: Your message has been delivered\n");

            //
            string configContent = File.ReadAllText(configFile);
            this.settings = JsonSerializer.Deserialize<Setting>(configContent);
            this.iPAddress = IPAddress.Parse(settings.ServerIPAddress);
            this.portNumber = settings.ServerPortNumber;

            //xta
            //IPAddress ServerIP = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndpoint = new IPEndPoint(this.iPAddress, this.portNumber);

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sock.Bind(localEndpoint);
            sock.Listen(5);
            Console.WriteLine("\nWaiting for clients...");
            Socket newSock = sock.Accept();

            while (true)
            {
                int b = newSock.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, b);

                if (data == "Closed")
                {
                    newSock.Close();
                    Console.WriteLine("Closing the socket..");
                    break;
                }
                else
                {
                    Console.WriteLine("" + data);
                    data = null;
                    newSock.Send(msg);
                }
            }
            sock.Close();
            //todo: implement the body. Add extra fields and methods to the class if it is needed

        }
    }

}



