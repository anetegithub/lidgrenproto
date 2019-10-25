using Lidgren.Network;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace lidgrenexample.client
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("application name");
            var client = new NetClient(config);
            client.Start();
            client.Connect(host: "127.0.0.1", port: 12345);

            ThroughtThread(client);

            string input = string.Empty;
            while (input!="exit")
            {
                input = Console.ReadLine();
                var message = client.CreateMessage();
                message.Write(input);
                client.SendMessage(message, NetDeliveryMethod.ReliableOrdered);
            }

            Console.WriteLine("Hello World!");
        }

        private static void ThroughtThread(NetClient client)
        {
            new Thread(() =>
            {
                client.MessageReceivedEvent.WaitOne(); // this will block until a message arrives
                GotMessage(client);
            }).Start();
        }

        private static void ThroughCallback(NetClient client)
        {
            SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
            client.RegisterReceivedCallback(new System.Threading.SendOrPostCallback(GotMessage));
        }

        public static void GotMessage(object peer)
        {
            var message = (peer as NetClient).ReadMessage();
            Console.WriteLine(message.ReadString());
        }
    }
}
