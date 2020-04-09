using System.Collections.Generic;
using Akka.Actor;
using GameProtos;

namespace GameServers
{
    public class LoginOk
    {
        public string AccountId;

        public LoginOk(string accountId)
        {
            AccountId = accountId;
        }
    }

    public class HallActor : ReceiveActor
    {
        private Dictionary<string, IActorRef> OnlineAccountLink;

        public HallActor()
        {
            OnlineAccountLink = new Dictionary<string, IActorRef>();

            Receive<LoginOk>(ok =>
            {
                var objAccountId = ok.AccountId;
                if (OnlineAccountLink.TryGetValue(objAccountId, out var actorRef))
                {
                    var errorResponse = new ErrorResponse {Reason = "", errorType = ErrorResponse.ErrorType.OtherLogin};
                    actorRef.Tell(errorResponse);
                }

                OnlineAccountLink[objAccountId] = Sender;
            });
        }
    }
}