using System;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using GameProtos;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace GameServers
{
    public class PlayerBase
    {
        [BsonId]
        public string AccountId { get; set; }
        public string Password { get; set; }
        public DateTime LastInTime { get; set; }
        public DateTime LastOutTime { get; set; }
    }

    public class MongodbBaseActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public MongodbBaseActor()
        {
            var dbClient = new MongoClient("mongodb://localhost");
            var dbList = dbClient.ListDatabases().ToList();
            foreach (var item in dbList)
            {
                _log.Info(item.ToString());
            }

            var dbName = "testDb";
            var mongoDatabase = dbClient.GetDatabase(dbName);
            var playerBase = "player_base";
            var mongoCollection = mongoDatabase.GetCollection<PlayerBase>(playerBase);
           


            var p = new PlayerBase
            {
                AccountId = "test1",Password = "12341235",LastInTime = DateTime.Now,LastOutTime = DateTime.Now
                
            };
            var q = new PlayerBase
            {AccountId = "test2",Password = "123412376",LastInTime = DateTime.Now,LastOutTime = DateTime.Now
            };
            //
            // try
            // {
            //     mongoCollection.InsertOne(p);
            //     mongoCollection.InsertOne(q);
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine(e);
            //     throw;
            // }

            var filter = Builders<PlayerBase>.Filter.Eq(x => x.AccountId, "test1");

            var doc = mongoCollection.Find(filter).FirstOrDefault();
          
            
                Console.Out.WriteLine($"!!!{doc.AccountId}");
            
          

            // var documents = mongoCollection.Find(new BsonDocument()).ToList();
            // foreach (BsonDocument document in documents)
            // {
            //     Console.WriteLine(document.ToString());
            // }
             // mongoDatabase.DropCollection(playerBase);
            Receive<LoginRequest>(request =>
            {
                var filter = Builders<PlayerBase>.Filter.Eq(x => x.AccountId, request.accountId); 
                
            });
            Receive<CreateAccountRequest>(request =>
            {
                string aaa = "aaa";
                
            });
        }
    }
}