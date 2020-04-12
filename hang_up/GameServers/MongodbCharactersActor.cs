using System.Collections.Generic;
using Akka.Actor;
using Akka.Event;
using GameProtos;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GameServers
{
    public class PlayerCharacters
    {
        [BsonId] public string AccountId { get; set; }
        public Dictionary<uint, CharactersGetResponse.Character.CharacterStatus> CharactersIdToStatus { get; set; }
    }

    


    public class MongodbCharactersActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public MongodbCharactersActor()
        {
            var dbClient = new MongoClient("mongodb://localhost");


            const string dbName = "testDb";
            var mongoDatabase = dbClient.GetDatabase(dbName);
            const string tableName = "player_character";
            var characterTable = mongoDatabase.GetCollection<PlayerCharacters>(tableName);


            Receive<GetCharacters>(characters =>
            {
                var accountIdFilter = Builders<PlayerCharacters>.Filter.Eq(x => x.AccountId, characters.AccountId);
                var firstOrDefault = characterTable.Find(accountIdFilter).FirstOrDefault();
                if (firstOrDefault == null)
                {
                    _log.Info($"accountId{characters.AccountId} have no bank, create one~~~~");
                    var playerCharactersNew
                        = Tools.PlayerCharactersNew(characters.AccountId);
                    characterTable.InsertOne(playerCharactersNew);
                    Sender.Tell(playerCharactersNew);
                }
                else
                {
                    Sender.Tell(firstOrDefault);
                }
            });

            Receive<SaveCharacters>(characters =>  {
                var charactersPlayerCharacters = characters.PlayerCharacters;
                var filter = Builders<PlayerCharacters>.Filter.Eq(x => x.AccountId, charactersPlayerCharacters.AccountId);
                var firstOrDefault = characterTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null) return;
                characterTable.UpdateOne(filter, new ObjectUpdateDefinition<PlayerCharacters>(charactersPlayerCharacters));
            });
            
        }
    }

    public class SaveCharacters
    {
        public PlayerCharacters PlayerCharacters;

        public SaveCharacters(PlayerCharacters playerCharacters)
        {
            PlayerCharacters = playerCharacters;
        }
    }

    public class GetCharacters
    {
        public readonly string AccountId;

        public GetCharacters(string accountId)
        {
            AccountId = accountId;
        }
    }
}