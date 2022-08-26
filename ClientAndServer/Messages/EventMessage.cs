using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Messages
{
    public class EventMessage
    {
        public IActorRef? EventCreator { get; private set; }
        public int EventId { get; private set; }

        public EventMessage(IActorRef? EventCreator, int EventId)
        {
            this.EventCreator = EventCreator;
        }

        public override string ToString()
        {
            return $"EventMessage:[EventCreator: {EventCreator}, EventId: {EventId}]";
        }
    }
}
