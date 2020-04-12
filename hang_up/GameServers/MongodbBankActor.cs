#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.Util;
using Akka.Util.Internal;
using GameProtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GameServers
{
    public static class PlayerBankOp
    {
        static PlayerBank? Use(PlayerBank playerBank, ulong gold, ulong soul, ulong crystal, ulong runePoint,
            Dictionary<uint, uint> items)
        {
            if (playerBank.Gold < gold || playerBank.Soul < soul || playerBank.Crystal < crystal ||
                playerBank.RunePoint < runePoint)
            {
                return null;
            }


            playerBank.Gold -= gold;
            playerBank.Soul -= soul;
            playerBank.Crystal -= crystal;
            playerBank.RunePoint -= runePoint;

            foreach (var (key, value) in items)
            {
                if (playerBank.ItemsIdToNum.TryGetValue(key, out var num))
                {
                    if (num < value)
                    {
                        return null;
                    }

                    playerBank.ItemsIdToNum[key] = value - num;
                }
                else
                {
                    return null;
                }
            }

            return playerBank;
        }

        public static PlayerBank Gain(PlayerBank playerBank, ulong gold, ulong soul, ulong crystal, ulong runePoint,
            Dictionary<uint, uint> items)
        {
            static ulong AddMoney(ulong a, ulong b)
            {
                return Math.Min(OtherConfig.MaxCoinNum, a + b);
            }

            playerBank.Gold = AddMoney(playerBank.Gold, gold);
            playerBank.Soul = AddMoney(playerBank.Soul, soul);
            playerBank.Crystal = AddMoney(playerBank.Crystal, crystal);
            playerBank.RunePoint = AddMoney(playerBank.RunePoint, runePoint);
            foreach (var (key, value) in items)
            {
                if (playerBank.ItemsIdToNum.TryGetValue(key, out var num))
                {
                    playerBank.ItemsIdToNum[key] =
                        Math.Min(OtherConfig.MaxItemNum, value + num);
                }
                else
                {
                    playerBank.ItemsIdToNum.Add(key, value);
                }
            }

            return playerBank;
        }
    }


    public class PlayerBank
    {
        [BsonId] public string AccountId { get; set; }
        public ulong Gold { get; set; }
        public ulong Soul { get; set; }
        public ulong Crystal { get; set; }
        public ulong RunePoint { get; set; }
        public Dictionary<uint, uint> ItemsIdToNum { get; set; }
    }


    public class MongodbBankActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);


        public MongodbBankActor()
        {
            var dbClient = new MongoClient("mongodb://localhost");


            const string dbName = "testDb";
            var mongoDatabase = dbClient.GetDatabase(dbName);
            const string tableName = "player_bank";
            var bankTable = mongoDatabase.GetCollection<PlayerBank>(tableName);


            Receive<CreateBank>(cBank =>
            {
                var filter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, cBank.AccountId);
                var firstOrDefault = bankTable.Find(filter).FirstOrDefault();
                var playerBank = Tools.PlayerNewBank(cBank.AccountId);
                if (firstOrDefault == null)
                {
                    bankTable.InsertOne(playerBank);
                }
            });

            Receive<GetBank>(bank =>
            {
                var accountIdFilter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, bank.AccountId);
                var firstOrDefault = bankTable.Find(accountIdFilter).FirstOrDefault();
                if (firstOrDefault == null)
                {
                    _log.Error($"accountId{bank.AccountId} have no bank, create one~~~~");
                    var playerNewBank = Tools.PlayerNewBank(bank.AccountId);
                    bankTable.InsertOne(playerNewBank);
                    Sender.Tell(playerNewBank);
                }
                else
                {
                    Sender.Tell(firstOrDefault);
                }
            });

            Receive<SaveBank>(saveBank =>
            {
                var bankPlayerBank = saveBank.PlayerBank;
                var filter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, bankPlayerBank.AccountId);
                var firstOrDefault = bankTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null) return;
                bankTable.UpdateOne(filter, new ObjectUpdateDefinition<PlayerBank>(bankPlayerBank));
            });
        }
    }

    public class CreateBank
    {
        public readonly string AccountId;

        public CreateBank(string accountId)
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
        public PlayerBank PlayerBank;

        public SaveBank(PlayerBank playerBank)
        {
            PlayerBank = playerBank;
        }
    }
}