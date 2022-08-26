using Akka.Actor;
using Akka.Event;
using RemoteAndClusters.ClientAndServer.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteAndClusters.ClientAndServer.Client
{
    public class ClientActor : DiligentActor
    {
        private ILoggingAdapter _log = Logging.GetLogger(Context);

        public static Props PropsFactory(ClientStrategy strategy)
        {
            return Props.Create(() => new ClientActor(strategy));
        }
        
        public ClientStrategy Strategy { get; set; }

        public IActorRef GetRef() => Self;
        
        public ClientActor(ClientStrategy strategy) 
        {
            Strategy = strategy;
            Strategy.Actor = this;
        }

        public override void Run()
        {
            Become(() =>
            {
                ReceiveNotificationsBehaviour();
                Strategy.Run();
            });

            _log.Info("Running...");
        }

        protected void ReceiveNotificationsBehaviour()
        {
            Receive<NotifyMessage>(msg => OnNotify(msg, Sender));
        }

        protected void OnNotify(NotifyMessage notify, IActorRef sender)
        {
            _log.Info($"Received notification form {sender} about event {notify.Event}.");
        }
        
        public Task<Object> Subscribe(IActorRef server)
        {
            _log.Info($"Subscribing to server {server}");
            return server.Ask(new SubscribeMessage(Self)).ContinueWith(confirm =>
            {
                if (!(confirm.Result is ConfirmSubscribeMessage))
                {
                    _log.Error($"Error while subscribing to server {server}. " +
                        $"Expected {typeof(ConfirmSubscribeMessage)} but received " +
                        $"{confirm.Result.GetType()}");
                    return confirm.Result;
                } 

                _log.Info($"Subscription to server {server} completed successfully!");
                return confirm.Result;
            });
        }

        public Task<Object> Unsubscribe(IActorRef server)
        {
            _log.Info($"Unsubscribing from server {server}");
            return server.Ask(new UnsubscribeMessage(Self)).ContinueWith(confirm =>
            {
                if (!(confirm.Result is ConfirmUnsubscribeMessage))
                {
                    _log.Error($"Error while unsubscribing to server {server}. " +
                        $"Expected {typeof(ConfirmUnsubscribeMessage)} but received " +
                        $"{confirm.GetType()}");
                    return confirm.Result;
                }

                _log.Info($"Un-subscription from server {server} completed successfully!");
                return confirm.Result;
            });
        }
    }
}
