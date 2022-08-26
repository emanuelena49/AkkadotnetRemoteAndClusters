using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters
{
    /// <summary>
    /// The classical local deployment of the subscription system
    /// </summary>
    internal class LocalDeployment
    {
        public static void Main()
        {
            var actorSystem = ActorSystem.Create("EchoActorSystem");
        }
    }
}
