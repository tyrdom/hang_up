#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Util;
using Akka.Util.Internal;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GameServers
{
    public class PlayerStatus
    {
        public PlayerBank PlayerBank;
        public PlayerGames PlayerGames;
        public PlayerCharacters PlayerCharacters;

        public PlayerStatus(PlayerBank playerBank, PlayerGames playerGames, PlayerCharacters playerCharacters)
        {
            PlayerBank = playerBank;
            PlayerGames = playerGames;
            PlayerCharacters = playerCharacters;
        }
    }

    public class PlayerBank
    {
        

        [BsonId] public string AccountId { get; set; }
        public Dictionary<uint, ulong> MoneysIdToNum { get; set; }
        public Dictionary<uint, uint> ItemsIdToNum { get; set; }
    }


    public class MongodbPlayerStatusActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        private readonly IMongoCollection<PlayerCharacters> _characterTable;
        private readonly IMongoCollection<PlayerBank> _bankTable;
        private readonly IMongoCollection<PlayerGames> _gamesTable;

        private PlayerGames GetAccountGames(string accountId)
        {
            var accountIdFilter = Builders<PlayerGames>.Filter.Eq(x => x.AccountId, accountId);
            var firstOrDefault = _gamesTable.Find(accountIdFilter).FirstOrDefault();
            if (firstOrDefault != null) return firstOrDefault;
            _log.Info($"accountId{accountId} have no games, create one~~~~");
            var playerNewGames
                = Tools.PlayerNewGames(accountId);
            _gamesTable.InsertOne(playerNewGames);
            return playerNewGames;
        }

        private PlayerCharacters GetAccountCharacters(string accountId)
        {
            var accountIdFilter = Builders<PlayerCharacters>.Filter.Eq(x => x.AccountId, accountId);
            var firstOrDefault = _characterTable.Find(accountIdFilter).FirstOrDefault();
            if (firstOrDefault == null)
            {
                _log.Info($"accountId{accountId} have no characters, create one~~~~");
                var playerNewCharacters
                    = Tools.PlayerNewCharacters(accountId);
                _characterTable.InsertOne(playerNewCharacters);
                return playerNewCharacters;
            }

            return firstOrDefault;
        }

        private PlayerBank GetAccountBank(string accountId)
        {
            var accountIdFilter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, accountId);
            var firstOrDefault = _bankTable.Find(accountIdFilter).FirstOrDefault();
            if (firstOrDefault != null) return firstOrDefault;
            _log.Error($"accountId{accountId} have no bank, create one~~~~");
            var playerNewBank = Tools.PlayerNewBank(accountId);
            _bankTable.InsertOne(playerNewBank);
            return playerNewBank;
        }

        public MongodbPlayerStatusActor()
        {
            var mongoClient = FamousActors.DbClient;


            const string dbName = "testDb";
            var mongoDatabase = mongoClient.GetDatabase(dbName);
            const string tableName = "player_bank";
            _bankTable = mongoDatabase.GetCollection<PlayerBank>(tableName);
            const string characterTableName = "player_character";
            _characterTable = mongoDatabase.GetCollection<PlayerCharacters>(characterTableName);
            const string gamesTableName = "player_games";
            _gamesTable = mongoDatabase.GetCollection<PlayerGames>(characterTableName);


            Receive<InitStatus>(status =>
            {
                var statusAccountId = status.AccountId;
                var accountCharacters = GetAccountCharacters(statusAccountId);
                var accountGames = GetAccountGames(statusAccountId);
                var accountBank = GetAccountBank(statusAccountId);
                var playerStatus = new PlayerStatus(accountBank,accountGames,accountCharacters);
                Sender.Tell(playerStatus);
            });

            Receive<CreateBank>(cBank =>
            {
                var filter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, cBank.AccountId);
                var firstOrDefault = _bankTable.Find(filter).FirstOrDefault();
                var playerBank = Tools.PlayerNewBank(cBank.AccountId);
                if (firstOrDefault == null)
                {
                    _bankTable.InsertOne(playerBank);
                }
            });


            Receive<GetBank>(bank =>
            {
                var accountBank = GetAccountBank(bank.AccountId);
                Sender.Tell(accountBank);
            });

            Receive<SaveBank>(saveBank =>
            {
                var bankPlayerBank = saveBank.PlayerBank;
                var filter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, bankPlayerBank.AccountId);
                var firstOrDefault = _bankTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null) return;
                _bankTable.UpdateOne(filter, new ObjectUpdateDefinition<PlayerBank>(bankPlayerBank));
            });

            Receive<GetCharacters>(characters =>
            {
                var accountCharacters = GetAccountCharacters(characters.AccountId);
                Sender.Tell(accountCharacters);
            });

            Receive<SaveCharacters>(characters =>
            {
                var charactersPlayerCharacters = characters.PlayerCharacters;
                var filter =
                    Builders<PlayerCharacters>.Filter.Eq(x => x.AccountId, charactersPlayerCharacters.AccountId);
                var firstOrDefault = _characterTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null) return;
                _characterTable.UpdateOne(filter,
                    new ObjectUpdateDefinition<PlayerCharacters>(charactersPlayerCharacters));
            });
        }
    }

    public class PlayerGames
    {
        [BsonId] public string AccountId { get; set; }

        public int MainLevel { get; set; }
        public int TowerLevel { get; set; }
        
        public DateTime LastGetHangBoxDateTime { get; set; }
    }

    public class CreateBank
    {
        public readonly string AccountId;

        public CreateBank(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class InitStatus
    {
        public string AccountId;

        public InitStatus(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class GetBank
    {
        public readonly string AccountId;


        public GetBank(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class SaveBank
    {
        public readonly PlayerBank PlayerBank;

        public SaveBank(PlayerBank playerBank)
        {
            PlayerBank = playerBank;
        }
    }
}