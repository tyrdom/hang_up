using System;
using System.Net;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using GameProtos;


namespace GameServers
{
    public class LinkActor : ReceiveActor
    {
        private readonly IActorRef _sender;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        private string _accountId;

        public LinkActor(EndPoint remote, IActorRef sender)
        {
            _sender = sender;
            Context.Watch(_sender);
            Receive<Tcp.Received>(received =>
            {
                var text = Encoding.UTF8.GetString(received.Data.ToArray()).Trim();
                _log.Info($"Received '{text}' from remote address [{remote}]");
                if (text == "exit")
                    Context.Stop(Self);
                else
                    Sender.Tell(Tcp.Write.Create(received.Data));

                var deSerialize = ProtoTool.DeSerialize<AMsg>(received.Data.ToArray());
                var deSerializeHead = deSerialize.head;

                switch (deSerializeHead)
                {
                    case AMsg.Head.UndefinedMsg:
                        break;
                    case AMsg.Head.UndefinedRequest:
                        break;

                    case AMsg.Head.LoginRequest:
                        var deSerializeLoginRequest = deSerialize.loginRequest;
                        FamousActors.MongodbAccountBaseActor.Tell(deSerializeLoginRequest);
                        break;

                    case AMsg.Head.CreateAccountRequest:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
            Receive<LoginOk>(ok =>
            {
                _accountId = ok.AccountId;
                FamousActors.HallActor.Tell(ok);
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
    }
}