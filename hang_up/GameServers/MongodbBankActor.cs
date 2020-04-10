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
    public class PlayerBank
    {
        [BsonId] public string AccountId { get; set; }
        public long Gold { get; set; }
        public long Soul { get; set; }
        public long Crystal { get; set; }
        public long RunePoint { get; set; }
        public List<BankItem> Items { get; set; }
    }

    public class BankItem
    {
        public int ItemId { get; set; }
        public int Num { get; set; }
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


            Receive<CreateBank>(bank =>
            {
                var filter = Builders<PlayerBank>.Filter.Eq(x => x.AccountId, bank.AccountId);
                var firstOrDefault = bankTable.Find(filter).FirstOrDefault();
                var playerBank = Tools.PlayerNewBank(bank.AccountId);
                if (firstOrDefault == null)
                {
                    bankTable.InsertOne(playerBank);
                }

                var bankBaseResponse = new BankBaseResponse
                {
                    Gold = playerBank.Gold, Soul = playerBank.Soul, Crystal = playerBank.Crystal,
                    RunePoint = playerBank.RunePoint
                };
                Sender.Tell(bankBaseResponse);//TODO sthbug
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
                    Sender.Tell(Tools.GenBankBaseResponseByPlayBank(playerNewBank));
                }
                else
                {
                    switch (bank.GetBankType)
                    {
                        case GetBankType.Base:
                            Sender.Tell(Tools.GenBankBaseResponseByPlayBank(firstOrDefault));
                            break;
                        case GetBankType.Item:
                            Sender.Tell(Tools.GenBankItemResponseByPlayBank(firstOrDefault));
                            break;
                        case GetBankType.CustomItem:
                            Sender.Tell(Tools.GenBankItemResponseByPlayBank(firstOrDefault, bank.ItemIds));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
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
        public readonly GetBankType GetBankType;
        public readonly int[]? ItemIds;


        public GetBank(string accountId, GetBankType bankType, int[]? itemIds)
        {
            AccountId = accountId;
            GetBankType = bankType;
            ItemIds = itemIds;
        }
    }

    public enum GetBankType
    {
        Base,
        Item,
        CustomItem
    }
}