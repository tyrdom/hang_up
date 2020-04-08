using System;
using System.Net;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Akka.IO;


namespace GameServers
{
    public class LinkActor : ReceiveActor
    {
        private readonly IActorRef _sender;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public LinkActor(EndPoint remote, IActorRef sender)
        {
            _sender = sender;

            Receive<Tcp.Received>(received =>
            {
                var text = Encoding.UTF8.GetString(received.Data.ToArray()).Trim();
                _log.Info($"Received '{text}' from remote address [{remote}]");
                if (text == "exit")
                    Context.Stop(Self);
                else
                    Sender.Tell(Tcp.Write.Create(received.Data));
            });
            Receive<Tcp.ConnectionClosed>(closed =>
            {
                _log.Info($"Stopped, remote connection [{remote}] closed");
                Context.Stop(Self);
            });
            Receive<Terminated>(terminated =>
            {
                _log.Info($"Stopped, remote connection [{remote}] died");
                Context.Stop(Self);
            });
        }

        protected override void PreStart()
        {
            Context.Watch(_sender);
        }
    }
}