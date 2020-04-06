using System;
using System.Net;
using Akka.Actor;
using Akka.IO;

namespace GameServers
{
    public class GenLinkActor : ReceiveActor
    {
        private IPEndPoint _endPoint;
        private IActorRef _tcpManager;


        protected override void PreStart()
        {
            _tcpManager.Tell(new Tcp.Bind(Self, _endPoint));
        }

        public GenLinkActor(IPEndPoint endPoint, IActorRef tcpManager)
        {
            _endPoint = endPoint;
            _tcpManager = tcpManager;

            Receive<Tcp.Bound>(bind =>
            {
                Console.Out.WriteLine("bind ok on watching::" + bind.LocalAddress); 
                Become(GenLink);
                
            });
        }

        public void GenLink()
        {
            Receive<Tcp.Connected>(connected =>
            {
                Console.Out.WriteLine("A Client Connected::"+connected.RemoteAddress);
                
            });
        }
    }
}