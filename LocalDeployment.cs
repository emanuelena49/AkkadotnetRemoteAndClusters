using Akka.Actor;
using RemoteAndClusters.ClientAndServer.Server;
using RemoteAndClusters.ClientAndServer.Client;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteAndClusters.ClientAndServer;

namespace RemoteAndClusters
{
    /// <summary>
    /// The classical local deployment of the subscription system
    /// </summary>
    public class LocalDeployment
    {
        public static void Main()
        {
            var actorSystem = ActorSystem.Create("EventsActorSystem");

            IActorRef server = actorSystem.ActorOf(ServerActor.PropsFactory(), "EventsServer");

            ClientStrategy passiveStrategy = new PassiveClientStrategy(new IActorRef[] { server });
            ClientStrategy activeStrategy = new ActiveClientStrategy(
                new IActorRef[] { server },
                new int[] { 3000, 500, 1000 },
                new IActorRef[] { server }
            );

            IActorRef passiveClient = actorSystem.ActorOf(ClientActor.PropsFactory(passiveStrategy), "PassiveClient");
            IActorRef activeClient = actorSystem.ActorOf(ClientActor.PropsFactory(activeStrategy), "ActiveClient");

            RunMessage runMessage = new RunMessage();

            server.Tell(runMessage);

            Thread.Sleep(1000);

            passiveClient.Tell(runMessage);
            activeClient.Tell(runMessage);

            Console.ReadKey();
            actorSystem.Terminate();
        }
    }
}
