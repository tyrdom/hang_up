using Akka.Actor;
using Akka.IO;

namespace GameServers
{
    public class LinkActor : ReceiveActor
    {
        private IActorRef _sender;

        public LinkActor(IActorRef sender)
        {
            _sender = sender;
            Receive<Tcp.Received>(received => { });
        }
    }
}