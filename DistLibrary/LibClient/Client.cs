using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;
using LibData;


namespace LibClient
{
    // Note: Do not change this class 
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

    // Note: Do not change this class 
    public class Output
    {
        public string Client_id { get; set; } // the id of the client that requests the book
        public string BookName { get; set; } // the name of the book to be reqyested
        public string Status { get; set; } // final status received from the server
        public string BorrowerName { get; set; } // the name of the borrower in case the status is borrowed, otherwise null
        public string BorrowerEmail { get; set; } // the email of the borrower in case the status is borrowed, otherwise null
    }

    // Note: Complete the implementation of this class. You can adjust the structure of this class.
    public class SimpleClient
    {
        // some of the fields are defined. 
        public Output result;
        public Socket clientSocket;
        public IPEndPoint serverEndPoint;
        public IPAddress ipAddress;
        public Setting settings;
        public string client_id;
        private string bookName;
        // all the required settings are provided in this file
        //public string configFile = @"../ClientServerConfig.json";
        public string configFile = @"../../../../ClientServerConfig.json"; // for debugging

        // todo: add extra fields here in case needed 
        public int ServerPortNumber;
        byte[] buffer = new byte[1000];

        // public IPAddress ServerIPAddress;
        /// <summary>
        /// Initializes the client based on the given parameters and seeting file.
        /// </summary>
        /// <param name="id">id of the clients provided by the simulator</param>
        /// <param name="bookName">name of the book to be requested from the server, provided by the simulator</param>
        public SimpleClient(int id, string bookName)
        {
            //todo: extend the body if needed.
            this.bookName = bookName;
            this.client_id = "Client " + id.ToString();
            this.result = new Output();
            result.BookName = bookName;
            result.Client_id = this.client_id;
            // read JSON directly from a file
            try
            {
                string configContent = File.ReadAllText(configFile);
                this.settings = JsonSerializer.Deserialize<Setting>(configContent);
                this.ipAddress = IPAddress.Parse(settings.ServerIPAddress);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("[Client Exception] {0}", e.Message);
            }
        }

        /// <summary>
        /// Establishes the connection with the server and requests the book according to the specified protocol.
        /// Note: The signature of this method must not change.
        /// </summary>
        /// <returns>The result of the request</returns>
        /// 

        public void sendMsgClient(Message input, IPEndPoint sender, Socket sock)
        {
            string inputString = JsonSerializer.Serialize(input);
            byte[] msg = Encoding.ASCII.GetBytes(inputString);
            sock.SendTo(msg, msg.Length, SocketFlags.None, sender);
            Console.WriteLine("Sending message to server");
        }

        public string[] receiveMsgClient(Socket sock)
        {
            int receiveBytes = sock.Receive(buffer);
            string data = Encoding.ASCII.GetString(buffer, 0, receiveBytes);
            string[] typeAndContent = new string[2];
            typeAndContent = data.Split(",");
            Console.WriteLine(typeAndContent[0] + typeAndContent[1] + " - Received from server with receiveMsgClient");
            return typeAndContent;
        }
       

        public Output start()
        {
            Console.WriteLine("start()");
            string configContent = File.ReadAllText(configFile);
            this.settings = JsonSerializer.Deserialize<Setting>(configContent);
            this.ipAddress = IPAddress.Parse(settings.ServerIPAddress);
            this.ServerPortNumber = settings.ServerPortNumber;

            
            byte[] msg = null;

            Message hello = new Message();
            hello.Content = this.client_id;
            hello.Type = MessageType.Hello;
                       
            IPEndPoint sender = new IPEndPoint(ipAddress, ServerPortNumber); 
            EndPoint remoteEP = (EndPoint)sender;
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {   
                sock.Connect(sender);
                Console.WriteLine("Trying to connect");
            }
            catch
            {
                Console.WriteLine("Connection error");
            }

            try
            {
                //Sending and receiving msgs
                sendMsgClient(hello, sender, sock);
                receiveMsgClient(sock);
            }
            catch 
            { 
                Console.WriteLine("Message error"); 
            }


            // todo: implement the body to communicate with the server and requests the book. Return the result as an Output object.
            // Adding extra methods to the class is permitted. The signature of this method must not change.

            return result;
        }

    }
}
