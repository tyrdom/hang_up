using System;
using System.Net;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using log4net;

namespace GameServers
{
    public class GenLinkActor : ReceiveActor
    {
        private IActorRef _manager = Context.System.Tcp();

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public GenLinkActor(IPEndPoint endPoint)
        {
            _manager.Tell(new Tcp.Bind(Self, endPoint));


            Receive<Tcp.Bound>(bind =>
            {
                _log.Info("bind ok on watching::" + bind.LocalAddress);
                Become(GenLink);
            });
        }

        private void GenLink()
        {
            Receive<Tcp.Connected>(connected =>
            {
                var connectedRemoteAddress = connected.RemoteAddress;
                _log.Info("A Client Connected::" + connectedRemoteAddress);
                var actorRef = Context.ActorOf(Props.Create(() => new LinkActor(connectedRemoteAddress, Sender)));
                Sender.Tell(new Tcp.Register(actorRef));
            });
        }
    }
}