using Shared;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        private static TcpClient client;
        private static NetworkStream stream;
        static void Main(string[] args)
        {
            client = new System.Net.Sockets.TcpClient("127.0.0.1", 1234); // Create a new connection
            stream = client.GetStream();

            Console.WriteLine(@"
            ================================
            =   Enter details and submit   =
            ================================");

            

            // Send the message
            /*byte[] bytes = sendMessage(System.Text.Encoding.Unicode.GetBytes(personToJSON));
            string answer = MyUtil.cleanMessage(bytes);
            Console.WriteLine(answer);*/

            Console.ReadLine();

            sendMessage(Encoding.Unicode.GetBytes("Hello there!"));

            readMessage();

            Console.Read();
        }

        private static void readMessage()
        {
            byte[] message = new byte[1024];
            stream.Read(message, 0, message.Length);
            Console.WriteLine(Encoding.Unicode.GetString(message));
        }

        private static void sendMessage(byte[] messageBytes)
        {
            try
            {
                stream.Write(messageBytes, 0, messageBytes.Length); // Write the bytes
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
