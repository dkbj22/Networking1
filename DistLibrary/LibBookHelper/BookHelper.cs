using System;
using System.Text.Json;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using LibData;
using System.Text;

namespace BookHelper
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
    public class SequentialHelper
    {
        public int portNumber;
        public int ServerListeningQueue;
        public IPAddress ipAddress;
        public Setting settings;
        public string configFile = @"../../../../ClientServerConfig.json";
        Socket sock { get; set; }
        IPEndPoint localEndpoint;
        byte[] buffer = new byte[1000];
        string data = null;


        public SequentialHelper()
        {
            string configContent = File.ReadAllText(configFile);
            this.settings = JsonSerializer.Deserialize<Setting>(configContent);
            this.ipAddress = IPAddress.Parse(settings.ServerIPAddress);
            this.portNumber = settings.BookHelperPortNumber;
            this.ServerListeningQueue = settings.ServerListeningQueue;
            //todo: implement the body. Add extra fields and methods to the class if needed

            localEndpoint = new IPEndPoint(this.ipAddress, this.portNumber);


            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                sock.Bind(localEndpoint);
                Console.WriteLine("\nSocket binding");
                sock.Listen(ServerListeningQueue);
                Console.WriteLine("\nWaiting for server... (bookhelper)");
            }
            catch
            {
                Console.WriteLine("Cannot connect with libServer");
            }


        }

        public void sendMsgBookHelper(Message input, Socket sock)
        {
            string strInput = JsonSerializer.Serialize(input);
            Console.WriteLine(strInput);
            byte[] inputInByte = Encoding.ASCII.GetBytes(strInput);
            sock.Send(inputInByte);
        }

        public string[] receiveMsgBookHelper(Socket newSock)
        {
            int b = newSock.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, b);
            string[] typeAndContent = new string[2];
            typeAndContent = data.Split(",");
            return typeAndContent;
        }

        public BookData ReadFromJson()
        {
            BookData jsnBook;
            string configContent = File.ReadAllText(@"../../../Books.json");
            jsnBook = JsonSerializer.Deserialize<BookData>(configContent);
            
            return jsnBook;
        }

        public void start()
        {
            while (true)
            {
                ReadFromJson();
                break;

                Socket newSock = sock.Accept();
                Console.WriteLine("Accetping sockets");

                string[] typeAndContent = receiveMsgBookHelper(newSock);

                if (typeAndContent[0] == "{\"Type\":2")
                {
                    //Check json if book available
                    BookData jsnBook = ReadFromJson();
                    /*foreach(string title in jsnBook.Title)
                    {

                    }*/
                    Message bookInquiryReply = new Message();
                    bookInquiryReply.Type = MessageType.BookInquiryReply;
                    bookInquiryReply.Content = "string";  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    sendMsgBookHelper(bookInquiryReply, newSock);

                    //Content":"Book"}//
                }

                else
                {
                    Console.WriteLine("No correct message recieved!!!");
                }
            }
            sock.Close();
            //todo: implement the body. Add extra fields and methods to the class if needed
        }
    }
}
