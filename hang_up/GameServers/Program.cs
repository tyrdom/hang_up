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
            var port = 9999;
            var actorSystem = ActorSystem.Create("GameServers");

            actorSystem.ActorOf(Props.Create(() => new GenLinkActor(new IPEndPoint(IPAddress.Any, port))), "Genlinks");
            Console.WriteLine("Welcome to Chatter service!\r\nType 'exit' to exit the service.");
            var input = string.Empty;
            while (string.IsNullOrEmpty(input) || !input.Equals("exit", StringComparison.CurrentCultureIgnoreCase))
            {
                input = Console.ReadLine();
            }
        }
    }
}