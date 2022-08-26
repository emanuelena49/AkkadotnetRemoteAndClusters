using Akka.Actor;
using Akka.Event;
using RemoteAndClusters.ClientAndServer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Server
{
    public class ServerActor : DiligentActor
    {
        private ILoggingAdapter _log = Logging.GetLogger(Context);

        public static Props PropsFactory()
        {
            return Props.Create(() => new ServerActor());
        }

        private List<IActorRef> Subscriptions { get; set; } = new List<IActorRef> { };
        
        public ServerActor() { }

        public override void Run()
        {
            Become(() =>
            {
                HandleSubscriptions();
                HandleEvents();
            });

            _log.Info("Running...");
        }

        public void HandleSubscriptions()
        {
            Receive<SubscribeMessage>((msg) =>
            {
                _log.Info($"Subscribing {msg.WhoSubscribe} (as asked by {Sender})");

                if (!Subscriptions.Contains(msg.WhoSubscribe))
                    Subscriptions.Add(msg.WhoSubscribe);

                Sender.Tell(new ConfirmSubscribeMessage(), Self);
            });

            Receive<UnsubscribeMessage>((msg) =>
            {
                _log.Info($"Un-subscribing {msg.WhoUnsubscribe} (as asked by {Sender})");

                Subscriptions.Remove(msg.WhoUnsubscribe);
                Sender.Tell(new ConfirmUnsubscribeMessage(), Self);
            });
        }

        public void HandleEvents()
        {
            Receive<EventMessage>((eventMessage) =>
            {
                _log.Info($"{Sender} told me to notify everyone about {eventMessage}");

                NotifyMessage notifyMessage = new NotifyMessage(eventMessage);
                
                foreach(IActorRef subcribed in Subscriptions)
                {
                    subcribed.Tell(notifyMessage, Self);
                }
            });
        }
    }
}
