// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: MsgScheme.proto

#pragma warning disable CS1591, CS0612, CS3021, IDE1006
namespace GameProtos
{

    [global::ProtoBuf.ProtoContract()]
    public partial class AMsg : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public Type type { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public RequestMsg requestMsg
        {
            get { return __pbn__msg.Is(2) ? ((RequestMsg)__pbn__msg.Object) : default; }
            set { __pbn__msg = new global::ProtoBuf.DiscriminatedUnionObject(2, value); }
        }
        public bool ShouldSerializerequestMsg() => __pbn__msg.Is(2);
        public void ResetrequestMsg() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__msg, 2);

        private global::ProtoBuf.DiscriminatedUnionObject __pbn__msg;

        [global::ProtoBuf.ProtoMember(3)]
        public ResponseMsg responseMsg
        {
            get { return __pbn__msg.Is(3) ? ((ResponseMsg)__pbn__msg.Object) : default; }
            set { __pbn__msg = new global::ProtoBuf.DiscriminatedUnionObject(3, value); }
        }
        public bool ShouldSerializeresponseMsg() => __pbn__msg.Is(3);
        public void ResetresponseMsg() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__msg, 3);

        [global::ProtoBuf.ProtoContract()]
        public enum Type
        {
            RequestMsg = 0,
            ResponseMsg = 1,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class RequestMsg : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public Head head { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public LoginRequest loginRequest
        {
            get { return __pbn__body.Is(2) ? ((LoginRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(2, value); }
        }
        public bool ShouldSerializeloginRequest() => __pbn__body.Is(2);
        public void ResetloginRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 2);

        private global::ProtoBuf.DiscriminatedUnionObject __pbn__body;

        [global::ProtoBuf.ProtoMember(3)]
        public FixAccountPasswordRequest fixAccountPasswordRequest
        {
            get { return __pbn__body.Is(3) ? ((FixAccountPasswordRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(3, value); }
        }
        public bool ShouldSerializefixAccountPasswordRequest() => __pbn__body.Is(3);
        public void ResetfixAccountPasswordRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 3);

        [global::ProtoBuf.ProtoMember(4)]
        public LogoutRequest logoutRequest
        {
            get { return __pbn__body.Is(4) ? ((LogoutRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(4, value); }
        }
        public bool ShouldSerializelogoutRequest() => __pbn__body.Is(4);
        public void ResetlogoutRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 4);

        [global::ProtoBuf.ProtoMember(5)]
        public BankBaseRequest bankBaseRequest
        {
            get { return __pbn__body.Is(5) ? ((BankBaseRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(5, value); }
        }
        public bool ShouldSerializebankBaseRequest() => __pbn__body.Is(5);
        public void ResetbankBaseRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 5);

        [global::ProtoBuf.ProtoMember(6)]
        public BankAllItemRequest bankItemRequest
        {
            get { return __pbn__body.Is(6) ? ((BankAllItemRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(6, value); }
        }
        public bool ShouldSerializebankItemRequest() => __pbn__body.Is(6);
        public void ResetbankItemRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 6);

        [global::ProtoBuf.ProtoMember(7)]
        public BankCustomItemRequest bankCustomItemRequest
        {
            get { return __pbn__body.Is(7) ? ((BankCustomItemRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(7, value); }
        }
        public bool ShouldSerializebankCustomItemRequest() => __pbn__body.Is(7);
        public void ResetbankCustomItemRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 7);

        [global::ProtoBuf.ProtoContract()]
        public enum Head
        {
            LoginRequest = 0,
            FixAccountPasswordRequest = 1,
            LogoutRequest = 2,
            BankBaseRequest = 3,
            BankItemAllRequest = 4,
            BankItemCustomRequest = 5,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ResponseMsg : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public Head head { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public ErrorResponse errorResponse
        {
            get { return __pbn__body.Is(2) ? ((ErrorResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(2, value); }
        }
        public bool ShouldSerializeerrorResponse() => __pbn__body.Is(2);
        public void ReseterrorResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 2);

        private global::ProtoBuf.DiscriminatedUnionObject __pbn__body;

        [global::ProtoBuf.ProtoMember(7)]
        public LoginResponse loginResponse
        {
            get { return __pbn__body.Is(7) ? ((LoginResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(7, value); }
        }
        public bool ShouldSerializeloginResponse() => __pbn__body.Is(7);
        public void ResetloginResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 7);

        [global::ProtoBuf.ProtoMember(9)]
        public FixAccountPasswordResponse fixAccountPasswordResponse
        {
            get { return __pbn__body.Is(9) ? ((FixAccountPasswordResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(9, value); }
        }
        public bool ShouldSerializefixAccountPasswordResponse() => __pbn__body.Is(9);
        public void ResetfixAccountPasswordResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 9);

        [global::ProtoBuf.ProtoMember(11)]
        public LogoutResponse logoutResponse
        {
            get { return __pbn__body.Is(11) ? ((LogoutResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(11, value); }
        }
        public bool ShouldSerializelogoutResponse() => __pbn__body.Is(11);
        public void ResetlogoutResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 11);

        [global::ProtoBuf.ProtoMember(13)]
        public BankBaseResponse bankBaseResponse
        {
            get { return __pbn__body.Is(13) ? ((BankBaseResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(13, value); }
        }
        public bool ShouldSerializebankBaseResponse() => __pbn__body.Is(13);
        public void ResetbankBaseResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 13);

        [global::ProtoBuf.ProtoMember(16)]
        public BankItemResponse bankItemResponse
        {
            get { return __pbn__body.Is(16) ? ((BankItemResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(16, value); }
        }
        public bool ShouldSerializebankItemResponse() => __pbn__body.Is(16);
        public void ResetbankItemResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 16);

        [global::ProtoBuf.ProtoContract()]
        public enum Head
        {
            ErrorResponse = 0,
            LoginResponse = 100002,
            FixAccountPasswordResponse = 100004,
            LogoutResponse = 100006,
            BankBaseResponse = 100008,
            BankItemResponse = 100011,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class BankCustomItemRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1, Name = @"itemId", IsPacked = true)]
        public uint[] itemIds { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class BankAllItemRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class BankBaseRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class BankBaseResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public ulong Gold { get; set; }

        [global::ProtoBuf.ProtoMember(2)]
        public ulong Soul { get; set; }

        [global::ProtoBuf.ProtoMember(3)]
        public ulong Crystal { get; set; }

        [global::ProtoBuf.ProtoMember(4)]
        public ulong RunePoint { get; set; }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class BankItemResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public global::System.Collections.Generic.List<Item> Items { get; set; } = new global::System.Collections.Generic.List<Item>();

        [global::ProtoBuf.ProtoContract()]
        public partial class Item : global::ProtoBuf.IExtensible
        {
            private global::ProtoBuf.IExtension __pbn__extensionData;
            global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
                => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

            [global::ProtoBuf.ProtoMember(1)]
            public uint itemId { get; set; }

            [global::ProtoBuf.ProtoMember(2, Name = @"num")]
            public uint Num { get; set; }

        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class LogoutResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class LogoutRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class FixAccountPasswordRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        [global::System.ComponentModel.DefaultValue("")]
        public string accountId { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2)]
        [global::System.ComponentModel.DefaultValue("")]
        public string oldPassword { get; set; } = "";

        [global::ProtoBuf.ProtoMember(3)]
        [global::System.ComponentModel.DefaultValue("")]
        public string newPassword { get; set; } = "";

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class FixAccountPasswordResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public Reason reason { get; set; }

        [global::ProtoBuf.ProtoContract()]
        public enum Reason
        {
            [global::ProtoBuf.ProtoEnum(Name = @"OK")]
            Ok = 0,
            [global::ProtoBuf.ProtoEnum(Name = @"NO_GOOD_ACCOUNT_ID")]
            NoGoodAccountId = 1,
            [global::ProtoBuf.ProtoEnum(Name = @"NO_GOOD_PASSWORD")]
            NoGoodPassword = 2,
            [global::ProtoBuf.ProtoEnum(Name = @"ACCOUNT_NOT_EXIST")]
            AccountNotExist = 3,
            [global::ProtoBuf.ProtoEnum(Name = @"WRONG_PASSWORD")]
            WrongPassword = 4,
            [global::ProtoBuf.ProtoEnum(Name = @"OTHER")]
            Other = 5,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class LoginRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        [global::System.ComponentModel.DefaultValue("")]
        public string accountId { get; set; } = "";

        [global::ProtoBuf.ProtoMember(2, Name = @"password")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Password { get; set; } = "";

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class LoginResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public Reason reason { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"nickname")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Nickname { get; set; } = "";

        [global::ProtoBuf.ProtoContract()]
        public enum Reason
        {
            [global::ProtoBuf.ProtoEnum(Name = @"OK")]
            Ok = 0,
            [global::ProtoBuf.ProtoEnum(Name = @"NO_GOOD_ACCOUNT")]
            NoGoodAccount = 1,
            [global::ProtoBuf.ProtoEnum(Name = @"NO_GOOD_PASSWORD")]
            NoGoodPassword = 2,
            [global::ProtoBuf.ProtoEnum(Name = @"WRONG_PASSWORD")]
            WrongPassword = 3,
            [global::ProtoBuf.ProtoEnum(Name = @"NOT_EXIST_SO_CREATE")]
            NotExistSoCreate = 4,
            [global::ProtoBuf.ProtoEnum(Name = @"OTHER")]
            Other = 5,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class ErrorResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

        [global::ProtoBuf.ProtoMember(1)]
        public ErrorType errorType { get; set; }

        [global::ProtoBuf.ProtoMember(2, Name = @"reason")]
        [global::System.ComponentModel.DefaultValue("")]
        public string Reason { get; set; } = "";

        [global::ProtoBuf.ProtoContract()]
        public enum ErrorType
        {
            [global::ProtoBuf.ProtoEnum(Name = @"UNKNOWN")]
            Unknown = 0,
            [global::ProtoBuf.ProtoEnum(Name = @"OTHER_LOGIN")]
            OtherLogin = 1,
        }

    }

}

#pragma warning restore CS1591, CS0612, CS3021, IDE1006
