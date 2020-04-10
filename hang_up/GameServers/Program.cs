using System;
using System.Net;
using Akka.Actor;
using Akka.IO;
using GameProtos;

namespace GameServers
{
    class Program
    {
        static void Main(string[] args)
        {
            const int port = 9999;
            var actorSystem = ActorSystem.Create("GameServers");

            actorSystem.ActorOf(Props.Create(() => new GenLinkActor(new IPEndPoint(IPAddress.Loopback, port))),
                "GenLinks");
            var mongodbAccountBaseActor = actorSystem.ActorOf(Props.Create(() => new MongodbAccountActor()));
            var hallActor = actorSystem.ActorOf(Props.Create(() => new HallActor()));
            var mongodbAccountBankActor = actorSystem.ActorOf(Props.Create(() => new MongodbBankActor()));
            FamousActors.MongodbAccountActor = mongodbAccountBaseActor;
            FamousActors.HallActor = hallActor;
            FamousActors.MongodbBankActor = mongodbAccountBankActor;
            Console.WriteLine("Welcome to service!\r\nType 'exit' to exit the service.");
            var input = string.Empty;
            while (string.IsNullOrEmpty(input) || !input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                input = Console.ReadLine();
            }
        }
    }
}