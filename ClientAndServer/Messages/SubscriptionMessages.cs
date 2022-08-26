using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Messages
{
    public class SubscribeMessage
    {
        public IActorRef WhoSubscribe { get; private set; }

        public SubscribeMessage(IActorRef actorRef)
        {
            WhoSubscribe = actorRef;
        }

    }

    public class UnsubscribeMessage
    {
        public IActorRef WhoUnsubscribe { get; private set; }

        public UnsubscribeMessage(IActorRef actorRef)
        {
            WhoUnsubscribe = actorRef;
        }
    }

    public class ConfirmMessage
    {

    }

    public class ConfirmSubscribeMessage : ConfirmMessage
    {

    }

    public class ConfirmUnsubscribeMessage : ConfirmMessage
    {

    }
}
