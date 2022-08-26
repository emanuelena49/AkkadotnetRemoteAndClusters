using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer
{
    /// <summary>
    /// A diligent actor capable of waiting before doing his stuff
    /// </summary>
    public abstract class DiligentActor : ReceiveActor
    {
        public DiligentActor()
        {
            Receive<RunMessage>(msg => Run());
        }

        public abstract void Run();
    }

    public class RunMessage { }
}
