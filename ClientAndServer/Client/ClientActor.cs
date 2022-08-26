using Akka.Actor;
using RemoteAndClusters.ClientAndServer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Client
{
    public class ClientActor : ReceiveActor
    {
        public static Props PropsFactory(ClientStrategy strategy)
        {
            return Props.Create(() => new ClientActor(strategy));
        }
        
        public ClientStrategy Strategy { get; set; }

        public IActorRef GetRef() => Self;
        
        public ClientActor(ClientStrategy strategy) 
        {
            ReceiveNotificationsBehaviour();

            Strategy = strategy;
            Strategy.Actor = this;
            Strategy.Run();
        }

        protected void ReceiveNotificationsBehaviour()
        {
            Receive<NotifyMessage>(msg => OnNotify(msg, Sender));
        }

        protected virtual void OnNotify(NotifyMessage notify, IActorRef sender)
        {
            Console.WriteLine($"Received notification form {sender} about event {notify.Event}.");
        }
        
        public Task Subscribe(IActorRef server)
        {
            Console.WriteLine($"Subscribing to server {server}");

            return server.Ask(new SubscribeMessage()).ContinueWith(confirm =>
            {
                if (confirm is ConfirmSubscribeMessage)
                {
                    Console.WriteLine($"Subscription to server {server} completed successfully!");
                    return confirm;
                }

                Console.Error.WriteLine($"Error while subscribing to server {server}!");
                return confirm;
            });
        }

        public void Unsubscribe(IActorRef server)
        {
            Console.WriteLine($"Unsubscribing from server {server}");
            server.Ask(new SubscribeMessage()).ContinueWith(confirm =>
            {
                if (confirm is ConfirmUnsubscribeMessage)
                {
                    Console.WriteLine($"Un-subscription from server {server} completed successfully!");
                    return;
                }

                Console.Error.WriteLine($"Error while unsubscribing from server {server}!");
            });
        }
    }
}
