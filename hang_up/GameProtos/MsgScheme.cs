// This file was generated by a tool; you should avoid making direct changes.
// Consider using 'partial classes' to extend these types
// Input: MsgScheme.proto

#pragma warning disable CS0612, CS1591, CS3021, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
namespace GameProtos
{

    [global::ProtoBuf.ProtoContract()]
    public partial class AMsg : global::ProtoBuf.IExtensible
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

        [global::ProtoBuf.ProtoMember(3)]
        public UndefinedMsg undefinedMsg
        {
            get { return __pbn__body.Is(3) ? ((UndefinedMsg)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(3, value); }
        }
        public bool ShouldSerializeundefinedMsg() => __pbn__body.Is(3);
        public void ResetundefinedMsg() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 3);

        [global::ProtoBuf.ProtoMember(4)]
        public UndefinedRequest undefinedRequest
        {
            get { return __pbn__body.Is(4) ? ((UndefinedRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(4, value); }
        }
        public bool ShouldSerializeundefinedRequest() => __pbn__body.Is(4);
        public void ResetundefinedRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 4);

        [global::ProtoBuf.ProtoMember(5)]
        public UndefinedResponse undefinedResponse
        {
            get { return __pbn__body.Is(5) ? ((UndefinedResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(5, value); }
        }
        public bool ShouldSerializeundefinedResponse() => __pbn__body.Is(5);
        public void ResetundefinedResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 5);

        [global::ProtoBuf.ProtoMember(6)]
        public LoginRequest loginRequest
        {
            get { return __pbn__body.Is(6) ? ((LoginRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(6, value); }
        }
        public bool ShouldSerializeloginRequest() => __pbn__body.Is(6);
        public void ResetloginRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 6);

        [global::ProtoBuf.ProtoMember(7)]
        public LoginResponse loginResponse
        {
            get { return __pbn__body.Is(7) ? ((LoginResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(7, value); }
        }
        public bool ShouldSerializeloginResponse() => __pbn__body.Is(7);
        public void ResetloginResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 7);

        [global::ProtoBuf.ProtoMember(8)]
        public CreateAccountRequest createAccountRequest
        {
            get { return __pbn__body.Is(8) ? ((CreateAccountRequest)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(8, value); }
        }
        public bool ShouldSerializecreateAccountRequest() => __pbn__body.Is(8);
        public void ResetcreateAccountRequest() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 8);

        [global::ProtoBuf.ProtoMember(9)]
        public CreateAccountResponse createAccountResponse
        {
            get { return __pbn__body.Is(9) ? ((CreateAccountResponse)__pbn__body.Object) : default; }
            set { __pbn__body = new global::ProtoBuf.DiscriminatedUnionObject(9, value); }
        }
        public bool ShouldSerializecreateAccountResponse() => __pbn__body.Is(9);
        public void ResetcreateAccountResponse() => global::ProtoBuf.DiscriminatedUnionObject.Reset(ref __pbn__body, 9);

        [global::ProtoBuf.ProtoContract()]
        public enum Head
        {
            [global::ProtoBuf.ProtoEnum(Name = @"Undefined_Msg")]
            UndefinedMsg = 0,
            [global::ProtoBuf.ProtoEnum(Name = @"Undefined_Request")]
            UndefinedRequest = 1,
            [global::ProtoBuf.ProtoEnum(Name = @"Undefined_Response")]
            UndefinedResponse = 2,
            [global::ProtoBuf.ProtoEnum(Name = @"Error_Response")]
            ErrorResponse = 3,
            [global::ProtoBuf.ProtoEnum(Name = @"Login_Request")]
            LoginRequest = 100001,
            [global::ProtoBuf.ProtoEnum(Name = @"Login_Response")]
            LoginResponse = 100002,
            [global::ProtoBuf.ProtoEnum(Name = @"CreateAccount_Request")]
            CreateAccountRequest = 100003,
            [global::ProtoBuf.ProtoEnum(Name = @"CreateAccount_Response")]
            CreateAccountResponse = 100014,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class CreateAccountRequest : global::ProtoBuf.IExtensible
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
    public partial class CreateAccountResponse : global::ProtoBuf.IExtensible
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
            [global::ProtoBuf.ProtoEnum(Name = @"ALREADY_EXIST")]
            AlreadyExist = 1,
            [global::ProtoBuf.ProtoEnum(Name = @"NO_GOOD_PASSWORD")]
            NoGoodPassword = 2,
            [global::ProtoBuf.ProtoEnum(Name = @"OTHER")]
            Other = 4,
        }

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UndefinedMsg : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UndefinedRequest : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

    }

    [global::ProtoBuf.ProtoContract()]
    public partial class UndefinedResponse : global::ProtoBuf.IExtensible
    {
        private global::ProtoBuf.IExtension __pbn__extensionData;
        global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
            => global::ProtoBuf.Extensible.GetExtensionObject(ref __pbn__extensionData, createIfMissing);

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
            [global::ProtoBuf.ProtoEnum(Name = @"WRONG_PASSWORD")]
            WrongPassword = 1,
            [global::ProtoBuf.ProtoEnum(Name = @"OTHER")]
            Other = 2,
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

#pragma warning restore CS0612, CS1591, CS3021, IDE1006, RCS1036, RCS1057, RCS1085, RCS1192
