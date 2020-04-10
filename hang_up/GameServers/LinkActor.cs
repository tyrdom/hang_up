using System;
using System.Net;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using GameProtos;


namespace GameServers
{
    enum GameState
    {
        Online,
        OffLine
    }

    public class LinkActor : ReceiveActor
    {
        private readonly IActorRef _tcpActorRef;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        private string _accountId;
        private EndPoint _remote;
        private LoginResponse _temp;
        private GameState _gameState = GameState.OffLine;

        public LinkActor(EndPoint remote, IActorRef tcpActorRef)
        {
            _tcpActorRef = tcpActorRef;
            _remote = remote;
            Context.Watch(_tcpActorRef);
            Receive<Tcp.Received>(received =>
            {
                var aMsg = ProtoTool.DeSerialize<AMsg>(received.Data.ToArray());
                var aMsgType = aMsg.type;
                if (aMsgType == AMsg.Type.RequestMsg)
                {
                    var aMsgRequestMsg = aMsg.requestMsg;
                    switch (aMsgRequestMsg.head)
                    {
                        case RequestMsg.Head.LoginRequest:
                            var aMsgLoginRequest = aMsgRequestMsg.loginRequest;
                            var accountId = aMsgLoginRequest.accountId;
                            var password = aMsgLoginRequest.Password;

                            if (!Tools.CheckAccountIdOk(accountId))
                            {
                                var loginResponse = new LoginResponse()
                                    {reason = LoginResponse.Reason.NoGoodAccount, Nickname = ""};

                                Sender.Tell(ProtoTool.Serialize(loginResponse));
                            }
                            else if (!Tools.CheckPasswordOk(password))
                            {
                                var loginResponse = new LoginResponse()
                                    {reason = LoginResponse.Reason.NoGoodPassword, Nickname = ""};
                                Sender.Tell(ProtoTool.Serialize(loginResponse));
                            }
                            else
                            {
                                _accountId = accountId;
                                FamousActors.MongodbAccountActor.Tell(aMsgLoginRequest);
                            }

                            break;
                        case RequestMsg.Head.FixAccountPasswordRequest:
                            var aMsgFixAccountPasswordRequest = aMsgRequestMsg.fixAccountPasswordRequest;

                            if (!Tools.CheckAccountIdOk(aMsgFixAccountPasswordRequest.accountId))
                            {
                                var loginResponse = new FixAccountPasswordResponse()
                                    {reason = FixAccountPasswordResponse.Reason.NoGoodAccountId};

                                Sender.Tell(ProtoTool.Serialize(loginResponse));
                            }
                            else if (!Tools.CheckPasswordOk(aMsgFixAccountPasswordRequest.newPassword) ||
                                     !Tools.CheckPasswordOk(aMsgFixAccountPasswordRequest.oldPassword))
                            {
                                var loginResponse = new FixAccountPasswordResponse()
                                    {reason = FixAccountPasswordResponse.Reason.NoGoodPassword};
                                Sender.Tell(ProtoTool.Serialize(loginResponse));
                            }
                            else
                            {
                                FamousActors.MongodbAccountActor.Tell(aMsgFixAccountPasswordRequest);
                            }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });

            Receive<LoginResponse>(response =>
            {
                var b = response.reason == LoginResponse.Reason.Ok;
                var b1 = response.reason == LoginResponse.Reason.NotExistSoCreate;
                if (b || b1)
                {
                    _temp = response;
                    FamousActors.HallActor.Tell(new LoginHall(_accountId));
                }
                else
                {
                    _tcpActorRef.Tell(ProtoTool.Serialize(new AMsg
                    {
                        type = AMsg.Type.ResponseMsg, responseMsg = new ResponseMsg
                        {
                            head = ResponseMsg.Head.LoginResponse, loginResponse = response
                        }
                    }));
                }
            });

            Receive<FixAccountPasswordResponse>(response =>
            {
                var serialize = ProtoTool.Serialize(new AMsg()
                    {
                        type = AMsg.Type.ResponseMsg, responseMsg =
                            new ResponseMsg
                            {
                                head = ResponseMsg.Head.FixAccountPasswordResponse,
                                fixAccountPasswordResponse = response
                            }
                    }
                );
                _tcpActorRef.Tell(serialize);
            });

            Receive<InHallOk>(ok =>
            {
                _gameState = GameState.Online;
                var aMsg = new AMsg
                {
                    type = AMsg.Type.ResponseMsg,
                    responseMsg = {head = ResponseMsg.Head.LoginResponse, loginResponse = _temp}
                };

                _tcpActorRef.Tell(ProtoTool.Serialize(aMsg));
                Become(OnLine);
            });

            Receive<ErrorResponse>(response =>
                _tcpActorRef.Tell(ProtoTool.Serialize(
                    new AMsg
                    {
                        type = AMsg.Type.ResponseMsg,
                        responseMsg = {head = ResponseMsg.Head.ErrorResponse, errorResponse = response}
                    }
                ))
            );


            Receive<Tcp.ConnectionClosed>(closed =>
            {
                _log.Info($"Stopped, remote connection [{remote}] closed");
                Context.Stop(Self);
            });
            Receive<Terminated>(terminated =>
            {
                _log.Info($"Stopped, remote connection [{remote}] died");

                Context.Stop(Self);
            });
        }

        private void OnLine()
        {
            Receive<ErrorResponse>(response =>
                _tcpActorRef.Tell(ProtoTool.Serialize(
                    new AMsg
                    {
                        type = AMsg.Type.ResponseMsg,
                        responseMsg = {head = ResponseMsg.Head.ErrorResponse, errorResponse = response}
                    }
                ))
            );

            Receive<Tcp.ConnectionClosed>(closed =>
            {
                FamousActors.HallActor.Tell(new OutHall(_accountId));
                FamousActors.MongodbAccountActor.Tell(new Logout
                    (_accountId, OutReason.Drop));
                _log.Info($"Stopped, remote connection [{_remote}] closed");
                Context.Stop(Self);
            });
            Receive<Terminated>(terminated =>
            {
                FamousActors.HallActor.Tell(new OutHall(_accountId));
                FamousActors.MongodbAccountActor.Tell(new Logout
                    (_accountId, OutReason.Drop));
                _log.Info($"Stopped, remote connection [{_remote}] died");

                Context.Stop(Self);
            });

            Receive<LogoutResponse>(response =>
            {
                _tcpActorRef.Tell(ProtoTool.Serialize(new AMsg
                {
                    type = AMsg.Type.ResponseMsg,
                    responseMsg = {head = ResponseMsg.Head.LogoutResponse, logoutResponse = response}
                }));
            });
            Receive<Tcp.Received>(received =>
            {
                var aMsg = ProtoTool.DeSerialize<AMsg>(received.Data.ToArray());
                var aMsgType = aMsg.type;

                if (aMsgType == AMsg.Type.RequestMsg && _gameState == GameState.Online && _accountId != null)
                {
                    var aMsgRequestMsg = aMsg.requestMsg;
                    switch (aMsgRequestMsg.head)
                    {
                        case RequestMsg.Head.BankBaseRequest:

                            FamousActors.MongodbBankActor.Tell(new GetBank(_accountId, GetBankType.Base,
                                null));
                            break;

                        case RequestMsg.Head.BankItemAllRequest:
                            FamousActors.MongodbBankActor.Tell(new GetBank(_accountId, GetBankType.Item,
                                null));
                            break;

                        case RequestMsg.Head.BankItemCustomRequest:
                            FamousActors.MongodbBankActor.Tell(new GetBank(_accountId,
                                GetBankType.CustomItem,
                                aMsgRequestMsg.bankCustomItemRequest.itemIds));

                            break;
                        case RequestMsg.Head.LogoutRequest:
                            FamousActors.MongodbAccountActor.Tell(new Logout
                                (_accountId, OutReason.LogOut));
                            FamousActors.HallActor.Tell(new OutHall(_accountId));
                            _gameState = GameState.OffLine;
                            break;
                        default:
                            _tcpActorRef.Tell(Tcp.Close.Instance);
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}