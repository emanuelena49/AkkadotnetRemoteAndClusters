using Akka.Actor;
using RemoteAndClusters.ClientAndServer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Server
{
    public class ServerActor : ReceiveActor
    {
        public static Props PropsFactory()
        {
            return Props.Create(() => new ServerActor());
        }

        private List<IActorRef> Subscriptions { get; set; }
        
        public ServerActor()
        {
            HandleSubscriptions();
            HandleEvents();
        }

        public void HandleSubscriptions()
        {
            Receive<SubscribeMessage>((msg) =>
            {
                if (!Subscriptions.Contains(Sender))
                    Subscriptions.Add(Sender);

                Sender.Tell(new ConfirmSubscribeMessage());
            });

            Receive<UnsubscribeMessage>((msg) =>
            {
                Subscriptions.Remove(Sender);
                Sender.Tell(new ConfirmUnsubscribeMessage());
            });
        }

        public void HandleEvents()
        {
            Receive<EventMessage>((eventMessage) =>
            {
                NotifyMessage notifyMessage = new NotifyMessage(eventMessage);
                
                foreach(IActorRef subcribed in Subscriptions)
                {
                    subcribed.Tell(notifyMessage);
                }
            });
        }
    }
}
