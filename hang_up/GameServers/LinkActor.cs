using System;
using System.Net;
using System.Text;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using GameProtos;


namespace GameServers
{
    internal enum GameState
    {
        Online,
        OffLine
    }

    public class LinkActor : ReceiveActor
    {
        private readonly IActorRef _tcpActorRef;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);
        private string _accountId;
        private readonly EndPoint _remote;
        private LoginResponse _temp;
        private GameState _gameState = GameState.OffLine;


        private PlayerBank _myWallet;
        private PlayerCharacters _myCharacters;
        private PlayerGames _myGames;

        private static Tcp.Write GenTcpWrite(AMsg aMsg)
        {
            return Tcp.Write.Create(ByteString.FromBytes(ProtoTool.Serialize(aMsg)));
        }

        public LinkActor(EndPoint remote, IActorRef tcpActorRef)
        {
            _tcpActorRef = tcpActorRef;
            _remote = remote;
            Context.Watch(_tcpActorRef);
            Become(OffLine);
        }

        private void OffLineSave(OutReason reason)
        {
            FamousActors.HallActor.Tell(new OutHall(_accountId));
            FamousActors.MongodbAccountActor.Tell(new Logout
                (_accountId, reason));
            FamousActors.MongodbPlayerStatusActor.Tell(new SaveBank(_myWallet));
            FamousActors.MongodbPlayerStatusActor.Tell(new SaveCharacters(_myCharacters));
        }

        private void OffLine()
        {
            Receive<Tcp.Received>(received =>
            {
                if (!Sender.Equals(_tcpActorRef))
                {
                    _log.Error($"link error close link!");
                    Sender.Tell(Tcp.Close.Instance);
                    return;
                }

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
                                var msg = new AMsg()
                                {
                                    type = AMsg.Type.ResponseMsg,
                                    responseMsg = new ResponseMsg()
                                        {head = ResponseMsg.Head.LoginResponse, loginResponse = loginResponse}
                                };
                                Sender.Tell(GenTcpWrite(msg));
                            }
                            else if (!Tools.CheckPasswordOk(password))
                            {
                                var loginResponse = new LoginResponse()
                                    {reason = LoginResponse.Reason.NoGoodPassword, Nickname = ""};
                                var msg = new AMsg()
                                {
                                    type = AMsg.Type.ResponseMsg,
                                    responseMsg = new ResponseMsg()
                                        {head = ResponseMsg.Head.LoginResponse, loginResponse = loginResponse}
                                };
                                Sender.Tell(GenTcpWrite(msg));
                            }
                            else
                            {
                                _accountId = accountId;
                                FamousActors.MongodbAccountActor.Tell(aMsgLoginRequest);
                            }

                            Become(LoginDoing);
                            break;
                        case RequestMsg.Head.FixAccountPasswordRequest:
                            var aMsgFixAccountPasswordRequest = aMsgRequestMsg.fixAccountPasswordRequest;

                            if (!Tools.CheckAccountIdOk(aMsgFixAccountPasswordRequest.accountId))
                            {
                                var loginResponse = new FixAccountPasswordResponse
                                    {reason = FixAccountPasswordResponse.Reason.NoGoodAccountId};
                                var msg = new AMsg()
                                {
                                    type = AMsg.Type.ResponseMsg,
                                    responseMsg = new ResponseMsg()
                                    {
                                        head = ResponseMsg.Head.FixAccountPasswordResponse,
                                        fixAccountPasswordResponse = loginResponse
                                    }
                                };
                                Sender.Tell(GenTcpWrite(msg));
                            }
                            else if (!Tools.CheckPasswordOk(aMsgFixAccountPasswordRequest.newPassword) ||
                                     !Tools.CheckPasswordOk(aMsgFixAccountPasswordRequest.oldPassword))
                            {
                                var loginResponse = new FixAccountPasswordResponse
                                    {reason = FixAccountPasswordResponse.Reason.NoGoodPassword};
                                var msg = new AMsg()
                                {
                                    type = AMsg.Type.ResponseMsg,
                                    responseMsg = new ResponseMsg()
                                    {
                                        head = ResponseMsg.Head.FixAccountPasswordResponse,
                                        fixAccountPasswordResponse = loginResponse
                                    }
                                };
                                Sender.Tell(GenTcpWrite(msg));
                            }
                            else
                            {
                                FamousActors.MongodbAccountActor.Tell(aMsgFixAccountPasswordRequest);
                            }

                            break;

                        default:
                            _tcpActorRef.Tell(Tcp.Close.Instance);
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });


            Receive<FixAccountPasswordResponse>(response =>
            {
                var genTcpWrite = GenTcpWrite(new AMsg()
                    {
                        type = AMsg.Type.ResponseMsg, responseMsg =
                            new ResponseMsg
                            {
                                head = ResponseMsg.Head.FixAccountPasswordResponse,
                                fixAccountPasswordResponse = response
                            }
                    }
                );
                _tcpActorRef.Tell(genTcpWrite);
            });

