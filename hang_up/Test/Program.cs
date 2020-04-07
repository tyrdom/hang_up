using System;
using GameProtos;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
              var aMsg = new AMsg
            {
                head = AMsg.Head.LoginRequest, loginRequest = new LoginRequest {accountId = "sdwe", Password = "sddwe"}
            };

            var serialize = ProtoTool.Serialize(aMsg);
            Console.Out.WriteLine($"after serialize:{serialize.Length}");

            AMsg deSerialize = ProtoTool.DeSerialize<AMsg>(serialize);
            switch (deSerialize.head)
            {
                case AMsg.Head.UndefinedMsg:
                    break;
                case AMsg.Head.UndefinedRequest:
                    break;
                case AMsg.Head.UndefinedResponse:
                    break;
                case AMsg.Head.ErrorResponse:
                    break;
                case AMsg.Head.TestRequest:
                    break;
                case AMsg.Head.TestResponse:
                    break;
                case AMsg.Head.LoginRequest:
                    Console.Out.WriteLine(
                        $"account_id:{deSerialize.loginRequest.accountId}|||password:{deSerialize.loginRequest.Password}");
                    break;
                case AMsg.Head.LoginResponse:
                    break;
                case AMsg.Head.CreateRoomRequest:
                    break;
                case AMsg.Head.CreateRoomResponse:
                    break;
                case AMsg.Head.JoinRoomRequest:
                    break;
                case AMsg.Head.JoinRoomResponse:
                    break;
                case AMsg.Head.QuitRoomRequest:
                    break;
                case AMsg.Head.QuitRoomResponse:
                    break;
                case AMsg.Head.CreateAccountRequest:
                    break;
                case AMsg.Head.CreateAccountResponse:
                    break;
                case AMsg.Head.ChangeNicknameRequest:
                    break;
                case AMsg.Head.ChangeNicknameResponse:
                    break;
                case AMsg.Head.RoomPlayerStatusBroadcast:
                    break;
                case AMsg.Head.HallPlayerStatusBroadcast:
                    break;
                case AMsg.Head.GetReadyRequest:
                    break;
                case AMsg.Head.GetReadyResponse:
                    break;
                case AMsg.Head.PlaygroundBroadcast:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}