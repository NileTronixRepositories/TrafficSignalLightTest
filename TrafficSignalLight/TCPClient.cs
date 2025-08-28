using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace TrafficSignalLight
{
    public class TCPClient
    {
        public static string Send(string ipAddress, string msg)
        {
            string ret = string.Empty;
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(ipAddress, 1337/*370*/);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");

                Stream stream = tcpclnt.GetStream();
                Byte[] reply = System.Text.Encoding.ASCII.GetBytes(msg);
                stream.Write(reply, 0, reply.Length);

                Thread.Sleep(1100);
                byte[] bb = new byte[100];
                int k = stream.Read(bb, 0, 100);
                if (k > 0)
                {
                    ret = System.Text.Encoding.ASCII.GetString(bb, 0, k);
                }

                stream.Close();
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                ret = e.Message;
                Console.WriteLine("Error..... " + e.StackTrace);
            }
            return ret;
        }

        public static string Send(string ipAddress, byte[] reply)
        {
            string ret = string.Empty;
            try
            {
                TcpClient tcpclnt = new TcpClient();
                Console.WriteLine("Connecting.....");

                tcpclnt.Connect(ipAddress, 370);
                // use the ipaddress as in the server program

                Console.WriteLine("Connected");

                Stream stream = tcpclnt.GetStream();
                //Byte[] reply = System.Text.Encoding.ASCII.GetBytes(msg);
                stream.Write(reply, 0, reply.Length);

                Thread.Sleep(500);
                byte[] bb = new byte[100];
                int k = stream.Read(bb, 0, 100);
                if (k > 0)
                {
                    ret = System.Text.Encoding.ASCII.GetString(bb, 0, k);
                }

                stream.Close();
                tcpclnt.Close();
            }

            catch (Exception e)
            {
                ret = e.Message;
                Console.WriteLine("Error..... " + e.StackTrace);
            }
            return ret;
        }
    }
}