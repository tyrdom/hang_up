using System;
using System.Net;
using Akka.Actor;
using Akka.IO;

namespace GameServers
{
    class Program
    {
        static void Main(string[] args)
        {
            var actorSystem = ActorSystem.Create("GameServers");
            var tcp = actorSystem.Tcp();
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 8123);
            
            
        }
    }
}