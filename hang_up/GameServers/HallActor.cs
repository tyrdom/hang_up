using System.Collections.Generic;
using Akka.Actor;
using GameProtos;
using MongoDB.Bson;

namespace GameServers
{
    public class OutHall
    {
        public readonly string AccountId;

        public OutHall(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class LoginHall
    {
        public readonly string AccountId;

        public LoginHall(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class InHallOk
    {
        public static readonly InHallOk Instance = new InHallOk();
    }

    public class HallActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> OnlineAccountLink;

        public HallActor()
        {
            OnlineAccountLink = new Dictionary<string, IActorRef>();

            Receive<LoginHall>(ok =>
            {
                var objAccountId = ok.AccountId;
                if (OnlineAccountLink.TryGetValue(objAccountId, out var actorRef))
                {
                    var errorResponse = new ErrorResponse {Reason = "", errorType = ErrorResponse.ErrorType.OtherLogin};
                    actorRef.Tell(errorResponse);
                }

                OnlineAccountLink[objAccountId] = Sender;

                Sender.Tell(InHallOk.Instance);
            });

            Receive<OutHall>(outHall => { OnlineAccountLink.Remove(outHall.AccountId); });
        }
    }
}