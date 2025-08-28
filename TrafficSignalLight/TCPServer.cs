using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web;

namespace TrafficSignalLight
{
    public class TCPServer
    {
        static TcpListener server = new TcpListener(IPAddress.Any, 370);
        static List<TcpClient> Clients = new List<TcpClient>();
        static bool STOP_SERVER = false;
        public static bool ServerStarted = false;
        public static string CMD = "";

        public static bool SENDCMD = false;

        public static void Start()
        {
            //ServerStarted = true;

            //server.Start();
            //StartListener();
        }

        public static void Stop()
        {
            STOP_SERVER = true;
            Clients.ForEach(c =>
            {
                try
                {
                    c.GetStream().Close();
                }
                catch { }
            });

            server.Stop();
            ServerStarted = false;

        }

        public static void SendCommand(string cmd)
        {
            Clients.ForEach(c =>
            {
                try
                {
                    var stream = c.GetStream();
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(cmd);
                    stream.Write(reply, 0, reply.Length);
                }
                catch { }
            });
        }

        public static void StartListener()
        {
            try
            {
                while (!STOP_SERVER)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                    t.Start(client);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public static void HandleDeivce(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();
            Clients.Add(client);

            string imei = String.Empty;

            string data = null;
            Byte[] bytes = new Byte[256];
            int i;
            try
            {
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    string hex = BitConverter.ToString(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);

                    string str = "Niletronix Demo @2024";
                    Byte[] reply = System.Text.Encoding.ASCII.GetBytes(str);
                    stream.Write(reply, 0, reply.Length);
                    Console.WriteLine("{1}: Sent: {0}", str, Thread.CurrentThread.ManagedThreadId);
                }


            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }

    }
}