            NormalAccident();
        }

        void NormalAccident()
        {
            Receive<ErrorResponse>(response =>
                _tcpActorRef.Tell(GenTcpWrite(
                    new AMsg
                    {
                        type = AMsg.Type.ResponseMsg,
                        responseMsg = {head = ResponseMsg.Head.ErrorResponse, errorResponse = response}
                    }
                ))
            );


            Receive<Tcp.ConnectionClosed>(closed =>
            {
                _log.Info($"Stopped, remote connection [{_remote}] closed");
                Context.Stop(Self);
            });
            Receive<Terminated>(terminated =>
            {
                _log.Info($"Stopped, remote connection [{_remote}] died");

                Context.Stop(Self);
            });
        }

        private void LoginDoing()
        {
            Receive<InHallOk>(ok =>
            {
                _gameState = GameState.Online;
                Become(OnLine);
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
                    _tcpActorRef.Tell(GenTcpWrite(new AMsg
                    {
                        type = AMsg.Type.ResponseMsg, responseMsg = new ResponseMsg
                        {
                            head = ResponseMsg.Head.LoginResponse, loginResponse = response
                        }
                    }));
                }
            });
            NormalAccident();
        }

        private void OnLine()
        {
            FamousActors.MongodbPlayerStatusActor.Tell(new InitStatus(_accountId));

            // ICancelable scheduleTellRepeatedlyCancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
            //     TimeSpan.FromMinutes(10),
            //     TimeSpan.FromMinutes(10),
            //     Self, SavePlayerDB.Instance
            //     , ActorRefs.Nobody);
            void ReallyLoginOk()
            {
                _temp.bankBaseResponse = Tools.GenBankBaseResponseByPlayBank(_myWallet);
                _temp.charactersGetResponse = Tools.GenCharactersGetResponseByPlayerCharacters(_myCharacters);
                var aMsg = new AMsg
                {
                    type = AMsg.Type.ResponseMsg,
                    responseMsg = {head = ResponseMsg.Head.LoginResponse, loginResponse = _temp}
                };
                _tcpActorRef.Tell(GenTcpWrite(aMsg));
            }

            Receive<PlayerStatus>(status =>
            {
                _myWallet = status.PlayerBank;
                _myGames = status.PlayerGames;
                _myCharacters = status.PlayerCharacters;
                ReallyLoginOk();
            });


            Receive<ErrorResponse>(response =>
                _tcpActorRef.Tell(GenTcpWrite(
                    new AMsg
                    {
                        type = AMsg.Type.ResponseMsg,
                        responseMsg = {head = ResponseMsg.Head.ErrorResponse, errorResponse = response}
                    }
                ))
            );
            Receive<Tcp.ConnectionClosed>(closed =>
            {
                OffLineSave(OutReason.Drop);
                _log.Info($"Stopped, remote connection [{_remote}] closed");
                Context.Stop(Self);
            });
            Receive<Terminated>(terminated =>
            {
                OffLineSave(OutReason.Drop);
                _log.Info($"Stopped, remote connection [{_remote}] died");

                Context.Stop(Self);
            });

            Receive<LogoutResponse>(response =>
            {
                _tcpActorRef.Tell(GenTcpWrite(new AMsg
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

                            var genBankBaseResponseByPlayBank = Tools.GenBankBaseResponseByPlayBank(_myWallet);
                            Sender.Tell(GenTcpWrite(new AMsg()
                            {
                                type = AMsg.Type.ResponseMsg,
                                responseMsg = new ResponseMsg()
                                {
                                    head = ResponseMsg.Head.BankBaseResponse,
                                    bankBaseResponse = genBankBaseResponseByPlayBank
                                }
                            }));
                            break;

                        case RequestMsg.Head.BankItemAllRequest:

                            var genBankItemAllResponseByPlayBank = Tools.GenBankItemResponseByPlayBank(_myWallet);
                            Sender.Tell(GenTcpWrite(new AMsg()
                            {
                                type = AMsg.Type.ResponseMsg,
                                responseMsg = new ResponseMsg()
                                {
                                    head = ResponseMsg.Head.BankItemResponse,
                                    bankItemResponse = genBankItemAllResponseByPlayBank
                                }
                            }));
                            break;

                        case RequestMsg.Head.BankItemCustomRequest:
                            var genBankItemResponseByPlayBank =
                                Tools.GenBankItemResponseByPlayBank(_myWallet,
                                    aMsgRequestMsg.bankCustomItemRequest.itemIds);
                            Sender.Tell(GenTcpWrite(new AMsg()
                            {
                                type = AMsg.Type.ResponseMsg,
                                responseMsg = new ResponseMsg()
                                {
                                    head = ResponseMsg.Head.BankItemResponse,
                                    bankItemResponse = genBankItemResponseByPlayBank
                                }
                            }));
                            break;
                        
                        
                        case RequestMsg.Head.LogoutRequest:

                            OffLineSave(OutReason.LogOut);
                            _gameState = GameState.OffLine;
                            Become(OffLine);
                            break;
                        default:
                            _tcpActorRef.Tell(Tcp.Close.Instance);
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    _tcpActorRef.Tell(Tcp.Close.Instance);
                    throw new ArgumentOutOfRangeException();
                }
            });
        }
    }
}