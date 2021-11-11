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
        public int ServerListeningQueue;
        public string configFile = @"../../../../ClientServerConfig.json";
        Socket sock { get; set; }
        string data = null;
        byte[] buffer = new byte[1000];
        byte[] msg = Encoding.ASCII.GetBytes("From server: Your message has been delivered\n");

        public SequentialServer()
        {
            string configContent = File.ReadAllText(configFile);
            this.settings = JsonSerializer.Deserialize<Setting>(configContent);
            this.iPAddress = IPAddress.Parse(settings.ServerIPAddress);
            this.portNumber = settings.ServerPortNumber;
            this.ServerListeningQueue = settings.ServerListeningQueue;


            IPEndPoint localEndpoint = new IPEndPoint(this.iPAddress, this.portNumber);

            
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sock.Bind(localEndpoint);
            Console.WriteLine("\nSocket binding");
            sock.Listen(ServerListeningQueue);
            Console.WriteLine("\nWaiting for clients... (Listening)");
            
            

            //todo: implement the body. Add extra fields and methods to the class if it is needed
        }

        public void sendMsg(Message input, Socket sock)
        {
            string strInput = JsonSerializer.Serialize(input);
            Console.WriteLine(strInput);
            byte[] inputInByte = Encoding.ASCII.GetBytes(strInput);
            sock.Send(inputInByte);
        }

        public void recieveMsg()
        {

        }

        public void start()
        {
            
            while (true)
            {
                Socket newSock = sock.Accept();
                Console.WriteLine("Accetping sockets");
                int b = newSock.Receive(buffer);
                data = Encoding.ASCII.GetString(buffer, 0, b);


                string[] typeAndContent = new string [2];
                typeAndContent = data.Split(",");

                if (typeAndContent[0] == "{\"Type\":0")
                {
                    Message welcome = new Message();
                    welcome.Type = MessageType.Welcome;
                    welcome.Content = "";
                    sendMsg(welcome, newSock);

                    

                    string strWelcome = JsonSerializer.Serialize(welcome);
                    msg = Encoding.ASCII.GetBytes(strWelcome);
                    newSock.Send(msg);

                    //Content":"Client 0"}//
                }

                else
                {
                    //Console.WriteLine("" + data);
                    data = null;
                    newSock.Send(msg);
                }
            }
            sock.Close();
            //todo: implement the body. Add extra fields and methods to the class if it is needed

        }
    }

}



