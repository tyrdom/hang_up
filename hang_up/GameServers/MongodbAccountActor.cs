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
        [BsonId] public string AccountId { get; set; }
        public string NickName { get; set; }
        public string Password { get; set; }
        public DateTime LastInTime { get; set; }
        public DateTime LastOutTime { get; set; }
    }

    public class MongodbAccountActor : ReceiveActor
    {
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public MongodbAccountActor()
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
            var accountBaseTable = mongoDatabase.GetCollection<PlayerBase>(playerBase);


            // var p = new PlayerBase
            // {
            //     AccountId = "test1",Password = "12341235",LastInTime = DateTime.Now,LastOutTime = DateTime.Now
            //     
            // };
            // var q = new PlayerBase
            // {AccountId = "test2",Password = "123412376",LastInTime = DateTime.Now,LastOutTime = DateTime.Now
            // };
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
            // var filter1 = Builders<PlayerBase>.Filter.Eq(x => x.AccountId, "text1");
            //
            // var doc = mongoCollection.Find(filter1).FirstOrDefault();
            //
            // Console.Out.WriteLine(doc == null ? "doc is null" : $"!!!{doc.AccountId}");


            Receive<LoginRequest>(request =>
            {
                var requestAccountId = request.accountId;
                var requestPassword = request.Password;

                var filter = Builders<PlayerBase>.Filter.Eq(x => x.AccountId, requestAccountId);
                var firstOrDefault = accountBaseTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null)
                {
                    var @base = new PlayerBase()
                    {
                        AccountId = requestAccountId, Password = Tools.GetMd5Hash(requestPassword),
                        NickName = requestAccountId, LastInTime = DateTime.Now,
                        LastOutTime = DateTime.MinValue
                    };
                    accountBaseTable.InsertOne(@base);
                    FamousActors.MongodbBankActor.Tell(new CreateBank (requestAccountId));
                    var loginResponse = new LoginResponse()
                        {reason = LoginResponse.Reason.NotExistSoCreate, Nickname = requestAccountId};
                    Sender.Tell(loginResponse);
                }
                else
                {
                    if (Tools.VerifyMd5Hash(requestPassword, firstOrDefault.Password))
                    {
                        var loginResponse = new LoginResponse {reason = LoginResponse.Reason.Ok, Nickname = firstOrDefault.NickName};
                        Sender.Tell(loginResponse);
                    }
                    else
                    {
                        var loginResponse = new LoginResponse {reason = LoginResponse.Reason.WrongPassword, Nickname = ""};
                        Sender.Tell(loginResponse);
                    }
                }
            });

            Receive<FixAccountPasswordRequest>(request =>
            {
                var requestAccountId = request.accountId;
                var requestOldPassword = request.oldPassword;
                var requestNewPassword = request.newPassword;

                var filter = Builders<PlayerBase>.Filter.Eq(x => x.AccountId, requestAccountId);
                var firstOrDefault = accountBaseTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null)
                {
                    Sender.Tell(new FixAccountPasswordResponse()
                        {
                            reason = FixAccountPasswordResponse.Reason.AccountNotExist
                        }
                    );
                }
                else
                {
                    if (Tools.VerifyMd5Hash(requestOldPassword, firstOrDefault.Password))
                    {
                        firstOrDefault.Password = requestNewPassword;
                        accountBaseTable.UpdateOne(filter, new ObjectUpdateDefinition<PlayerBase>(firstOrDefault));

                        Sender.Tell(new FixAccountPasswordResponse()
                        {
                            reason = FixAccountPasswordResponse.Reason.Ok
                        });
                    }
                    else
                    {
                        Sender.Tell(new FixAccountPasswordResponse()
                            {reason = FixAccountPasswordResponse.Reason.WrongPassword});
                    }
                }
            });


            Receive<Logout>(logout =>
            {
                var logoutAccountId = logout.AccountId;
                var filter = Builders<PlayerBase>.Filter.Eq(x => x.AccountId, logoutAccountId);
                var firstOrDefault = accountBaseTable.Find(filter).FirstOrDefault();
                if (firstOrDefault == null) return;
                firstOrDefault.LastOutTime = DateTime.Now;
                accountBaseTable.UpdateOne(filter, new ObjectUpdateDefinition<PlayerBase>(firstOrDefault));
                if (logout.OutReason == OutReason.LogOut)
                {
                    Sender.Tell(new LogoutResponse());
                }
            });
        }
    }

    public class Logout
    {
        public readonly string AccountId;
        public readonly OutReason OutReason;

        public Logout(string accountId, OutReason outReason)
        {
            AccountId = accountId;
            OutReason = outReason;
        }
    }

    public enum OutReason
    {
        LogOut,
        Drop
    }
}