using Akka.Actor;

namespace GameServers
{
    public static class FamousActors
    {
        public static IActorRef MongodbAccountBaseActor { get; set; }

        public static IActorRef HallActor { get; set; }
    }
}