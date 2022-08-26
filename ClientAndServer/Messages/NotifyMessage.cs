using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Messages
{
    public class NotifyMessage
    {
        public EventMessage Event { get; private set; }

        public NotifyMessage(EventMessage eventMessage) 
        {
            Event = eventMessage;
        }
    }
}
