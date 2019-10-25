using Lidgren.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace lidgrenexample
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("application name")
            { Port = 12345 };
            var server = new NetServer(config);
            server.Start();

            ThroughtThread(server);
            
            string input = string.Empty;
            while (input != "exit")
            {
                input = Console.ReadLine();
                var message = server.CreateMessage();
                message.Write(input);
                server.SendMessage(message, server.Connections, NetDeliveryMethod.ReliableOrdered,0);
            }
        }


        private static void ThroughtThread(NetServer client)
        {
            new Thread(() =>
            {
                client.MessageReceivedEvent.WaitOne(); // this will block until a message arrives
                GotMessage(client);
            }).Start();
        }

        private static void ThroughCallback(NetServer client)
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(GotMessage));
        }

        public static void GotMessage(object peer)
        {
            var message = (peer as NetServer).ReadMessage();
            Console.WriteLine(message.ReadString());
        }
    }
}
