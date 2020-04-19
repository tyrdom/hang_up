using Akka.Actor;

namespace GameServers
{
    public static class FamousActors
    {
        public static IActorRef? MongodbAccountActor { get; set; }

        public static IActorRef? HallActor { get; set; }

        public static IActorRef? MongodbBankActor { get; set; }
        
        public static IActorRef? MongodbCharacterActor { get; set; }
        
    }
}