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

            var deSerialize = ProtoTool.DeSerialize<AMsg>(serialize);
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

                case AMsg.Head.LoginRequest:
                    Console.Out.WriteLine(
                        $"account_id:{deSerialize.loginRequest.accountId}|||password:{deSerialize.loginRequest.Password}");
                    break;
                case AMsg.Head.LoginResponse:
                    break;

                case AMsg.Head.CreateAccountRequest:
                    break;
                case AMsg.Head.CreateAccountResponse:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}