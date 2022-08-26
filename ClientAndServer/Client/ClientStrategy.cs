using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteAndClusters.ClientAndServer.Messages;

namespace RemoteAndClusters.ClientAndServer.Client
{
    public abstract class ClientStrategy
    {
        public ClientActor Actor { get; set; }

        public abstract void Run();
    }

    public class PassiveClientStrategy : ClientStrategy
    {
        private List<IActorRef> activeSubscriptions = new List<IActorRef>();
        private List<IActorRef> pendingSubscriptions;
        private List<IActorRef> failedSubscriptions = new List<IActorRef>();

        public PassiveClientStrategy(IActorRef[] serversToSubscribe)
        {
            pendingSubscriptions = serversToSubscribe.ToList();
        }

        public override void Run()
        {
            PerformSubscriptions();
        }

        private void PerformSubscriptions()
        {
            foreach (IActorRef server in pendingSubscriptions)
            {
                // try to subscribe to any server
                Actor.Subscribe(server).ContinueWith(task =>
                {
                    pendingSubscriptions.Remove(server);
                    
                    // check if subscription has gone well or not
                    if (task.Result is ConfirmSubscribeMessage)
                    {
                        activeSubscriptions.Add(server);
                    } else
                    {
                        failedSubscriptions.Add(server);
                    }
                });
            }
        }
    }

    public class ActiveClientStrategy : PassiveClientStrategy
    {
        private List<int> PausesBeforeEvents { get; set; }
        private List<IActorRef> WherePublishEvents { get; set; }

        public ActiveClientStrategy(IActorRef[] serversToSubscribe, int[] pausesBeforeEvents, IActorRef[] wherePublish) : base(serversToSubscribe)
        {
            PausesBeforeEvents = pausesBeforeEvents.ToList();
            WherePublishEvents = wherePublish.ToList();
        }

        public override void Run()
        {
            base.Run();
            RunEvents();
        }

        private async void RunEvents()
        {
            int eventId = 0;

            foreach (int timeToWait in PausesBeforeEvents)
            {
                // wait some time before publishing the next event
                Thread.Sleep(timeToWait);

                // publish each event on each server
                EventMessage eventMessage = new EventMessage(Actor.GetRef(), eventId);

                foreach (IActorRef server in WherePublishEvents)
                {
                    server.Tell(eventMessage, Actor.GetRef());
                }

                eventId++;
            }
        }
    }
}
