﻿using System;
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
        Socket clientSock { get; set; }
        IPEndPoint bookHelperEndPoint { get; set; }

        string data = null;
        byte[] buffer = new byte[1000];
        byte[] msg = Encoding.ASCII.GetBytes("From server: Your message has been delivered\n");


        public SequentialServer()
        {
            string configContent = File.ReadAllText(configFile);
            this.settings = JsonSerializer.Deserialize<Setting>(configContent);

            this.iPAddress = IPAddress.Parse(settings.ServerIPAddress);
            IPAddress BookHelperIPAddress = IPAddress.Parse(settings.BookHelperIPAddress);
            IPAddress UserHelperIPAddress = IPAddress.Parse(settings.UserHelperIPAddress);

            this.portNumber = settings.ServerPortNumber;
            int BookHelperPortNumber = settings.BookHelperPortNumber;
            int UserHelperPortNumber = settings.UserHelperPortNumber;

            this.ServerListeningQueue = settings.ServerListeningQueue;


            IPEndPoint localEndpoint = new IPEndPoint(this.iPAddress, this.portNumber);
            bookHelperEndPoint = new IPEndPoint(BookHelperIPAddress, BookHelperPortNumber);

            clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            


            clientSock.Bind(localEndpoint);
            Console.WriteLine("\nSocket binding");
            clientSock.Listen(ServerListeningQueue);
            Console.WriteLine("\nWaiting for clients... (Listening)");
            
            

            //todo: implement the body. Add extra fields and methods to the class if it is needed
        }

        public void sendMsgServer(Message input, Socket sock)
        {
            string strInput = JsonSerializer.Serialize(input);
            Console.WriteLine(strInput);
            byte[] inputInByte = Encoding.ASCII.GetBytes(strInput);
            sock.Send(inputInByte);
        }

        public string[] receiveMsgServer(Socket newSock)
        {
            int b = newSock.Receive(buffer);
            data = Encoding.ASCII.GetString(buffer, 0, b);
            Console.WriteLine(data);
            string[] typeAndContent = new string[2];
            typeAndContent = data.Split(",");
            Console.WriteLine(typeAndContent[1]);
            return typeAndContent;
        }

        public string correctContent(string badContent)
        {
            string removedEnd = badContent.Remove(badContent.Length - 2);
            string removeBegin = removedEnd.Remove(0,11);
            Console.WriteLine(removeBegin);
            return removeBegin;
        }


        public void start()
        {
            
            while (true)
            {
                Socket newSock = clientSock.Accept();
                Console.WriteLine("Accetping sockets");

                string[] typeAndContent = receiveMsgServer(newSock);

                if (typeAndContent[0] == "{\"Type\":0")
                {
                    Message welcome = new Message();
                    welcome.Type = MessageType.Welcome;
                    welcome.Content = "";
                    sendMsgServer(welcome, newSock);

                    //Content":"Client 0"}//
                }

                typeAndContent = receiveMsgServer(newSock);

                if (typeAndContent[0] == "{\"Type\":2")
                {
                    try
                    {
                        Socket bookHelperSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        bookHelperSock.Connect(bookHelperEndPoint);
                        Message copyBookInquiry = new Message();
                        copyBookInquiry.Type = MessageType.BookInquiry;
                        copyBookInquiry.Content = correctContent(typeAndContent[1]);
                        Console.WriteLine("before sending message");
                        sendMsgServer(copyBookInquiry, bookHelperSock);
                        Console.WriteLine("end of try");
                    }
                    catch
                    {
                        Console.WriteLine("could not connect to Book helper server");
                    }

                    
                    //TODO: The server forwards the book request to the BookHelper

                    //TODO: Ontvang message van bookhelper

                    /*  TypeAndContent[0]       TypeAndContent[1]*/
                    /*{     Type:1 */   /* , */ /* Content:Titel} */
                }
                Console.WriteLine("About to receive MSG4");
                typeAndContent = receiveMsgServer(newSock);
                Console.WriteLine(typeAndContent[0]);
                Console.WriteLine(correctContent(typeAndContent[1]));

                if (typeAndContent[0] == "{\"Type\":4")
                {
                    Console.WriteLine("Book found & recieved");
                    Console.WriteLine(typeAndContent[0]);
                    Console.WriteLine(correctContent(typeAndContent[1]));
                }
                else { Console.WriteLine("Type 4 NOT RECIEVED"); }

                if (typeAndContent[0] == "{\"Type\":8")
                {
                    Console.WriteLine("test succesful but book not found");
                }
                else { Console.WriteLine("Type 4 NOT RECIEVED"); }
            }
            clientSock.Close();
            //todo: implement the body. Add extra fields and methods to the class if it is needed

        }
    }

}



