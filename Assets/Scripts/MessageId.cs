// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: proto/messageId.proto
// </auto-generated>
#pragma warning disable 1591, 0612, 3021, 8981
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace MessageId {

  /// <summary>Holder for reflection information generated from proto/messageId.proto</summary>
  public static partial class MessageIdReflection {

    #region Descriptor
    /// <summary>File descriptor for proto/messageId.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static MessageIdReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChVwcm90by9tZXNzYWdlSWQucHJvdG8SCW1lc3NhZ2VJZCrdBAoJTWVzc2Fn",
            "ZUlkEggKBE5vbmUQABIbChVDMlNfUmVnaXN0ZXJfQWNjb291bnQQ0YYDEhsK",
            "FVMyQ19SZWdpc3Rlcl9BY2Nvb3VudBDShgMSFgoQQzJTX0NyZWF0ZVBsYXll",
            "chDThgMSFgoQUzJDX0NyZWF0ZVBsYXllchDUhgMSDwoJQzJTX0xvZ2luENWG",
            "AxIPCglTMkNfTG9naW4Q1oYDEg4KCFMyQ19LaWNrENeGAxISCgxDMkdTX01z",
            "Z1BpbmcQgfEEEhIKDEMyR1NfTXNnUG9uZxCC8QQSFQoPQzJHU19FbnRlclNl",
            "bmNlEKGNBhIVCg9HUzJDX0VudGVyU2VuY2UQoo0GEhUKD0MyR1NfUGxheWVy",
            "TW92ZRCjjQYSFQoPR1MyQ19QbGF5ZXJNb3ZlEKSNBhIWChBDMkdTX1BsYXll",
            "ckxlYXZlEKWNBhIWChBHUzJDX1BsYXllckxlYXZlEKaNBhIUCg5DMkdTX0Fk",
            "ZEZyaWVuZBCnjQYSFAoOR1MyQ19BZGRGcmllbmQQqI0GEhQKDkMyR1NfRGVs",
            "RnJpZW5kEKmNBhIUCg5HUzJDX0RlbEZyaWVuZBCqjQYSFgoQQzJHU19TZW5k",
            "Q2hhdE1zZxCrjQYSFgoQR1MyQ19TZW5kQ2hhdE1zZxCsjQYSGwoVQzJHU19P",
            "TkxpbmVQbGF5ZXJMaXN0EK2NBhIbChVHUzJDX09OTGluZVBsYXllckxpc3QQ",
            "ro0GEhkKE0MyR1NfUGxheWVyU3RvcE1vdmUQr40GEhkKE0dTMkNfUGxheWVy",
            "U3RvcE1vdmUQsI0GQgxaCi9tZXNzYWdlSWRiBnByb3RvMw=="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(new[] {typeof(global::MessageId.MessageId), }, null, null));
    }
    #endregion

  }
  #region Enums
  public enum MessageId {
    [pbr::OriginalName("None")] None = 0,
    [pbr::OriginalName("C2S_Register_Accoount")] C2SRegisterAccoount = 50001,
    [pbr::OriginalName("S2C_Register_Accoount")] S2CRegisterAccoount = 50002,
    [pbr::OriginalName("C2S_CreatePlayer")] C2SCreatePlayer = 50003,
    [pbr::OriginalName("S2C_CreatePlayer")] S2CCreatePlayer = 50004,
    [pbr::OriginalName("C2S_Login")] C2SLogin = 50005,
    [pbr::OriginalName("S2C_Login")] S2CLogin = 50006,
    [pbr::OriginalName("S2C_Kick")] S2CKick = 50007,
    /// <summary>
    ///heartbeat
    /// </summary>
    [pbr::OriginalName("C2GS_MsgPing")] C2GsMsgPing = 80001,
    [pbr::OriginalName("C2GS_MsgPong")] C2GsMsgPong = 80002,
    /// <summary>
    ///player
    /// </summary>
    [pbr::OriginalName("C2GS_EnterSence")] C2GsEnterSence = 100001,
    [pbr::OriginalName("GS2C_EnterSence")] Gs2CEnterSence = 100002,
    [pbr::OriginalName("C2GS_PlayerMove")] C2GsPlayerMove = 100003,
    [pbr::OriginalName("GS2C_PlayerMove")] Gs2CPlayerMove = 100004,
    [pbr::OriginalName("C2GS_PlayerLeave")] C2GsPlayerLeave = 100005,
    [pbr::OriginalName("GS2C_PlayerLeave")] Gs2CPlayerLeave = 100006,
    [pbr::OriginalName("C2GS_AddFriend")] C2GsAddFriend = 100007,
    [pbr::OriginalName("GS2C_AddFriend")] Gs2CAddFriend = 100008,
    [pbr::OriginalName("C2GS_DelFriend")] C2GsDelFriend = 100009,
    [pbr::OriginalName("GS2C_DelFriend")] Gs2CDelFriend = 100010,
    [pbr::OriginalName("C2GS_SendChatMsg")] C2GsSendChatMsg = 100011,
    [pbr::OriginalName("GS2C_SendChatMsg")] Gs2CSendChatMsg = 100012,
    [pbr::OriginalName("C2GS_ONLinePlayerList")] C2GsOnlinePlayerList = 100013,
    [pbr::OriginalName("GS2C_ONLinePlayerList")] Gs2COnlinePlayerList = 100014,
    [pbr::OriginalName("C2GS_PlayerStopMove")] C2GsPlayerStopMove = 100015,
    [pbr::OriginalName("GS2C_PlayerStopMove")] Gs2CPlayerStopMove = 100016,
  }

  #endregion

}

#endregion Designer generated code
