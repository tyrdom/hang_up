using System.Collections.Generic;
using GameProtos;
using MongoDB.Bson.Serialization.Attributes;

namespace GameServers
{
    public class PlayerCharacters
    {
        [BsonId] public string AccountId { get; set; }
        public Dictionary<uint, CharacterStatus> CharactersIdToStatus { get; set; }
    }

    public class SaveCharacters
    {
        public readonly PlayerCharacters PlayerCharacters;

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